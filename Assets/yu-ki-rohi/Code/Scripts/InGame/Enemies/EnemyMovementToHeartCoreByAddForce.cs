using UnityEngine;

public class EnemyMovementToHeartCoreByAddForce : IUpdatable
{
    private Transform transform;
    private Rigidbody2D rigidbody;
    private Transform target;
    private bool canMove = true;
    private EnemyIndividualData enemyIndividualData;

    public EnemyMovementToHeartCoreByAddForce(Transform transform, Rigidbody2D rigidbody, Transform target, EnemyIndividualData enemyIndividualData)
    {
        this.transform = transform;
        this.rigidbody = rigidbody;
        this.target = target;
        this.enemyIndividualData = enemyIndividualData;
    }

    public void Start()
    {
        if (DebugMessenger.NullCheckError(transform) ||
            DebugMessenger.NullCheckError(rigidbody) ||
            DebugMessenger.NullCheckError(target))
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }
        enemyIndividualData.MoveDir = (target.position - transform.position).normalized;
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        if (canMove == false)
        {
            rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (DebugMessenger.NullCheckError(transform) ||
            DebugMessenger.NullCheckError(rigidbody) ||
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

        rigidbody.AddForce(enemyIndividualData.MoveDir * rigidbody.linearDamping * enemyIndividualData.BasicData.Agility, ForceMode2D.Force);
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
        rigidbody.linearVelocity = Vector2.zero;
    }

    public void OnMove()
    {
        canMove = true;
    }

    public void OnDie()
    {
        canMove = false;
        rigidbody.linearVelocity = Vector2.zero;
    }
}
