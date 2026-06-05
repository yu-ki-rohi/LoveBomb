using UnityEngine;
using UnityEngine.Pool;

// 敵の攻撃エフェクトの挙動を記述したクラス
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
        // 移動
        transform.position += moveDir * enemyCommonData.AnxietyPropagateSpeed * Time.deltaTime;

        // 目的地を通り過ぎたかを内積で検知
        if(Vector3.Dot(moveDir, targetPosition - transform.position) <= 0 )
        {
            // 心の核に向かっていた場合はスコアを渡して終了
            if(heartCore != null)
            {
                heartCore.AddEnemyScore(power);
                Deactivate();
                return;
            }
            // 向かう先が無かった場合はそこで終わり(この条件は普通はありえないという想定)
            else if(targetEnemy == null)
            {
                Deactivate();
                return;
            }

            // 敵から攻撃力を受け取る
            power += targetEnemy.Strength;

            // 次の目的地を受け取る
            var nextTargetEnemy = targetEnemy.HoldingHandsEnemy;
            if( nextTargetEnemy != null )
            {
                targetPosition = nextTargetEnemy.AnxietyEffectPos;
            }
            // 次の目的地が敵でなかったら、心の核が目的地のはず
            else
            {
                heartCore = targetEnemy.HeartCore;

                // 行き先がなければ終了
                if( heartCore == null )
                {
                    Deactivate();
                    return;
                }

                targetPosition = heartCore.transform.position;
            }
            targetEnemy = nextTargetEnemy;

            // 移動方向の再計算
            moveDir = (targetPosition - transform.position).normalized;
        }
    }
}
