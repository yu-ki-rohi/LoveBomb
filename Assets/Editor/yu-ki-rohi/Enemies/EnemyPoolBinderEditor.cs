using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyPoolBinder))]
public class EnemyPoolBinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyPoolBinder enemyPoolBinder = (EnemyPoolBinder)target;
        base.OnInspectorGUI();

        if (GUILayout.Button(enemyPoolBinder.EnableAutoSettingPool ? "Disable Auto Update" : "Enable Auto Update"))
        {
            enemyPoolBinder.RegisterOrUnlinkOnHierarchyChanged();
        }

        if (GUILayout.Button("Set Enemy Pool to Children Object"))
        {
            enemyPoolBinder.ProccessUpdatePool(true);
        }

        if (GUILayout.Button("Set Enemy Pool to All Object"))
        {
            enemyPoolBinder.SetPooToAllObjectInThisScene();
        }

        if (GUILayout.Button("Remove Pool Binder"))
        {
            EnemyPoolBinder.RemovePoolBinderFromAllGameObject(enemyPoolBinder);
            return;
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyPoolBinder);
        }
    }
}
