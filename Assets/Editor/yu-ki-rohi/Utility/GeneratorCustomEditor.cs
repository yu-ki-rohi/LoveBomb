using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generator))]
public class GeneratorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Generator enemyGenerator = (Generator)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Attach Circle Generator"))
        {
            enemyGenerator.AttachCircle();
        }

        if (GUILayout.Button("Attach Box Generator"))
        {
            enemyGenerator.AttachBox();
        }

        if (GUILayout.Button("Generate Enemy"))
        {
            enemyGenerator.ForcedGenerate();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyGenerator);
        }
    }
}
