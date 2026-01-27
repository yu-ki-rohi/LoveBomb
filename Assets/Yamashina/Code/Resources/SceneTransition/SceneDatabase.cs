using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneDatabase", menuName = "Scene/SceneDatabase")]
public class SceneDatabase : ScriptableObject
{
    [SerializeField] private List<SceneData> scenes = new List<SceneData>();

    private Dictionary<string, SceneData> sceneDict;

    [System.Serializable]
    public class SceneData
    {
        public SceneObject scene;
        public SceneObject nextScene;
        public SceneObject previousScene;
    }

    private void OnEnable()
    {
        sceneDict = new Dictionary<string, SceneData>();
        foreach (var s in scenes)
        {
            if (!string.IsNullOrEmpty((string)s.scene))
            {
                if (!sceneDict.ContainsKey((string)s.scene))
                    sceneDict.Add((string)s.scene, s);
                else
                    Debug.LogWarning($"SceneDatabase: Scene {(string)s.scene} already exists in database.");
            }
        }
    }

    public SceneObject GetScene(string name) => sceneDict.TryGetValue(name, out var data) ? data.scene : null;
    public SceneObject GetNextScene(string name) => sceneDict.TryGetValue(name, out var data) ? data.nextScene : null;
    public SceneObject GetPreviousScene(string name) => sceneDict.TryGetValue(name, out var data) ? data.previousScene : null;
}