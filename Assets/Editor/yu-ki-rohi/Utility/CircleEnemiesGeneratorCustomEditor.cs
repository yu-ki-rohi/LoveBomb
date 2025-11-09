#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CircleGenerator), true)]
public class CircleEnemiesGeneratorEditor : GeneratorCustomEditor
{
    public override void OnInspectorGUI()
    {
        var circleGenerators = targets.Cast<CircleGenerator>().ToArray();
        base.OnInspectorGUI();

        // 範囲を指定
        foreach (var cg in circleGenerators)
        {
            float inner = cg.InnerRadius;
            float outer = cg.OuterRadius;

            // 下限はともかく、上限はインスペクター上で変更できるようにしたい
            float lowerLimit = 0.0f;
            float upperLimit = 50.0f;

            EditorGUILayout.MinMaxSlider("Radius Range", ref inner, ref outer, lowerLimit, upperLimit);

            if(inner != cg.InnerRadius || outer != cg.OuterRadius)
            {
                Undo.RecordObject(cg, "Change Area Range");
                cg.SetInnerRadius(inner);
                cg.SetOuterRadius(outer);
                EditorUtility.SetDirty(cg); // 変更を反映

                SceneView.RepaintAll(); // Sceneビューを即時更新
            }
        }


    }
}
#endif