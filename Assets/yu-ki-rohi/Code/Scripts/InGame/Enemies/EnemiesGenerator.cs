using UnityEditor;
using UnityEngine;

public class EnemiesGenerator : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private ManagedEnemyPoolManager pool;
    [SerializeField, HideInInspector] private GeneratorBase generator;
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


    void OnDestroy()
    {
        generator?.UnlinkCallback(OnGenerate);
    }

    void Start()
    {
        if(generator == null )
        {
            Debug.LogError("Attach Generater in Utility");
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

    private void OnGenerate()
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
        pool.EnemyAppear(generator.DecideGeneratePosition(), target, enemyData);
    }
    #region エディタ限定
#if UNITY_EDITOR
    public void ForcedGenerate()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("This Execute Only in Playing!!");
            return;
        }
        OnGenerate();
    }

    public void AttachCircle()
    {
        AttachGenerator<CircleGenerator>();
    }

    public void AttachBox()
    {

        AttachGenerator<BoxGenerator>();
    }

    private void AttachGenerator<T>() where T : GeneratorBase
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("This Execute Only in Editor!!");
            return;
        }

        float initialGenerateDelay = 5.0f, generateInterval = 3.0f, generateIntervalRandomOffset = 0.0f;
        if (generator != null)
        {
            initialGenerateDelay = generator.InitialGenerateDelay;
            generateInterval = generator.GenerateInterval;
            generateIntervalRandomOffset = generator.GenerateIntervalRandomOffset;
            Undo.DestroyObjectImmediate(generator);
        }
        generator = Undo.AddComponent<T>(gameObject);
        generator.InitialGenerateDelay = initialGenerateDelay;
        generator.GenerateInterval = generateInterval;
        generator.GenerateIntervalRandomOffset = generateIntervalRandomOffset;
    }

#endif

    #endregion

}
