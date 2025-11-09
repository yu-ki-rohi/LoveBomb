#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Drawing;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(GeneratorBase), true)] 
public class GeneratorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Chat GPTによるサンプルコードを使用※一部アレンジ

        // SerializedObjectを最新状態に更新
        serializedObject.Update();


        // デフォルトのインスペクタを描画
        DrawDefaultInspector();

        // 表示切替(常時)
        if (GUILayout.Button("Draw Area Always"))
        {
            // 表示切替時しか必要ないのでこのスコープ
            var generates = targets.Cast<GeneratorBase>().ToArray();

            foreach (var g in generates)
            {
                g.SetShowAlways();
                EditorUtility.SetDirty(g); // 変更を反映
            }
           
        }

        // 表示切替(選択時のみ)
        if (GUILayout.Button("Draw Area When Selected"))
        { 
            // 表示切替時しか必要ないのでこのスコープ
            var generates = targets.Cast<GeneratorBase>().ToArray();

            foreach (var g in generates)
            {
                g.SetShowWhenSelected();
                EditorUtility.SetDirty(g); // 変更を反映
            }

        }
    }
}
#endif