using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartCore : MonoBehaviour
{
    private int enemyCount = 0;

    // ‰¼‚Å“ü‚ê‚Ä‚¢‚é‚¾‚¯
    private int playerScore = 0;
    private int enemyScore = 0;

    [SerializeField] private TextMeshProUGUI enemyNumText;
    [SerializeField] private Image playerScoreUI;
    [SerializeField] private Image enemyScoreUI;
    [Min(50), SerializeField] private int maxSub = 1000;

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
    }

    public void AddEnemyScore(int score)
    {
        enemyScore += score;
        ReflectUI();
    }

    void Start()
    {
        enemyNumText.text = enemyCount.ToString();
    }

    private void ReflectUI()
    {
        int sub = playerScore - enemyScore;
        float ratio = Mathf.Clamp01((float)(maxSub + sub) / (maxSub * 2.0f));
        playerScoreUI.fillAmount = ratio;
        enemyScoreUI.fillAmount = 1.0f - ratio;
    }
}
