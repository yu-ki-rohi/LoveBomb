using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class SceneManagerWithFade
{
    public static void LoadScene(string sceneName)
    {
        // 別スレッドで非同期処理を開始します
        UniTask.Void(async () =>
        {
            await LoadSceneWithFadeAsync(sceneName);
        });
    }

    private static async UniTask LoadSceneWithFadeAsync(string sceneName, CancellationToken cancellationToken = default)
    {
        ScreenFader screen = ScreenFader.Instance();

        await screen.FadeOutAsync(cancellationToken);

        await SceneManager.LoadSceneAsync(sceneName).WithCancellation(cancellationToken);
        
        await screen.FadeInAsync(cancellationToken);
    }
}
