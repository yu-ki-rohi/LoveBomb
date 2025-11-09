using UnityEngine;
using UnityEngine.Pool;

public class ManagedEnemy : Enemy, IEnemyManaged, IPooledObject<ManagedEnemy>
{

    private IObjectPool<ManagedEnemy> pool;
    public new IObjectPool<ManagedEnemy> ObjectPool { set { pool = value; } }
    public override void Deactivate()
    {
        base.Deactivate();
        pool.Release(this);
    }

    public bool ManagedUpdate()
    {
        if (gameObject.activeInHierarchy == false) { return false; }
        CheckHoldingHands();

        foreach (var enemyComoponent in enemyComponents)
        {
            enemyComoponent.Update(Time.deltaTime);
        }
        return true;
    }

    public bool ManagedFixedUpdate()
    {
        if (gameObject.activeInHierarchy == false) { return false; }

        foreach (var enemyComoponent in enemyComponents)
        {
            enemyComoponent.FixedUpdate(Time.fixedDeltaTime);
        }
        return true;
    }

    void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

}
