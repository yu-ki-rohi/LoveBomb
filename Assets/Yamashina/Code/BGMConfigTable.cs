using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内で使用するBGM設定の一覧を保持する ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "BGMConfig", menuName = "GameData/BGMConfigTable")]
public class BGMConfigTable : ScriptableObject
{
    #region BGMのリストやディクショナリの内部管理用変数

    /// <summary>
    /// ゲーム内で使用するBGM設定の一覧のリスト
    /// </summary>
    [Header("▼ゲーム内で使用するBGM設定の一覧")]

    [SerializeField, Tooltip("ゲーム内で使用するBGM設定のリスト")]
    private List<BGMConfig> bgmLists = new List<BGMConfig>();

    /// <summary>
    /// BGMのディクショナリ
    /// </summary>
    private Dictionary<BGMName, BGMConfig> bgmDict;

    #endregion


    #region 読み取り専用プロパティ（BGMのリストやディクショナリの内部管理用変数)

    /// <summary>
    /// ゲーム内で使用するBGM設定の一覧の読み取り専用
    /// </summary>
    internal List<BGMConfig> BgmLists => bgmLists;

    #endregion


    #region ゲッターメソッド

    /// <summary>
    ///BGMのリスト情報をすべて返す  
    /// </summary>
    /// <returns>BGMConfigのList</returns>
    internal List<BGMConfig> GetAllBgmConfigs()
    {
        return bgmLists;
    }

    /// <summary>
    /// リスト内のBGMConfigをIDで探して返す
    /// </summary>
    /// <param name="id">BGMID</param>
    /// <returns>BGMConfig</returns>
    internal BGMConfig GetBgmConfig(BGMName id)
    {
        if (bgmDict == null)
        {
            InitializeDictionary();
        }

        bgmDict.TryGetValue(id, out var config);
        return config;
    }

    /// <summary>
    /// 指定した BGM ID が複数回登録されているかをチェック
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal bool IsDuplicateBgmId(BGMName id)
    {
        int count = 0;
        foreach (var bgm in bgmLists)
        {
            if (bgm.BgmId.Equals(id))
            {
                count++;
            }
        }
        return count > 1;
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
        bgmDict = new Dictionary<BGMName, BGMConfig>();
        foreach (var bgm in bgmLists)
        {
            string bgmId = bgm.BgmId.ToString();

            //BGMリストのBGMIDに文字列が入ってる＆ディクショナリにその文字列（キー）が含まれていないなら
            if (!string.IsNullOrEmpty(bgmId) && !bgmDict.ContainsKey(bgm.BgmId))
            {
                // ディクショナリにその文字列を追加
                bgmDict.Add(bgm.BgmId, bgm);
                foreach (var key in bgmDict.Keys)
                {
                    //どのキーが登録されているかのデバッグログ
                    Debug.Log($"登録されているBGMキー: {key}");
                }
            }
            else
            {
                //同じキーを登録しようとしているかBGMIDが空白
                Debug.LogWarning($"[BGMConfigTable] 重複または空のBGM ID: {bgm.BgmId}");
            }
        }
    }

    #endregion

}
