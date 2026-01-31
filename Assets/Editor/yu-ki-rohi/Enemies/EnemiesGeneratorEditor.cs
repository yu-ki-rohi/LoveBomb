using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EnemiesGenerator))]

public class EnemiesGeneratorEditor : GeneratorCustomEditor
{

    public override void OnInspectorGUI()
    {
        EnemiesGenerator enemyGenerator = (EnemiesGenerator)target;

        EnemyDataList enemyDataList = enemyGenerator.EnemyDataList;
        if (enemyDataList != null && enemyDataList.EnemyList.Count > 0)
        {
            List<string> enemyNames = new List<string>();
            foreach (var enemy in enemyDataList.EnemyList)
            {
                enemyNames.Add(enemy.Name);
            }
            int currentIndex = Mathf.Max(0, enemyNames.IndexOf(enemyGenerator.EnemyName));

            // プルダウンを表示
            int newIndex = EditorGUILayout.Popup("It will generate", currentIndex, enemyNames.ToArray());

            // 選択が変わったら反映
            enemyGenerator.EnemyName = enemyNames[newIndex];
            enemyGenerator.SetEnemyData(newIndex);
        }
        else
        {
            EditorGUILayout.HelpBox("EnemyListを設定すると選択できます。", MessageType.Info);
        }

        base.OnInspectorGUI();
    }
}
