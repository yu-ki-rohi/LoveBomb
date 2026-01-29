using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// シーン遷移管理
/// Singletonで保持し、ScriptableObjectから設定を読み込む
/// </summary>
public class SceneTransitionManager : SingletonMonoBehaviour<SceneTransitionManager>
{
    #region 設定データ

    [Header("シーン遷移設定")]
    private SceneTransitionSettings settings;
    private SceneDatabase database;

    #endregion

    #region フェード制御

    private FadeControllerBase fadeController;

    #endregion

    #region 内部状態

    private bool isTransitioning;
    internal bool IsTransitioning => isTransitioning;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        // TODO: 将来的には Addressables / AssetBundle に差し替え予定
        settings = Resources.Load<SceneTransitionSettings>("ScriptableObject/SceneTransitionSettings");
        database = Resources.Load<SceneDatabase>("ScriptableObject/SceneDatabase");

        if (settings == null)
            Debug.LogError("SceneTransitionSettings が見つかりません");

        if (database == null)
            Debug.LogError("SceneDatabase が見つかりません");
    }

    #region Public API

    /// <summary>
    /// 次のシーンへ遷移
    /// </summary>
    public void TransitionToNextScene(FadeMode fadeMode = FadeMode.SimpleColor)
    {
        if (isTransitioning) return;

        string current = SceneManager.GetActiveScene().name;
        SceneObject next = database.GetNextScene(current);

        if (next == null)
        {
            Debug.LogWarning($"Next scene not found from '{current}'");
            return;
        }

        TransitionToSceneAsync(next, fadeMode).Forget();
    }

    public void TransitionToCurrentScene(FadeMode fadeMode = FadeMode.SimpleColor)
    {
        if (isTransitioning) return;

        string current = SceneManager.GetActiveScene().name;
        SceneObject next = database.GetScene(current);

        if (next == null)
        {
            Debug.LogWarning($"Next scene not found from '{current}'");
            return;
        }

        TransitionToSceneAsync(next, fadeMode).Forget();
    }
    /// <summary>
    /// 前のシーンへ遷移
    /// </summary>
    public void TransitionToPreviousScene(FadeMode fadeMode = FadeMode.SimpleColor)
    {
        if (isTransitioning) return;

        string current = SceneManager.GetActiveScene().name;
        SceneObject prev = database.GetPreviousScene(current);

        if (prev == null)
        {
            Debug.LogWarning($"Previous scene not found from '{current}'");
            return;
        }

        TransitionToSceneAsync(prev, fadeMode).Forget();
    }

    /// <summary>
    /// SceneObject を指定して遷移
    /// </summary>
    public async UniTask TransitionToSceneAsync(
        SceneObject targetScene,
        FadeMode fadeMode = FadeMode.SimpleColor)
    {
        if (isTransitioning || targetScene == null)
            return;

        isTransitioning = true;

        InitFade(settings.simpleColorPrefab);

        // フェードアウト
        await fadeController.FadeOutAsync(settings.fadeOutSettings);

        // シーンロード
        await SceneManager.LoadSceneAsync(targetScene.SceneName);

        // フェードイン
        await fadeController.FadeInAsync(settings.fadeInSettings);

        isTransitioning = false;
    }

    #endregion

    #region Fade Init

    private void InitFade(GameObject prefab)
    {
        if (fadeController != null) return;

        var fadeObj = Instantiate(prefab);
        DontDestroyOnLoad(fadeObj);

        var canvas = fadeObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        fadeController = fadeObj.GetComponent<FadeControllerBase>();
    }
    #endregion
}
