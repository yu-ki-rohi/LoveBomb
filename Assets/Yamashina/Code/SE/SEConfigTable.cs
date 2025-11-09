using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEConfig", menuName = "GameData/SEConfigTable")]
/// <summary>
/// ゲーム内で使用するSE（効果音）の設定一覧を管理するScriptableObject
/// </summary>
public class SEConfigTable : ScriptableObject
{
    #region SEのリストやディクショナリの内部管理用変数

    /// <summary>
    /// ゲーム内で使用するSE設定のリスト
    /// </summary>
    [SerializeField, Tooltip("▼ゲーム内で使用するSE設定のリスト")]
    private List<SEConfig> seLists = new List<SEConfig>();

    /// <summary>
    /// ゲーム内で使用するSE設定のリストのディクショナリ
    /// </summary>
    private Dictionary<SEName, SEConfig> seConfigDict;

    #endregion


    #region ゲッターメソッド

    /// <summary>
    /// リスト内のSEConfigをIDで探して返す
    /// </summary>
    /// <param name="id"></param>
    /// <returns>SEConfigのリスト</returns>
    internal SEConfig GetSeConfig(SEName id)
    {
        if (seConfigDict == null)
        {
            InitializeDictionary();
        }

        seConfigDict.TryGetValue(id, out var config);
        return config;
    }

    #endregion



    private void OnEnable()
    {
        // ScriptableObject 再読み込み時にも対応
        InitializeDictionary();
    }


    #region プライベートメソッド

    /// <summary>
    /// ディクショナリ初期化
    /// </summary>
    private void InitializeDictionary()
    {
        seConfigDict = new Dictionary<SEName, SEConfig>();
        foreach (var se in seLists)
        {
            //SE設定の一覧のリストのSeIdに文字列が入ってる＆ディクショナリにその文字列（キー）が含まれていないなら
            if (!string.IsNullOrEmpty(se.SeId.ToString()) && !seConfigDict.ContainsKey(se.SeId))
            {
                // ディクショナリにその文字列を追加
                seConfigDict.Add(se.SeId, se);
               
            }
            else
            {

            }
        }
    }

    #endregion

}


