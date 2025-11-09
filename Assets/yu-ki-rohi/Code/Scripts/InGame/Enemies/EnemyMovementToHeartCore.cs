using UnityEngine;

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

        // ˆê‰”O‚Ì‚½‚ß–ˆ‰ñ•ûŒü‚ğİ’è‚µ‚È‚¨‚·
        // normalize‚Í‚â‚âd‚ß‚È‚Ì‚ÅAˆê’èŠÔ–ˆ‚É‚µ‚Ä‚à‚¢‚¢‚©‚à
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
