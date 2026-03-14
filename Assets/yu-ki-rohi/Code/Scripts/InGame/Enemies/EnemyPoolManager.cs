using UnityEngine;
// Œ»ó‚ÍManagedEnemy‚ğg‚Á‚Ä‚¢‚é‚Ì‚Å‚±‚¿‚ç‚Íg—p‚µ‚Ä‚¢‚È‚¢
#if false
public class EnemyPoolManager : PoolManager<Enemy>, IEnemyPoolManager
{
    [SerializeField] private EnemyDataList enemyDataList;
    [SerializeField] private ExplosionPoolManager explosionPool;

#if UNITY_EDITOR
    public EnemyDataList EnemyDataList { get =>  enemyDataList; }
#endif


    public void EnemyAppear(Vector3 position, Transform target, EnemyData data)
    {
        var enemy = objectPool.Get();
        enemy.Initialize(position, target, data);
    }

}
#endif