using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    [SerializeField] private Button start;
    [SerializeField] private float transiteDemoTime = 60.0f;

    void Start()
    {
        start.onClick.AddListener(OnTitleButtonClicked);
        GameInitializer.Instance.SetUpGameInitialize();
        AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Title);
        StartCoroutine(TransiteDemoCoroutine(transiteDemoTime));
    }

  
    private void OnTitleButtonClicked()
    {
        SceneTransitionManager.Instance.TransitionToNextScene(FadeMode.SimpleColor);
    }

    private IEnumerator TransiteDemoCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        AudioManager.Instance.StopBGM();
        SceneTransitionManager.Instance.TransitionToPreviousScene(FadeMode.SimpleColor);
    }

}
