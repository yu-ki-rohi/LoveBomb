using UnityEngine;

/// <summary>
/// 単一のBGMの設定データ
/// </summary>
[System.Serializable]
public class BGMConfig
{
    #region  BGMの内部管理用変数

    /// <summary>
    /// BGMのID
    /// </summary>
    [SerializeField, Tooltip("BGMのID")]
    private BGMName bgmId;

    [Space(15)]

    /// <summary>
    /// 使用するオーディオクリップ
    /// </summary>
    [SerializeField, Tooltip("使用するオーディオクリップ")]
    private AudioClip bgmAudioClip;

    [Space(15)]

    /// <summary>
    /// 表示用の曲名
    /// </summary>
    [SerializeField, Tooltip("表示用の曲名")]
    private string bgmDisplayName;

    [Space(15)]

    /// <summary>
    /// ジャケット画像
    /// </summary>
    [SerializeField, Tooltip("ジャケット画像")]
    private Sprite bgmJacketImage;

    #endregion


    #region  読み取り専用プロパティ (BGMの内部管理用変数)

    /// <summary>
    /// BGMのIDの読み取り専用
    /// </summary>
    internal BGMName BgmId => bgmId;

    /// <summary>
    /// 使用するオーディオクリップの読み取り専用
    /// </summary>
    internal AudioClip BgmAudioClip => bgmAudioClip;

    /// <summary>
    /// 表示用の曲名の読み取り専用
    /// </summary>
    internal string BgmDisplayName => bgmDisplayName;

    /// <summary>
    /// ジャケット画像の読み取り専用
    /// </summary>
    internal Sprite BgmJacketImage => bgmJacketImage;

    #endregion
}