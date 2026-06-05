using System;

// Enemy‚ªŽg—p‚·‚éPool‚ð‚Ü‚Æ‚ß‚½‚à‚Ì
[Serializable]
public class PoolsEnemyUse
{
    public ExplosionPoolManager ExplosionPool;
    public AnxietyPropagationEffectPoolManager AnxietyPropagationEffectPool;
    public EnemyDropsPoolManager EnemyDropsPool;
    public HeartEnergyPoolManager HeartEnergyPool;
    public EffectPoolManager EffectPool;
    public CommonDropItemPoolManger DropItemPool;
}
