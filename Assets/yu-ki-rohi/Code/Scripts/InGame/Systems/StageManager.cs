using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StageManager : MonoBehaviour
{
    [SerializeField] private EffectPoolManager effectPoolManager;
    [SerializeField] private ManagedEnemyPoolManager managedEnemyPoolManager;
    [SerializeField] private ExplosionPoolManager expsionPoolManager;
    [SerializeField] private EnemiesGeneratorManager enemiesGeneratorManager;
    [SerializeField] private HeartCore heartCore;
    [SerializeField] private Light2D heartCoreLight;
    [SerializeField] private CompositeCollider2D visibleArea;
    [SerializeField] private Transform initialTransform;

    private Transform player;

    public EffectPoolManager EffectPoolManager { get { return effectPoolManager; }  }
    public ManagedEnemyPoolManager ManagedEnemyPoolManager { get { return managedEnemyPoolManager; } }
    public ExplosionPoolManager ExpsionPoolManager { get { return expsionPoolManager; } }
    public EnemiesGeneratorManager EnemiesGeneratorManager { get { return enemiesGeneratorManager; } }
    public HeartCore HeartCore { get { return heartCore; } }
    public Light2D HeartCoreLight { get { return heartCoreLight; } }
    public CompositeCollider2D VisibleArea { get { return visibleArea; } }

    public Transform PlayerTransfrom { set { PlayerTransfrom = value; } }

    public void SetInitialPositionOfPlayer(Transform player)
    {
        player.position = initialTransform.position;
        // ついでにジェネレーターへの通知
        enemiesGeneratorManager.SetTargetToGenerators(player);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
