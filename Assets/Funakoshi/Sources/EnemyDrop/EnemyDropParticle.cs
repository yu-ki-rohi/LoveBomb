using UnityEngine;

public class EnemyDropParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;

    [SerializeField]
    private ParticleSettings settings;

    private Transform coreTransform;
    private ICoreChargable coreCharge;

    public static void Instantiate(GameObject dropItemGameObject, Vector3 generatePos, Transform coreTransform, ICoreChargable coreCharge)
    {
        EnemyDropParticle dropItem = Instantiate(dropItemGameObject, generatePos, Quaternion.identity)
            .GetComponent<EnemyDropParticle>();

        dropItem.coreTransform = coreTransform;
        dropItem.coreCharge = coreCharge;

        dropItem.Init();
    }

    private float timer = 0f;

    enum State
    {
        Stand,
        Emitting,
        Staying,
        Moving,
        Terminate
    }

    private State currentState = State.Stand;

    private DropParticleManager particleManager;

    private void Init()
    {
        if (particle == null)
        {
            Debug.LogError("ParticleSystemがアタッチされていません");
            return;
        }
        if (coreTransform == null)
        {
            Debug.LogError("CoreTransformがアタッチされていません");
            return;
        }

        particleManager = new DropParticleManager(particle, settings, coreTransform);

        particleManager.InitSettings();

        ExecuteParticleBurst(particle.transform.position);
    }

    // パーティクルを開始
    private void ExecuteParticleBurst(Vector3 startPosition)
    {
        transform.position = startPosition;
        particle.Play();
        timer = 0f;
        currentState = State.Emitting;
    }

    void Update()
    {
        ParticleUpdate(timer);

        timer += Time.deltaTime;
    }
    public void ParticleUpdate(float timer)
    {
        switch (currentState)
        {
            case State.Stand:
                break;
            case State.Emitting:
                particleManager.EmittingUpdate();
                if (settings.emitDuration < timer)
                {
                    currentState = State.Staying;
                    particleManager.Stop();
                }
                break;
            case State.Staying:
                if (settings.emitDuration + settings.stayDuration < timer)
                {
                    currentState = State.Moving;
                    particleManager.Launch();
                }
                break;
            case State.Moving:
                particleManager.MovingToCoreUpdate();
                if (settings.emitDuration + settings.stayDuration + settings.moveDuration < timer)
                {
                    currentState = State.Terminate;
                }
                break;
            case State.Terminate:
                coreCharge.ChargeExecute();
                Destroy(gameObject);
                break;
        }
    }
}
