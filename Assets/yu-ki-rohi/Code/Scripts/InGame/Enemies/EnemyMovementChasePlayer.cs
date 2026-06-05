using UnityEngine;

// こうもり型のエネミーの挙動
public class EnemyMovementChasePlayer : IUpdatable
{
    private Transform transform;
    private Transform target;
    private EnemyIndividualData enemyIndividualData;

    private Animator animator;
    private State state = State.Chase;
    private float sign = 1.0f;

    private float timer = 0.0f;

    enum State
    {
        Chase,
        Standby,
        PrepareRush,
        Rush,
        Die
    }

    public EnemyMovementChasePlayer(Transform transform, Transform target, EnemyIndividualData enemyIndividualData)
    {
        this.transform = transform;
        this.target = target;
        this.enemyIndividualData = enemyIndividualData;
        enemyIndividualData.MoveDir = (target.position - transform.position).normalized;
        
    }

    public void Start()
    {
        if (DebugMessenger.NullCheckError(transform) ||
           DebugMessenger.NullCheckError(enemyIndividualData.Rigidbody) ||
           DebugMessenger.NullCheckError(target))
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        switch(state)
        {
            case State.Chase:
                Chase();
                break; 
            case State.Standby:
                Standby(fixedDeltaTime);
                break;
            case State.PrepareRush:
                PrepareRush(fixedDeltaTime);
                break;
            case State.Rush:
                Rush();
                break;
        }
    }

    public void Update(float deltaTime)
    {

    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }

    public void OnAttack()
    {
        
    }

    public void OnMove()
    {
        
    }

    public void OnDie()
    {
        state = State.Die;
        enemyIndividualData.Rigidbody.linearVelocity = Vector3.zero;
        enemyIndividualData.Animator.SetTrigger("Disappear");
    }

    // 大した行動はないのでステートベース
    private void Chase()
    {
        if (DebugMessenger.NullCheckError(transform) ||
            DebugMessenger.NullCheckError(enemyIndividualData.Rigidbody) ||
            DebugMessenger.NullCheckError(target))
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }

        Vector3 toTargetVec = target.position - transform.position;
        if (Vector3.Dot(toTargetVec, enemyIndividualData.MoveDir) <= 0.0f)
        {
            enemyIndividualData.MoveDir = toTargetVec.normalized;
        }

        enemyIndividualData.Rigidbody.AddForce(enemyIndividualData.MoveDir * enemyIndividualData.Rigidbody.linearDamping * enemyIndividualData.BasicData.Agility, ForceMode2D.Force);

        float borderDistance = enemyIndividualData.BasicData.StandbyDistance;

        // 遷移条件
        if (toTargetVec.sqrMagnitude < borderDistance * borderDistance)
        {
            state = State.Standby; 
            int judge = Random.Range(0, 2);
            if (judge == 1)
            {
                sign *= -1.0f;
            }
        }
    }

    private void Standby(float fixedDeltaTime)
    {
        if (DebugMessenger.NullCheckError(transform) ||
           DebugMessenger.NullCheckError(enemyIndividualData.Rigidbody) ||
           DebugMessenger.NullCheckError(target))
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }

        Vector3 toTargetVec = target.position - transform.position;
        Vector3 toTargetVecNorm = toTargetVec.normalized;

        enemyIndividualData.MoveDir = new Vector3(-toTargetVecNorm.y, toTargetVecNorm.x, 0.0f);


        enemyIndividualData.Rigidbody.AddForce((enemyIndividualData.MoveDir * sign) * enemyIndividualData.Rigidbody.linearDamping * enemyIndividualData.BasicData.Agility, ForceMode2D.Force);


        // 遷移関係

        EnemyData enemyData = enemyIndividualData.BasicData;
        float borderDistance = enemyData.StandbyDistance + enemyData.StandbyDistanceBaffar;

        if (toTargetVec.sqrMagnitude > borderDistance * borderDistance)
        {
            state = State.Chase;
            enemyIndividualData.Rigidbody.linearVelocity = Vector3.zero;
        }

        timer += fixedDeltaTime;

        if(timer > enemyData.RushInterval)
        {
            timer = 0;
            enemyIndividualData.Rigidbody.linearVelocity = Vector3.zero;
            state = State.PrepareRush;
            enemyIndividualData.Animator.SetTrigger("Attack");
        }
    }

    private void PrepareRush(float fixedDeltaTime)
    {
        timer += fixedDeltaTime;

        if (timer > enemyIndividualData.BasicData.PrepareRushTime)
        {
            timer = 0;
            enemyIndividualData.MoveDir = (target.position - transform.position).normalized;
            state = State.Rush;
            AudioManager.Instance.PlaySEById(SEName.BatChargePrepare);

            enemyIndividualData.Animator.SetTrigger("Attack");
        }
    }

    private void Rush()
    {
        enemyIndividualData.Rigidbody.AddForce(enemyIndividualData.MoveDir * enemyIndividualData.Rigidbody.linearDamping * enemyIndividualData.BasicData.Agility * enemyIndividualData.BasicData.RushSpeedMultiplier, ForceMode2D.Force);
        Vector3 toTargetVec = target.position - transform.position;

        // 終了判定の値
        // 本当はパラメーター化すべきだが、まあ説明しにくいので今回は埋め込み
        float judgeValue = -6.0f;

        if (Vector3.Dot(toTargetVec, enemyIndividualData.MoveDir) <= judgeValue)
        {
            state = State.Chase;
            AudioManager.Instance.PlaySEById(SEName.BatCharge);

            enemyIndividualData.Animator.SetTrigger("Attack");
        }
    }

}
