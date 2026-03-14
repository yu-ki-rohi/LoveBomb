using UnityEngine;

// 破壊可能なジェネレーターに付けるもの
public class DamageableGenerator : MonoBehaviour, IDamageable
{
    [SerializeField] private PoolsEnemyUse pools;
    [SerializeField] private EnemyGeneratorData data;
    private int currentHP;

    public void TakeDamage(int attack, DamageType damageType)
    {
        currentHP -= attack;

        if(currentHP <= 0)
        {
            Disappear();
        }
        else if(damageType == DamageType.Piercing)
        {
            if(DebugMessenger.NullCheckError(pools.EffectPool)) { return; }
            pools.EffectPool.PlayEffect(transform.position, EffectData.EffectType.HitEffect);
            AudioManager.Instance.PlaySEById(SEName.EnemyDamage);
        }
    }

    void Start()
    {
        if (DebugMessenger.NullCheckError(pools) || 
            DebugMessenger.NullCheckError(data)) { Destroy(gameObject); return; }
        currentHP = data.MaxHitPoint;
    }

    private void Disappear()
    {
        if (DebugMessenger.NullCheckError(pools.ExplosionPool)) { return; }
        pools.ExplosionPool.Explode(data.ExplosionPower, transform.position, data.ExplosionScale);


        if (DebugMessenger.NullCheckError(pools.EnemyDropsPool) ||
            data.BaseScore == 0) { return; }
        pools.EnemyDropsPool.DropEnergy(data.BaseScore, transform.position);
        Destroy(gameObject);
    }
}
