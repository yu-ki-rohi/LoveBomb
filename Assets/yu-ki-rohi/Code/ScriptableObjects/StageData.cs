using System;
using UnityEngine;

[Serializable]
public class TimeInfomation
{
    public float GameTime = 90.0f;
}

[Serializable]
public class ScoreInfomation
{
    public int ScoreInitial = 1000;
    public int ScoreBorder = 3000;
    public int ScoreMax = 6000;
}


[CreateAssetMenu(fileName = "StageData[n]", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    public StageManager StageObject;

    public TimeInfomation TimeInfomation;

    public ScoreInfomation ScoreInfomation;

}
