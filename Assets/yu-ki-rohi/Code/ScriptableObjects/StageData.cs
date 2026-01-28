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

[Serializable]
public class LightInfomation
{
    [Min(0.01f)]
    public float minLightRadius = 0.1f;

    [Min(10.0f)] 
    public float maxLightRadius = 100.0f;

    [Min(2.0f)]
    public float lightOuterRadiusMaltiplier = 10.0f;

    public Color minGrobalLight;
    public Color maxGrobalLight;

    [Range(0f, 1f)]
    public float playerLightBorder = 0.7f;
}

[CreateAssetMenu(fileName = "StageData[n]", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    public StageManager StageManager;

    public TimeInfomation TimeInfomation;

    public ScoreInfomation ScoreInfomation;

    public LightInfomation LightInfomation;
}
