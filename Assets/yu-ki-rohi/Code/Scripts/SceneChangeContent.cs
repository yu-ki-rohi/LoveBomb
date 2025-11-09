using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeContent : CoroutineContent
{
    [SerializeField]
    private string nextSceneName;
    public override void ForcedEnd()
    {
        ScoreManager.Instance.ResetKillNum();
        SceneManager.LoadScene(nextSceneName);
    }
    public override void ProcessStarted() { }
}