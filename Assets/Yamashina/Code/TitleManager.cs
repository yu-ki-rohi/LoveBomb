using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    [SerializeField] private Button start;

    void Start()
    {
        start.onClick.AddListener(OnTitleButtonClicked);
        AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Title);
    }

    private void OnTitleButtonClicked()
    {
        SceneTransitionManager.Instance.TransitionToNextScene(FadeMode.SimpleColor);
    }

}
