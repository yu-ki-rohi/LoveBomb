using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour, IScoreFluctuate
{
    [SerializeField] private Image playerScoreBar;
    [SerializeField] private ScoreBonus scoreBonus;

    private event Action OnTouchUp;

    private DefeatNumViewer defeatNumViewer;
    private ScoreInfomation scoreInfomation;
    private LightManager lightManager;

    private int currentScore;

    private bool isLockScoreFluctuation = false;

    public ScoreInfomation ScoreInfomation { set { scoreInfomation = value; } }
    public DefeatNumViewer DefeatNumViewer { set { defeatNumViewer = value; } }
    public LightManager LightManager { set { lightManager = value; } }

    public int CurrentScore { get { return currentScore; } }

    public void SetOnTouchUpEvent(Action action)
    {
        OnTouchUp += action;
    }
    
    public void LockScoreFluctuation()
    {
        isLockScoreFluctuation = true;
    }

    public void AddScore(int add)
    {
        if (isLockScoreFluctuation == true ||
            currentScore >= scoreInfomation.ScoreMax) { return; }

        int defeatNum = defeatNumViewer.DefeatNum;
        float bounus = 1.0f;
        for(int i = scoreBonus.Length - 1; i > -1; i--) 
        {
            if (defeatNum > scoreBonus.BonusBorder[i] )
            {
                bounus = scoreBonus.Bonus[i];
                break;
            }
        }

        int tmp = currentScore;
        currentScore += (int)(add * bounus);

        if(tmp < scoreInfomation.ScoreBorder && currentScore >= scoreInfomation.ScoreBorder)
        {
            // TODO: スコアが一定値を越えたことを知らせる効果音
            AudioManager.Instance.PlaySEById(SEName.ClearScoreReached);
        }


        ReflectUI();

        if(currentScore < scoreInfomation.ScoreMax) { return; }

        currentScore = scoreInfomation.ScoreMax;

        DebugMessenger.Log("Touch Up!! Winner : Player!");

        OnTouchUp?.Invoke();

    }

    public void ReduceScore(int sub)
    {
        if (isLockScoreFluctuation == true ||
            currentScore <= 0) { return; }

        currentScore -= sub;

        ReflectUI();

        if(currentScore > 0) { return; }

        currentScore = 0;
        DebugMessenger.Log("Touch Up!! Winner : Enemy!");
        OnTouchUp?.Invoke();
    }


    void Start()
    {
        currentScore = scoreInfomation.ScoreInitial;
        ReflectUI();
    }

    private void ReflectUI()
    {
        float fillAmount = 0.0f;

        if(currentScore < scoreInfomation.ScoreBorder)
        {
            fillAmount = (float)currentScore / scoreInfomation.ScoreBorder * 0.5f;
        }
        else
        {
            fillAmount = (float)(currentScore - scoreInfomation.ScoreBorder) / (scoreInfomation.ScoreMax - scoreInfomation.ScoreBorder) + 0.5f;
        }

        playerScoreBar.fillAmount = fillAmount;

        lightManager?.ChangeLight(fillAmount);
    }
}
