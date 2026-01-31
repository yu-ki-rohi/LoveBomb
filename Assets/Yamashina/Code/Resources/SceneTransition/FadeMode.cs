using UnityEngine;

[System.Serializable]

public enum FadeMode
{
    SimpleColor,
    AnimatedPrefab,
    Particle
}

[System.Serializable]
public class FadeSettings
{
    [Range(0f, 1f), Tooltip("フェード開始時の透明度")]
    public float startAlpha = 0f;

    [Range(0f, 1f), Tooltip("フェード終了時の透明度")]
    public float endAlpha = 1f;


    /// <summary>
    /// フェードの速度
    /// </summary>
    [Header("ゲームのその他初期設定")]
    [SerializeField, Tooltip("フェードの速度")]
    private float fadeSpeed = 1f;

    [SerializeField, Tooltip("▼フェード設定")]
    private FadeMode defaultFadeMode = FadeMode.SimpleColor;

    /// <summary>
    /// フェードの速度の読み取り専用
    /// </summary>
    internal float FadeSpeed => fadeSpeed;
}

