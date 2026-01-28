using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject
{
    private int stageID;
    private int score;
    private float clearTime;
    private int comboMax;


    public int StageID
    { 
        get { return stageID; }

        set
        {
            if(stageID == value) { return; }
            DebugMessenger.Log("Change StageID : " + stageID + " Å® " + value);
            stageID = value;
        }
    }

    public int Score
    { 
        get { return score; }
        set
        {
            if(score == value) { return; }
            score = value;
            DebugMessenger.Log("Set new score" + score);
        }
    }

    public float ClearTime
    {
        get { return clearTime; }
        set
        {
            if (clearTime == value) { return; }
            clearTime = value;
            DebugMessenger.Log("Set new score" + clearTime);
        }
    }

    public int ComboMax
    { 
        get { return comboMax; }
        set
        {
            if (comboMax == value) { return; }
            DebugMessenger.Log("ComboMax Update : " + comboMax + " Å® " + value);
            comboMax = value;
        }
    }

}
