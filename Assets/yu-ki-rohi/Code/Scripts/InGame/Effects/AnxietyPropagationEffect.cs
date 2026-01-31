using UnityEngine;
using UnityEngine.Pool;

public class AnxietyPropagationEffect : MonoBehaviour, IPooledObject<AnxietyPropagationEffect>
{
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;
    private int power;

    private HeartCore heartCore = null;
    private Enemy targetEnemy;
    private EnemyCommonData enemyCommonData;

    private IObjectPool<AnxietyPropagationEffect> objectPool;
    public IObjectPool<AnxietyPropagationEffect> ObjectPool { set => objectPool = value; }
    
    public void OnCreate(EnemyCommonData enemyCommonData)
    {
        this.enemyCommonData = enemyCommonData;
    }

    public void Initialize(Enemy targetEnemy, Vector3 position, int power)
    {
        if(targetEnemy == null) { Deactivate(); return; }
        transform.position = position;
        this.targetEnemy = targetEnemy;
        targetPosition = targetEnemy.AnxietyEffectPos;
        moveDir = (targetPosition - transform.position).normalized;
        heartCore = null;
        this.power = power;
    }
    
    public void Initialize()
    {

    }

    public void Deactivate()
    {
        objectPool.Release(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * enemyCommonData.AnxietyPropagateSpeed * Time.deltaTime;

        if(Vector3.Dot(moveDir, targetPosition - transform.position) <= 0 )
        {
            // HACK：複雑すぎ、要リファクタリング
            if(heartCore != null)
            {
                heartCore.AddEnemyScore(power);
                Deactivate();
                return;
            }
            else if(targetEnemy == null)
            {
                Deactivate();
                return;
            }
            power += targetEnemy.Strength;
            var nextTargetEnemy = targetEnemy.HoldingHandsEnemy;
            if( nextTargetEnemy != null )
            {
                targetPosition = nextTargetEnemy.AnxietyEffectPos;
            }
            else
            {
                heartCore = targetEnemy.HeartCore;
                if( heartCore == null )
                {
                    Deactivate();
                    return;
                }
                targetPosition = heartCore.transform.position;
            }
            targetEnemy = nextTargetEnemy;

            moveDir = (targetPosition - transform.position).normalized;
        }
    }
}
