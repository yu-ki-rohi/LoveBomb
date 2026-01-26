using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour, IScoreFluctuate
{
    [SerializeField] private Image playerScoreBar;
    [SerializeField] private ScoreBonus scoreBonus;

    private DefeatNumViewer defeatNumViewer;
    private ScoreInfomation scoreInfomation;

    private int currentScore;

    private bool isLockScoreFluctuation = false;

    public ScoreInfomation ScoreInfomation { set { scoreInfomation = value; } }
    public DefeatNumViewer DefeatNumViewer { set { defeatNumViewer = value; } }
    
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

        currentScore += (int)(add * bounus);

        ReflectUI();

        if(currentScore < scoreInfomation.ScoreMax) { return; }

        currentScore = scoreInfomation.ScoreMax;

        // TODO: クリア処理呼び出し

    }

    public void ReduceScore(int sub)
    {
        if (isLockScoreFluctuation == true ||
            currentScore <= 0) { return; }

        currentScore -= sub;

        ReflectUI();

        if(currentScore > 0) { return; }

        currentScore = 0;

        // TODO: ゲームオーバー処理呼び出し

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
    }
}
