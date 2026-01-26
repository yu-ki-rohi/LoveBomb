using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// HACK: EnemyManagerAPoolManager‚Ì–ğŠ„‚Ìüˆø‚«‚ª‚¨‚©‚µ‚¢

// NOTE: Œã‚©‚çEnemyManager‚ğ’Ç‰Á‚µ‚½‚Ì‚ÅA‚»‚ê‚É‚æ‚Á‚ÄŠÇ—‚³‚ê‚éEnemy‚ğ•Ê‚Éì‚Á‚Ä‚¢‚é

public class ManagedEnemyPoolManager : PoolManager<ManagedEnemy>, IEnemyPoolManager
{
    [SerializeField] private EnemyDataList enemyDataList;
    [SerializeField] private EnemyCommonData enemyCommonData;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PoolsEnemyUse poolsEnemyUse;

    // HACK: •Ê‚Ì‚Æ‚±‚ë‚ªó‚¯‚Â‚×‚«‚©‚à
    [SerializeField] private DefeatNumViewer defeatNumViewer;

    public DefeatNumViewer DefeatNumViewer { set  => defeatNumViewer = value; } 

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
        instance.OnCreate(enemyCommonData, poolsEnemyUse, defeatNumViewer);
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

