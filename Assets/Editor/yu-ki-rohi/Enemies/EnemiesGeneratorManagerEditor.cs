using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemiesGeneratorManager))]
public class EnemiesGeneratorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemiesGeneratorManager enemyGeneratorManager = (EnemiesGeneratorManager)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Set Enemy Generator"))
        {
            enemyGeneratorManager.SetGenerators();
        }

        if (GUILayout.Button("Set Enemy Pool to All Object"))
        {
            enemyGeneratorManager.SetPooToAllObjectInThisScene();
        }

        if (GUILayout.Button("Boot Generator On Start"))
        {
            enemyGeneratorManager.SetIsBootOnStart(true);
        }

        if (GUILayout.Button("Boot Generator By Manager"))
        {
            enemyGeneratorManager.SetIsBootOnStart(false);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyGeneratorManager);
        }
    }
}
