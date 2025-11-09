using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    /// <summary>
    /// シングルトンインスタンスを取得
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //  インスタンスが存在しない場合、シーン内を検索
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    // インスタンスが見つからなければ新しく作成
                    SetupInstance();
                }
                else
                {
                    // 既にインスタンスが存在する場合のデバッグメッセージ
                    Debug.Log($"[Singleton] Instance of {typeof(T).Name} already created: {_instance.gameObject.name}");
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// インスタンスがシーン内に存在しない場合、インスタンスをセットアップ
    /// </summary>
    private static void SetupInstance()
    {
        GameObject gameObj = new GameObject(typeof(T).Name);
        _instance = gameObj.AddComponent<T>();
        DontDestroyOnLoad(gameObj);
    }

    /// <summary>
    /// 重複インスタンスの除去
    /// </summary>
    protected virtual void Awake()
    {
        // インスタンスがすでに存在するか確認し、存在する場合は自身を破棄
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
