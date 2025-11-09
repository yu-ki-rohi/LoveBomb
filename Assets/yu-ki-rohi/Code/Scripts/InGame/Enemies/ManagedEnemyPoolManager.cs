using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ManagedEnemyPoolManager : PoolManager<ManagedEnemy>, IEnemyPoolManager
{
    [SerializeField] private EnemyDataList enemyDataList;
    [SerializeField] private EnemyCommonData enemyCommonData;
    [SerializeField] private ExplosionPoolManager explosionPool;
    [SerializeField] private AnxietyPropagationEffectPoolManager anxietyPropagationEffectPool;
    [SerializeField] private EnemyDropsPoolManager enemyDropsManager;
    [SerializeField] private EnemyManager enemyManager;
#if UNITY_EDITOR
    public EnemyDataList EnemyDataList { get => enemyDataList; }
#endif
    private void Start()
    {
        if (enemyManager == null)
        {
            Debug.LogWarning("EnemyManager is Null");
        }
    }



    public void EnemyAppear(Vector3 position, Transform target, EnemyData data)
    {
        if(DebugMessenger.NullCheckError(enemyManager)) { return; }

        if(enemyManager.IsEnemyMax)
        {
            //Debug.Log("Enemy Num reach Limit. Generating is canceled.");
            return;
        }

        var enemy = objectPool.Get();
        enemy.Initialize(position, target, data);

        enemyManager.AddManagedEnemy(enemy);

        //Debug.Log("Enemy appears!");
    }

    protected override ManagedEnemy Create()
    {
        var instance = base.Create();
        instance.OnCreate(enemyCommonData, explosionPool, anxietyPropagationEffectPool, enemyDropsManager);
        return instance;
    }

    protected override void OnReleaseToPool(ManagedEnemy enemy)
    {
        if (DebugMessenger.NullCheckWarning(enemyManager) == false)
        {
            enemyManager.RemoveManagedEnemy(enemy);
        }
        base.OnReleaseToPool(enemy);
    }


    protected override void OnDestroyPooledObject(ManagedEnemy enemy)
    {
        if (DebugMessenger.NullCheckWarning(enemyManager) == false)
        {
            enemyManager.RemoveManagedEnemy(enemy);
        }
        base .OnDestroyPooledObject(enemy);
    }
}

