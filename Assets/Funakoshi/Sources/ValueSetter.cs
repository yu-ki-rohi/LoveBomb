using System.Collections.Generic;
using UnityEngine;

public class ValueSetter : MonoBehaviour
{
    [SerializeField] private NumberTextComponent clearScore;
    [SerializeField] private NumberTextComponent timeBonus;
    [SerializeField] private NumberTextComponent finalResult;
    [SerializeField] private GameState gameState;

    [SerializeField] private List<NumberTextComponent> rankingText;
    private Ranking ranking = new Ranking();

    void Start()
    {
        int score, bonus;
        score = gameState.Score;
        bonus = Mathf.Max((int)(gameState.ClearTime * 1000.0f), 0);

        // ƒV[ƒ“‚ğŒ×‚¢‚Å‚«‚½’l‚ğ‚±‚±‚Å‘ã“ü‚µ‚Ü‚·
        clearScore.InitalSetValue(score);
        timeBonus.InitalSetValue(bonus);
        finalResult.InitalSetValue(score + bonus);

        ranking.GetRanking(gameState.StageID);
        ranking.SetRanking(score + bonus, gameState.StageID);

        int[] rankingValue = ranking.RankingValue;

        int length = Mathf.Min(rankingValue.Length, rankingText.Count);

        for(int i = 0; i < length; i++)
        {
            rankingText[i].InitalSetValue(rankingValue[i]);
        }
    }
}
