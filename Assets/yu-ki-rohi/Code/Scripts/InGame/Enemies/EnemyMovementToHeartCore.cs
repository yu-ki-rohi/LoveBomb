using UnityEngine;

// お化け型のエネミーの挙動
// 挙動周りは出来れば完全にこっちに委譲したい
public class EnemyMovementToHeartCore : IUpdatable
{
    private Transform transform;
    private Rigidbody2D rigidbody;
    private Transform target;
    private EnemyData enemyData;
    private bool canMove = true;

    public EnemyMovementToHeartCore(Transform transform, Rigidbody2D rigidbody, Transform target, EnemyData enemyData)
    {
        this.transform = transform;
        this.rigidbody = rigidbody;
        this.target = target;
        this.enemyData = enemyData;
    }

    public void Start()
    {

    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        if(canMove == false)
        {
            rigidbody.linearVelocity = Vector2.zero; 
            return; 
        }

        if (transform == null ||
            rigidbody == null ||
            target == null)
        {
            Debug.LogWarning("Lack of Movement Info");
            return;
        }

        // 一応念のため毎回方向を設定しなおす
        // normalizeはやや重めなので、一定時間毎にしてもいいかも
        rigidbody.linearVelocity = (target.position - transform.position).normalized * enemyData.Agility;
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
