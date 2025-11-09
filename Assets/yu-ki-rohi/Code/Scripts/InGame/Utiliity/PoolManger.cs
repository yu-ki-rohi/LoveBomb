// 参考資料：https://qiita.com/KeichiMizutani/items/ca46a40de02e87b3d8a8

using UnityEngine;
using UnityEngine.Pool;

// yu-ki-rohi
// Tが何か分からない人は「C# ジェネリック」で検索してください
// ちなみにC++でいうところのテンプレートだと思います
public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour, IPooledObject<T>
{
    [SerializeField] private T pooledPrefab;
    protected IObjectPool<T> objectPool;

    // 既にプールに入っている既存のアイテムを返そうとすると例外が発生します
    private bool collectionCheck = true;
    // プール容量と最大サイズをコントロールする追加オプション
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 100;

    private void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        objectPool = new ObjectPool<T>(
            Create,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPooledObject,
            collectionCheck,
            defaultCapacity,
            maxSize);
    }

    protected virtual T Create()
    {

        if (pooledPrefab == null)
        {
            Debug.Log("Can't Find Object to Create.");
            return null;
        }
            
        T instance = Instantiate(pooledPrefab, transform.position, Quaternion.identity, transform);
        instance.ObjectPool = objectPool;
        return instance;
    }

    protected virtual void OnReleaseToPool(T pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    protected virtual void OnGetFromPool(T pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    protected virtual void OnDestroyPooledObject(T pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
