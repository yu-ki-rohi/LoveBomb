using UnityEngine;

[CreateAssetMenu(fileName = "StageData[n]", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    public StageManager StageObject;
    public float GameTime = 90.0f;

    public int ScoreInitial = 1000;
    public int ScoreBorder = 3000;
    public int ScoreMax = 6000;

}
