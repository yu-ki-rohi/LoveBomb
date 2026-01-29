using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject
{
    [SerializeField] private StageDataBase stageDataBase;
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

            if(stageDataBase != null && 
               (value < 0 || value >= stageDataBase.Stages.Count)) 
            {
                DebugMessenger.LogError("Blocked access to out of array bounds. Access : " + value +" StageId : " + stageID);
                return;
            }

            DebugMessenger.Log("Change StageID : " + stageID + " Å® " + value);
            stageID = value;
        }
    }

    public int Score
    { 
        get { return score; }
        set
        {
            score = value;
            DebugMessenger.Log("Set new score: " + score);
        }
    }

    public float ClearTime
    {
        get { return clearTime; }
        set
        {
            clearTime = value;
            DebugMessenger.Log("Set Clear Time: " + clearTime);
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
