using UnityEngine;

public class DropParticleManager
{
    private readonly ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    private readonly ParticleSettings settings;
    private readonly Transform coreTransform;

    public DropParticleManager(ParticleSystem particleSystem, ParticleSettings settings, Transform coreTransform)
    {
        this.particleSystem = particleSystem;
        this.coreTransform = coreTransform;
        this.settings = settings;
        particles = new ParticleSystem.Particle[settings.particleCount];
    }
    public void InitSettings()
    {
        // ParticleSystemの初期設定
        var main = particleSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = settings.particleCount;
        main.startSize = settings.particleSize;
        main.startLifetime = settings.emitDuration + settings.stayDuration + 5f; // 十分な生存時間
        main.startSpeed = new ParticleSystem.MinMaxCurve(0, settings.emissionIntensity); 
        main.loop = false;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new(0f, settings.particleCount) });

        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;

        particles = new ParticleSystem.Particle[settings.particleCount];
    }

    public void EmittingUpdate()
    {
        int particleCount = particleSystem.GetParticles(particles);
        for (int i = 0; i < particleCount; i++)
        {
            // 0に向かって減速
            particles[i].velocity *= settings.emittingSpeed​Multiplier;
        }
        // パーティクルを反映
        particleSystem.SetParticles(particles, particleCount);
    }

    public void Stop()
    {
        int particleCount = particleSystem.GetParticles(particles);
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].velocity = Vector3.zero;
        }

        // パーティクルを反映
        particleSystem.SetParticles(particles, particleCount);
    }

    public void Launch()
    {
        int particleCount = particleSystem.GetParticles(particles);
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].velocity = Vector3.up * settings.launchPower;
        }

        // パーティクルを反映
        particleSystem.SetParticles(particles, particleCount);
    }

    public void MovingToCoreUpdate()
    {
        // 移動フェーズの進行度
        float t = Mathf.Clamp01((particleSystem.time - (settings.emitDuration + settings.stayDuration)) / settings.moveDuration);

        int particleCount = particleSystem.GetParticles(particles);
        for (int i = 0; i < particleCount; i++)
        {
            // パーティクルの速度を制限します
            float particleSpeed = Mathf.Min(particles[i].velocity.magnitude, settings.moveMaxSpeed);
            particles[i].velocity = particles[i].velocity.normalized * particleSpeed;

            // coreTransformまでの向き
            Vector3 toCore = coreTransform.position - particles[i].position;
            Vector3 direction = toCore.normalized;

            // coreに近づくほど透明にします
            Color color = particles[i].startColor;
            float colorAlpha = 1.0f - t * 1.0f;
            particles[i].startColor = new Color(color.r, color.g, color.b, colorAlpha);
            particles[i].velocity += direction * settings.moveAccelaration / Time.deltaTime; // 速度を更新
        }

        // パーティクルを反映
        particleSystem.SetParticles(particles, particleCount);
    }
}
