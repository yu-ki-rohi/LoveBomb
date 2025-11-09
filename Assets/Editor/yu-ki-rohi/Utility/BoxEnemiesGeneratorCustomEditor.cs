#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(BoxGenerator), true)]
public class BoxEnemiesGeneratorCustomEditor : GeneratorCustomEditor
{
    public override void OnInspectorGUI()
    {
        // Chat GPTによるサンプルコードを使用※一部アレンジ

        var boxGenerators = targets.Cast<BoxGenerator>().ToArray();

        base.OnInspectorGUI();

        // 範囲を指定
        foreach (var bg in boxGenerators)
        {
            Vector2 min = bg.MinLength;
            Vector2 max = bg.MaxLength;

            // 下限はともかく、上限はインスペクター上で変更できるようにしたい
            float lowerLimit = 0.0f;
            float upperLimit = 50.0f;

            EditorGUILayout.MinMaxSlider("X Range", ref min.x, ref max.x, lowerLimit, upperLimit);
            EditorGUILayout.MinMaxSlider("Y Range", ref min.y, ref max.y, lowerLimit, upperLimit);

            if(min != bg.MinLength || max != bg.MaxLength)
            {
                Undo.RecordObject(bg, "Change Area Range");
                bg.SetRangeX(min.x, max.x);
                bg.SetRangeY(min.y, max.y);
                EditorUtility.SetDirty(bg); // 変更を反映

                SceneView.RepaintAll(); // Sceneビューを即時更新
            }
        }


    }
}
#endif