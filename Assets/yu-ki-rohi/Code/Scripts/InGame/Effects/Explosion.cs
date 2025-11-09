using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Pool;

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
            damageable.TakeDamage(power, DamageType.Explosion, 1.0f);
        }
    }
}
