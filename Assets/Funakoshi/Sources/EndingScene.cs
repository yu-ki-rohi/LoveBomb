using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    [SerializeField] ContentManagement contentManagement;
    [SerializeField] GameState gameState;
    [SerializeField] StageDataBase stageDataBase;

    private bool isFinish = false;

    void Start()
    {

        if(gameState.Score < stageDataBase.Stages[gameState.StageID].ScoreInfomation.ScoreBorder)
        {

            // TODO: Ž¸”sŽžˆ—
            AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Failed);


        }
        else
        {
            // TODO: ¬Œ÷Žžˆ—
            AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Succeed);


        }

        contentManagement.RunFirstContent();
    }
    void Update()
    {
        if (!contentManagement.IsAllContentEnd())
        {
            contentManagement.ContentUpdate();
        }
        else if (isFinish == false) 
        {
            SceneTransitionManager.Instance.TransitionToNextScene();
            isFinish = true;
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
