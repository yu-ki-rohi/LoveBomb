using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    [SerializeField] ContentManagement contentManagement;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        BGMName bgmName = BGMName.None;

        switch (sceneName)
        {
            case string name when name == "GameOver":
                bgmName = BGMName.Failed; // リザルト失敗のBGM名
                break;

            case string name when name == "GameClear":

                bgmName = BGMName.Succeed;
                break;
            default:
                Debug.LogWarning($"No BGM assigned for the scene '{sceneName}'.");
                return; // BGMが指定されていない場合は終了

        }
        if (!string.IsNullOrEmpty(bgmName.ToString()))
        {
            AudioManager.Instance.PlayBGMIfNotPlaying(bgmName); // BGMを再生
            contentManagement.RunFirstContent();
        }
    }
    void Update()
    {
        if (!contentManagement.IsAllContentEnd())
        {
            contentManagement.ContentUpdate();
        }

        InputKeys();
    }
    void InputKeys()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!contentManagement.IsAllContentEnd())
            {
                contentManagement.SkipContent();
            }
        }
    }
}
