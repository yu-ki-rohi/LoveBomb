using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    private int killNum = 0;

    public int GetKillNum()
    {
        return killNum;
    }

    public void CountKillNum()
    {
        killNum++;
    }

    public void ResetKillNum()
    {
        killNum = 0;
    }
}
