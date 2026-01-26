using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HeartCore : MonoBehaviour
{
    private int enemyCount = 0;


    private Light2D light2d;

    [SerializeField] private TextMeshProUGUI enemyNumText;
    [Min(0.01f), SerializeField] private float minLightRadius = 0.1f;
    [Min(10.0f), SerializeField] private float maxLightRadius = 100.0f;
    [Min(2.0f), SerializeField] private float lightOuterRadiusMaltiplier = 10.0f;

    private IScoreFluctuate scoreFluctuate;

    public IScoreFluctuate ScoreFluctuate { set { scoreFluctuate = value; } }
    

    public void AddEnemyCount()
    {
        enemyCount++;
        enemyNumText.text = enemyCount.ToString();
    }

    public void ReduceEnemyCount()
    {
        enemyCount--;
        enemyNumText.text = enemyCount.ToString();
    }

    public void AddPlayerScore(int score)
    {
        scoreFluctuate?.AddScore(score);
        ReflectLight();
    }

    public void AddEnemyScore(int score)
    {
        scoreFluctuate?.ReduceScore(score);
        ReflectLight();
    }

    void Start()
    {
        enemyNumText.text = enemyCount.ToString();
        light2d = GetComponent<Light2D>();
        ReflectLight();

    }

   
    private void ReflectLight()
    {
        //float ratio = Mathf.Clamp01((maxSub + sub) / (maxSub * 2.0f));

        //light2d.pointLightInnerRadius = (maxLightRadius - minLightRadius) * ratio + minLightRadius;
        //light2d.pointLightOuterRadius = light2d.pointLightInnerRadius * lightOuterRadiusMaltiplier;
    }
}
