using UnityEngine;
using UnityEngine.Pool;

// EnemyDropParticleを改造
[RequireComponent(typeof(ParticleSystem))]
public class PooledEnemyDrops : MonoBehaviour, IPooledObject<PooledEnemyDrops>, ICoreChargable
{
    [SerializeField]
    private ParticleSettings settings;

    private ParticleSystem particle;

    private HeartCore heartCore;

    private float timer = 0f;

    private int score = 0;
    
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
    private IObjectPool<PooledEnemyDrops> objectPool;

    public IObjectPool<PooledEnemyDrops> ObjectPool { set => objectPool = value; }

    public void Initialize(HeartCore heartCore, int score)
    {
        this.heartCore = heartCore;
        this.score = score;
        Initialize();
    }

    public void Initialize()
    {
        currentState = State.Stand;
        Init();
    }

    public void Deactivate()
    {
        objectPool.Release(this);
    }

    public void ChargeExecute()
    {
        if (DebugMessenger.NullCheckError(heartCore)) { return; }
        heartCore.AddPlayerScore(score);
    }

    private void Init()
    {
        if (particle == null)
        {
            Debug.LogError("ParticleSystemがアタッチされていません");
            return;
        }
        if (heartCore == null)
        {
            Debug.LogError("CoreTransformがアタッチされていません");
            return;
        }

        particleManager = new DropParticleManager(particle, settings, heartCore.transform);

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

    void Awake()
    {
        particle = GetComponent<ParticleSystem>(); 
    }

    void Update()
    {
        ParticleUpdate(timer);

        timer += Time.deltaTime;
    }
    private void ParticleUpdate(float timer)
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
                ChargeExecute();
                Deactivate();
                break;
        }
    }
}
