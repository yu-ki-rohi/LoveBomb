using UnityEngine;
[CreateAssetMenu(fileName = "SceneTransitionSettings", menuName = "Settings/SceneTransitionSettings")]
public class SceneTransitionSettings : ScriptableObject
{
    [Header("▼フェード用Prefab")]
    public GameObject simpleColorPrefab;
    public GameObject animatedPrefab;
    public GameObject particlePrefab;



    [Header("▼フェードアウト設定")]
    public FadeSettings fadeOutSettings = new FadeSettings();

    [Header("▼フェードイン設定")]
    public FadeSettings fadeInSettings = new FadeSettings();
}


