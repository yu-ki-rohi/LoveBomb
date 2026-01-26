public static class ResultDataHub
{
    private static readonly TemporaryStorage<GameResult> temporaryStorage = new();

    public static void HoldData(GameResult result)
    {
        if (temporaryStorage.IsAlreadySet())
        {
            throw new ValueAlreadySetException(
                "GameResultは既に保持されています。" +
                "この状態で新しいデータを保持することはできません。");
        }

        temporaryStorage.SetData(result);
    }
    public static GameResult Pop()
    {
        if (temporaryStorage.IsNull())
        {
            throw new System.NullReferenceException(
                "GameResultが保持されていません。" +
                "既に取り出されているか、または保持し忘れていないかどうか確認してください。");
        }

        var retval = temporaryStorage.CurrentData();

        temporaryStorage.Exclude();

        return retval;
    }
}

public class TemporaryStorage<T> where T : class
{
    private T data = null;

    public void SetData(T data)
    {
        this.data = data;
    }

    public bool IsNull() => data == null;
    public bool IsAlreadySet() => data != null;

    public T CurrentData() => data;

    public void Exclude()
    {
        data = null;
    }
}
