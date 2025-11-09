using UnityEngine;

/// <summary>
/// ゲームの初期設定クラス
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{

    #region ゲームの初期音量設定の内部管理用変数

    /// <summary>
    /// 同時に再生できる効果音の数
    /// </summary>
    [Header("ゲームの初期音量設定")]
    [SerializeField, Tooltip("同時に再生できる効果音の数")]
    private int maxSeCount = 3;

    [Space(15)]

    /// <summary>
    /// 初期 BGM 音量 (0.0 - 1.0)
    /// </summary>
    [SerializeField, Tooltip("初期 BGM 音量 (0.0 - 1.0)")]
    [Range(0f, 1f)] 
    private float initialBgmVolume = 1f;

    [Space(15)]

    /// <summary>
    /// 初期 SE 音量 (0.0 - 1.0)
    /// </summary>
    [SerializeField, Tooltip("初期 SE 音量 (0.0 - 1.0)")]
    [Range(0f, 1f)]
    private float initialSeVolume = 1f;

    #endregion


    #region ゲームのその他初期設定の内部管理用変数

    [Space(15)] 

    /// <summary>
    /// フェードの速度
    /// </summary>
    [Header("ゲームのその他初期設定")]
    [SerializeField, Tooltip("フェードの速度")] 
    private float fadeSpeed = 1f;

    /// <summary>
    /// リトライ時などの待機時間 (ms)
    /// </summary>
    [SerializeField, Tooltip("リトライ時などの待機時間 (ms)")]
    private int retryDelayMilliseconds = 100;

    #endregion


    #region　読み取り専用フィールド( ゲームの初期音量設定の内部管理用変数)

    /// <summary>
    /// 同時に再生できる効果音の数の読み取り専用
    /// </summary>
    internal int MaxSeCount => maxSeCount;

    /// <summary>
    /// 初期 BGM 音量 (0.0 - 1.0)の読み取り専用
    /// </summary>
    internal float InitialBgmVolume => initialBgmVolume;

    /// <summary>
    /// 初期 SE 音量 (0.0 - 1.0)の読み取り専用
    /// </summary>
    internal float InitialSeVolume => initialSeVolume;

    #endregion


    #region　読み取り専用フィールド( ゲームのその他初期設定の内部管理用変数)

    /// <summary>
    /// フェードの速度の読み取り専用
    /// </summary>
    internal float FadeSpeed => fadeSpeed;

    /// <summary>
    /// リトライ時などの待機時間 (ms)の読み取り専用
    /// </summary>
    internal int RetryDelayMilliseconds => retryDelayMilliseconds;

    #endregion

}
