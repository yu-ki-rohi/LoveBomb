using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimelineAsset))]
public class TimelineAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Open Timeline Editor"))
        {
            TimelineWindow.OpenWithAsset(target as TimelineAsset);
        }
    }
}
