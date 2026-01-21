public abstract class GameResult
{
    public bool IsClear { get; }
    protected GameResult(bool isClear)
    {
        IsClear = isClear;
    }
}

public class ClearResult : GameResult
{
    public ClearResult() : base(isClear: true)
    {

    }
}
public class FailedResult : GameResult
{
    public FailedResult() : base(isClear: false)
    {

    }
}
