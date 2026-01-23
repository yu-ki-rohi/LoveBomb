public class EndingSceneBGM
{
    public void PlayFor(GameResult result)
    {
        BGMName bgmName = result switch
        {
            ClearResult => BGMName.Succeed,
            FailedResult => BGMName.Failed,
            _ => throw new System.NotImplementedException($"No BGM assigned for the resultType '{result}'.")
        };

        if (!string.IsNullOrEmpty(bgmName.ToString()))
        {
            AudioManager.Instance.PlayBGMIfNotPlaying(bgmName); // BGMÇçƒê∂
        }
    }
}
