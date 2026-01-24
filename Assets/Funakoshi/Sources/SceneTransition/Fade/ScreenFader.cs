using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(Canvas))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField]
    private TransparentImage screenPanel = new();

    [SerializeField]
    private FadeSetting fadeSetting;

    #region シングルトン

    private readonly static InstanceHolder<ScreenFader> singleton = new();

    public static ScreenFader Instance()
    {
        if (singleton.IsNotSet())
            throw new GameObjectNotPlacedException("ScreenFaderがシーン内に配置されていません");

        return singleton.Current();
    }

    private void SetSingleton()
    {
        if (singleton.IsAlreadySet())
        {
            Destroy(gameObject);
            return;
        }
        if (singleton.IsNotSet())
        {
            singleton.SetInstance(this);

            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    private void NullCheck()
    {
        screenPanel.NullCheck();

        if (!fadeSetting)
            throw new NullReferenceException("fadeSettingがアタッチされていません");
    }

    void Awake()
    {
        NullCheck();

        SetSingleton();
    }

    /// <summary>
    /// 画面全体をフェードアウトします
    /// </summary>
    public async UniTask FadeOutAsync(CancellationToken cancellationToken = default)
    {
        float elapsedTime = 0f;

        // フェードの進行度を計算します
        float FadeProgression()
        {
            return Mathf.Clamp01(elapsedTime / fadeSetting.FadeDuration);
        }

        while (FadeProgression() < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = FadeProgression();

            float alpha = fadeSetting.FadeCurve.Evaluate(t);

            // 透明度を設定します
            screenPanel.SetAlpha(alpha);

            await UniTask.Yield(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        // 最後に完全に黒くします
        screenPanel.SetAlpha(1f);
    }

    /// <summary>
    /// 画面全体をフェードインします
    /// </summary>
    public async UniTask FadeInAsync(CancellationToken cancellationToken = default)
    {
        float elapsedTime = 0f;

        // フェードの進行度を計算します
        float FadeProgression()
        {
            return Mathf.Clamp01(elapsedTime / fadeSetting.FadeDuration);
        }

        while (FadeProgression() < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = FadeProgression();

            float alpha = fadeSetting.FadeCurve.Evaluate(1 - t);

            // 透明度を設定します
            screenPanel.SetAlpha(alpha);

            await UniTask.Yield(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        // 最後に完全に透明にします
        screenPanel.SetAlpha(0f);
    }
}
