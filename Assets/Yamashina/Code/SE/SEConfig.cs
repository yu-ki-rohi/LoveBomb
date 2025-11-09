using UnityEngine;

/// <summary>
/// 単一のSE（効果音）の設定データ
/// </summary>
[System.Serializable]
public class SEConfig
{
    #region SE設定の内部管理用変数

    /// <summary>
    /// SEのID名
    /// </summary>
    [Header("▼ SE設定")]

    [SerializeField, Tooltip("SEのID名")]
    private SEName seId;

    [Space(15)]

    /// <summary>
    /// 使用するSEオーディオクリップ
    /// </summary>
    [SerializeField, Tooltip("使用するSEオーディオクリップ")]
    private AudioClip seAudioClip;

    [Space(15)]

    /// <summary>
    /// SEの説明
    /// </summary>
    [SerializeField, Tooltip("SEの説明")]
    private string description;  // 例：「ボタン押下音」など

    #endregion


    #region 読み取り専用プロパティ(SE設定の内部管理用変数)

    /// <summary>
    /// SEのID名の読み取り専用
    /// </summary>
    internal SEName SeId => seId;

    /// <summary>
    /// 使用するSEオーディオクリップの読み取り専用
    /// </summary>
    internal AudioClip SeAudioClip => seAudioClip;

    /// <summary>
    /// SEの説明の読み取り専用
    /// </summary>
    internal string Description => description;

    #endregion

}