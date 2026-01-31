using UnityEditor;
using UnityEngine;

#region SceneObject
/// <summary>
/// Inspector上でシーンを設定できるラッパークラス
/// 内部的にはシーン名（string）だけを保持する
/// </summary>
[System.Serializable]
public class SceneObject
{
    #region 内部管理用変数

    /// <summary>
    /// 対象のシーン名
    /// </summary>
    [SerializeField, Tooltip("対象のシーン名")]
    private string m_SceneName;

    #endregion

    #region 読み取り専用プロパティ

    /// <summary>
    /// 設定されているシーン名の取得
    /// </summary>
    public string SceneName => m_SceneName;

    #endregion

    #region 型変換（暗黙的変換）

    /// <summary>
    /// SceneObject → string に自動変換
    /// </summary>
    /// <param name="sceneObject">SceneObjectのインスタンス</param>
    public static implicit operator string(SceneObject sceneObject) => sceneObject?.m_SceneName;

    /// <summary>
    /// string → SceneObject に自動変換
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    public static implicit operator SceneObject(string sceneName) => new SceneObject() { m_SceneName = sceneName };

    #endregion
}
#endregion

#if UNITY_EDITOR

#region SceneObjectEditor
/// <summary>
/// SceneObjectのInspector表示をカスタマイズするPropertyDrawer
/// Build Settings に登録されたシーンのみ選択可能にする
/// </summary>
[CustomPropertyDrawer(typeof(SceneObject))]
public class SceneObjectEditor : PropertyDrawer
{
    #region Build SettingsからSceneAssetを取得

    /// <summary>
    /// Build Settings に登録されたSceneAssetを取得
    /// </summary>
    /// <param name="sceneName">探すシーン名</param>
    /// <returns>SceneAsset または null</returns>
    private SceneAsset FindSceneAsset(string sceneName)
    {
        // シーン名が空の場合は null を返す
        if (string.IsNullOrEmpty(sceneName)) return null;

        // Build Settings に登録されている全シーンを検索
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.path.Contains(sceneName))
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
        }

        // 見つからなければ警告
        Debug.LogWarning($"Scene '{sceneName}' is not in Build Settings.");
        return null;
    }

    #endregion

    #region Inspector描画

    /// <summary>
    /// Inspector上にSceneObjectを描画する
    /// </summary>
    /// <param name="position">描画する位置とサイズ</param>
    /// <param name="property">描画対象のSerializedProperty</param>
    /// <param name="label">ラベル</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // SceneObject内部のシーン名を取得
        var nameProp = property.FindPropertyRelative("m_SceneName");

        // Build Settingsに登録されたSceneAssetを取得
        var sceneAsset = FindSceneAsset(nameProp.stringValue);

        // InspectorにSceneAsset選択フィールドを描画
        var newScene = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false) as SceneAsset;

        // 選択された場合は m_SceneName にセット、nullの場合は空文字に
        nameProp.stringValue = newScene != null ? newScene.name : "";
    }

    #endregion
}
#endregion
#endif
