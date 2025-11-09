using UnityEditor;
using UnityEngine;

// yu-ki-rohi
// サムネ表示のため
// ソースはほぼChatGPTのもの
// 本当は完全にカスタムしたいところだけど、時間を考えるてプレビューだけにとどめる
[CustomEditor(typeof(EnemyDataList))]
public class EnemyDataListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyDataList dataList = (EnemyDataList)target;
        base.OnInspectorGUI();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Enemy List Preview", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (dataList.EnemyList != null)
        {
            foreach (var enemy in dataList.EnemyList)
            {
                if (enemy == null) continue;

                // 描画を横並びに
                EditorGUILayout.BeginHorizontal();

                // サムネイル描画
                int size = 72;
                if (enemy.Image != null)
                {
                    // Sprite → Texture2D
                    Texture2D texture = enemy.Image.texture;
                    GUILayout.Label(texture, GUILayout.Width(size), GUILayout.Height(size));
                }
                else
                {
                    GUILayout.Label("", GUILayout.Width(size), GUILayout.Height(size));
                }

                // 名前などの情報
                // 縦並びに
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(enemy.Name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Type: {enemy.Type}");
                EditorGUILayout.LabelField($"HP: {enemy.MaxHitPoint} / STR: {enemy.Strength}/ AGI: {enemy.Agility}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
        }
        // Previewの更新用
        if (GUILayout.Button("Refresh"))
        {
            EditorUtility.SetDirty(dataList);
        }
    }
}
