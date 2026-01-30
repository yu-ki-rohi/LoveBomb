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
        int score, bonus = 0;
        score = gameState.Score;
        if(score > 0)
        {
            bonus = Mathf.Max((int)(gameState.ClearTime * 1000.0f), 0);
        }

        // ÉVÅ[ÉìÇå◊Ç¢Ç≈Ç´ÇΩílÇÇ±Ç±Ç≈ë„ì¸ÇµÇ‹Ç∑
        clearScore.InitalSetValue(score, Color.white);
        timeBonus.InitalSetValue(bonus, Color.white);
        finalResult.InitalSetValue(score + bonus, Color.white);

        ranking.GetRanking(gameState.StageID);
        ranking.SetRanking(score + bonus, gameState.StageID);

        int[] rankingValue = ranking.RankingValue;

        int length = Mathf.Min(rankingValue.Length, rankingText.Count);

        for(int i = 0; i < length; i++)
        {
            Color color = Color.white;
            if(score > 0 && rankingValue[i]== score + bonus)
            {
                color = Color.yellow;
            }
            rankingText[i].InitalSetValue(rankingValue[i], color);
            DebugMessenger.Log(rankingValue[i].ToString());
        }
    }
}
