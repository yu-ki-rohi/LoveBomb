using UnityEditor;
using UnityEngine;

// 敵を生成させるクラス
// Generator派生とGeneratorBase派生は別系統であることに注意
// 出来れば適切な名前に変えたいが、作業量的にそこまでは難しい

// 現状はジェネレーターをマップ上に生成してここのパラメーターをいじってバランス調整みたいな造りだが、
// 出来ればタイムテーブル的なのを参照する形で一括化するか、メタAIでプレイ状況を参照しながら生成していく形にしたい
public class EnemiesGenerator : Generator
{
    [SerializeField] Transform target;
    [SerializeField] private ManagedEnemyPoolManager pool;
    [SerializeField, HideInInspector] private EnemyData enemyData;

    #region エディタ限定
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private string enemyName = "";

    public string EnemyName { get => enemyName; set => enemyName = value; }

    public EnemyDataList EnemyDataList
    {
        get
        {
            if (pool == null)
            {
                Debug.LogError("Enemy PoolManager is Null!");
                return null;
            }
            return pool.EnemyDataList;
        }
    }

    public void SetEnemyData(int index)
    {
        if (enemyData == EnemyDataList.EnemyList[index]) { return; }
        enemyData = EnemyDataList.EnemyList[index];
        Debug.Log("Set Enemy Data : " + EnemyDataList.EnemyList[index].Name);
        EditorUtility.SetDirty(this);
    }

    public void SetPool(ManagedEnemyPoolManager ePool)
    {
        Debug.Log("Set Enemy Pool Manager.");
        pool = ePool;

        if (pool == null)
        {
            Debug.LogWarning("Enemy Pool Manager is not set!!");
        }

        EditorUtility.SetDirty(this);
    }

#endif


    #endregion

    public EnemyData EnemyData { get { return enemyData; } }

    public Transform Target { get { return transform; } set { target = value; } }

    void Start()
    {
        if(DebugMessenger.NullCheckError(generator))
        {
            gameObject.SetActive(false);
            return;
        }
        if(pool == null)
        {
            Debug.LogError("EnemyPool is Null");
            gameObject.SetActive(false);
            return;
        }
        generator.RegisterCallback(OnGenerate);
    }

    protected override void OnGenerate()
    {
        if(generator == null)
        {
            Debug.LogError("Generator is Null!");
            return;
        }
        if(pool == null)
        {
            Debug.LogError("EnemyPool is Null!");
            return;
        }
        int num = generateNumAtOnce + Random.Range(-generateNumRange, generateNumRange);
        for(int i = 0; i < num; i++)
        {
            pool.EnemyAppear(generator.DecideGeneratePosition(), target, enemyData);
        }
    }
}
