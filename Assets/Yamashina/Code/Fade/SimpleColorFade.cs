using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SimpleColorFade : FadeControllerBase
{
    [SerializeField] private Image fadeImage;

    public override async UniTask FadeOutAsync(FadeSettings settings)
    {
        await FadeCoroutineAsync(settings.startAlpha, settings.endAlpha, settings.FadeSpeed);
    }

    public override async UniTask FadeInAsync(FadeSettings settings)
    {
        await FadeCoroutineAsync(settings.startAlpha, settings.endAlpha, settings.FadeSpeed);
    }

    private async UniTask FadeCoroutineAsync(float start, float end, float speed)
    {
        Color color = fadeImage.color;
        color.a = start;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);

        while (!Mathf.Approximately(color.a, end))
        {
            if (fadeImage == null)
                return;
            color.a = Mathf.MoveTowards(color.a, end, speed * Time.unscaledDeltaTime);
            fadeImage.color = color;
            await UniTask.Yield();
        }

        if (Mathf.Approximately(end, 0f))
            fadeImage.gameObject.SetActive(false);
    }
}
