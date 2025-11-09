using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif
// ※※※【重要】※※※
// エディター上でのみの使用を想定しているクラスです
// 特別な理由がない限りこのコンポーネントを取得したり実行しないでください
// もし何らかの理由で使用する場合は必ず【#if UNITY_EDITOR *** #endif】という形でくくってください(***は処理の内容を指します)
// 守っていただかないとビルド出来ないのでよろしくお願いいたします。

public class EnemyPoolBinder : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private ManagedEnemyPoolManager pool;
    [SerializeField] private bool isAllowedToRemoveEnemyPoolBinder = false;
    [SerializeField, HideInInspector] private bool enableAutoSettingPool = false;
    [SerializeField, HideInInspector] private HashSet<Transform> previousChildren = new HashSet<Transform>();


    public bool IsAllowedToRemoveEnemyPoolBinder { get => isAllowedToRemoveEnemyPoolBinder; }
    public bool EnableAutoSettingPool
    {
        get => enableAutoSettingPool;
        set
        {
            if(enableAutoSettingPool == true && value == false)
            {
                UnlinkOnHierarchyChanged();
            }
            else if(enableAutoSettingPool == false && value == true)
            {
                RegisterOnHierarchyChanged();
            }
            enableAutoSettingPool = value;
        }
    }

    public static void RemovePoolBinderFromAllGameObject(EnemyPoolBinder excuseObject)
    {
        if (excuseObject == null) { return; }
        if(excuseObject.IsAllowedToRemoveEnemyPoolBinder == false)
        {
            Debug.LogWarning("If you want to remove Pool Binders from All GameObject, you must check IsAllowedToRemoveEnemyPoolBinder");
            return;
        }

        var poolBinders = FindObjectsByType<EnemyPoolBinder>(FindObjectsSortMode.None);
        foreach (var poolBinder in poolBinders)
        {
            Undo.DestroyObjectImmediate(poolBinder);
        }
    }

    public void SetPooToAllObjectInThisScene()
    {
        if (pool == null)
        {
            Debug.LogWarning("Enemy Pool is Null");
        }
        Debug.Log("Start to set Enemy Pool to All GameObject");

        // IEnemyGeneratorを持つオブジェクトを取得
        var generators = FindObjectsByType<GeneratorBase>(FindObjectsSortMode.None).OfType<EnemiesGenerator>();
        foreach (var generator in generators)
        {
            generator.SetPool(pool);
        }
    }

    public void RegisterOrUnlinkOnHierarchyChanged()
    {
        EnableAutoSettingPool = !enableAutoSettingPool;
        SetRegisterOrUnlinkOnHierarchyChanged(enableAutoSettingPool);
    }

    public void SetRegisterOrUnlinkOnHierarchyChanged(bool enableAutoSettingPool)
    {
        HashSet<Transform> currentChildren = new HashSet<Transform>();
        foreach (Transform child in transform)
        {
            currentChildren.Add(child);
        }
        foreach (var child in currentChildren)
        {
            if(child.TryGetComponent<EnemyPoolBinder>(out var enemyPoolBinder))
            {
                enemyPoolBinder.EnableAutoSettingPool = enableAutoSettingPool;
                enemyPoolBinder.SetRegisterOrUnlinkOnHierarchyChanged(enableAutoSettingPool);
            }
        }
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            UnlinkOnHierarchyChanged();
        }
    }

    public void RegisterOnHierarchyChanged()
    {
        CacheCurrentChildren();
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        Debug.Log("Register OnHierarchyChanged with EditorApplication.hierarchyChanged");
    }

    public void UnlinkOnHierarchyChanged()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        Debug.Log("Unlink OnHierarchyChanged from EditorApplication.hierarchyChanged");
    }

    private void CacheCurrentChildren()
    {
        previousChildren.Clear();
        foreach (Transform child in transform)
        {
            previousChildren.Add(child);
        }
    }

    private void OnHierarchyChanged()
    {
        ProccessUpdatePool();
    }

    public void ProccessUpdatePool(bool isForcedExecution = false)
    {
        if(pool == null)
        {
            Debug.LogWarning("Enemy Pool is Null");
            return;
        }

        HashSet<Transform> currentChildren = new HashSet<Transform>();
        foreach (Transform child in transform)
        {
            currentChildren.Add(child);
        }
        if (isForcedExecution == true)
        {
            foreach (var newChild in currentChildren)
            {
                ProcessChildRecursively(newChild, isForcedExecution);
            }

        }
        else
        {
            // 新しく追加された直下の子
            foreach (var newChild in currentChildren)
            {

                if (!previousChildren.Contains(newChild))
                {
                    ProcessChildRecursively(newChild, isForcedExecution);
                }
            }
        }

        // キャッシュ更新
        previousChildren = currentChildren;
    }

    public void ProccessUpdatePool(ManagedEnemyPoolManager pool, bool isForcedExecution = false)
    {
        if(this.pool == null)
        {
            this.pool = pool;
        }
        ProccessUpdatePool(isForcedExecution);
    }

    private void ProcessChildRecursively(Transform child, bool isForcedExecution)
    {
        // IEnemyGenerator を持つコンポーネントを探す
        var generator = child.GetComponent<EnemiesGenerator>();
        if (generator != null)
        {
            // 見つかった場合はプールをセットして処理終了
            generator.SetPool(pool);
            return;
        }

        // 見つからなかった場合は自分と同じコンポーネントを追加
        EnemyPoolBinder enemyPoolBinder = child.gameObject.GetComponent<EnemyPoolBinder>(); // 自身と同じコンポーネントをアタッチ

        if(enemyPoolBinder == null)
        {
            enemyPoolBinder = child.gameObject.AddComponent<EnemyPoolBinder>(); // 自身と同じコンポーネントをアタッチ
        }
        enemyPoolBinder.EnableAutoSettingPool = enableAutoSettingPool;
        enemyPoolBinder.ProccessUpdatePool(pool, isForcedExecution);
    }
#endif
}


