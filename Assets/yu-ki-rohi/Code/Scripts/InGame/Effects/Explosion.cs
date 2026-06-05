using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Pool;

// 爆発による攻撃を記述するため
// 判定のON/OFFはアニメーションで付けている
public class Explosion : MonoBehaviour, IPooledObject<Explosion>
{
    private int power;
    private IObjectPool<Explosion> objectPool;
    public IObjectPool<Explosion> ObjectPool { set => objectPool = value; }

    public void Initialize(int power)
    {
        this.power = power;
    }

    public void Initialize()
    {

    }

    public void Deactivate()
    {
        if (objectPool == null)
        {
            Debug.LogAssertion("ObjectPool is Null");
            Destroy(gameObject); return;
        }
        objectPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {

            var damageable = collision.GetComponent<IDamageable>();
            if(DebugMessenger.NullCheckError(damageable)) { return; }
            damageable.TakeDamage(power, DamageType.Explosion);
        }
    }
}
