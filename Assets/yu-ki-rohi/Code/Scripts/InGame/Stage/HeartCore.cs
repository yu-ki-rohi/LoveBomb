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

    private IScoreFluctuate scoreFluctuate;

    public IScoreFluctuate ScoreFluctuate { set { scoreFluctuate = value; } }

    public TextMeshProUGUI EnemyNumText { set { enemyNumText = value; } }
    

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
    }

    public void AddEnemyScore(int score)
    {
        scoreFluctuate?.ReduceScore(score);
    }

    void Start()
    {
        enemyNumText.text = enemyCount.ToString();
        light2d = GetComponent<Light2D>();

    }

   
}
