using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HeartCore : MonoBehaviour
{
    private int enemyCount = 0;

    // ‰¼‚Å“ü‚ê‚Ä‚¢‚é‚¾‚¯
    private int playerScore = 0;
    private int enemyScore = 0;

    private Light2D light2d;

    [SerializeField] private TextMeshProUGUI enemyNumText;
    [SerializeField] private Image playerScoreUI;
    [SerializeField] private Image enemyScoreUI;
    [Min(50), SerializeField] private int maxSub = 1000;
    [Min(0.01f), SerializeField] private float minLightRadius = 0.1f;
    [Min(10.0f), SerializeField] private float maxLightRadius = 100.0f;
    [Min(2.0f), SerializeField] private float lightOuterRadiusMaltiplier = 10.0f;

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
        playerScore += score;
        ReflectUI();
        ReflectLight();
    }

    public void AddEnemyScore(int score)
    {
        enemyScore += score;
        ReflectUI();
        ReflectLight();
    }

    void Start()
    {
        enemyNumText.text = enemyCount.ToString();
        enemyScore = (int)(maxSub * 0.7f);
        light2d = GetComponent<Light2D>();
        ReflectUI();
        ReflectLight();

    }

    private void ReflectUI()
    {
        int sub = playerScore - enemyScore;
        float ratio = Mathf.Clamp01((maxSub + sub) / (maxSub * 2.0f));
        playerScoreUI.fillAmount = ratio;
        enemyScoreUI.fillAmount = 1.0f - ratio;
    }

    private void ReflectLight()
    {
        int sub = playerScore - enemyScore;
        float ratio = Mathf.Clamp01((maxSub + sub) / (maxSub * 2.0f));

        light2d.pointLightInnerRadius = (maxLightRadius - minLightRadius) * ratio + minLightRadius;
        light2d.pointLightOuterRadius = light2d.pointLightInnerRadius * lightOuterRadiusMaltiplier;
    }
}
