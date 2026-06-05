using UnityEngine;

// お化け型エネミーの挙動
// こちらはAddForceを通じて動かしている
public class EnemyMovementToHeartCoreByAddForce : IUpdatable
{
    private Transform transform;
    private Transform target;
    private bool canMove = true;
    private EnemyIndividualData enemyIndividualData;

    public EnemyMovementToHeartCoreByAddForce(Transform transform, Transform target, EnemyIndividualData enemyIndividualData)
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
        if (canMove == false)
        {
            enemyIndividualData.Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (DebugMessenger.NullCheckError(transform) ||
            DebugMessenger.NullCheckError(enemyIndividualData.Rigidbody) ||
            DebugMessenger.NullCheckError(target))
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }

        Vector3 toTargetVec = target.position - transform.position;
        if(Vector3.Dot(toTargetVec, enemyIndividualData.MoveDir) <= 0.0f)
        {
            enemyIndividualData.MoveDir = toTargetVec.normalized;
        }

        enemyIndividualData.Rigidbody.AddForce(enemyIndividualData.MoveDir * enemyIndividualData.Rigidbody.linearDamping * enemyIndividualData.BasicData.Agility, ForceMode2D.Force);
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
        canMove = false;
        enemyIndividualData.Rigidbody.linearVelocity = Vector2.zero;
    }

    public void OnMove()
    {
        canMove = true;
    }

    public void OnDie()
    {
        canMove = false;
        enemyIndividualData.Rigidbody.linearVelocity = Vector2.zero;
    }
}
