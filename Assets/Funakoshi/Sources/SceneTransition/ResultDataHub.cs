public static class ResultDataHub
{
    private static readonly TemporaryStorage<GameResult> storage = new();

    public static void HoldData(GameResult result)
    {
        if (storage.IsAlreadySet())
        {
            throw new ValueAlreadySetException(
                "GameResultは既に保持されています。" +
                "この状態で新しいデータを保持することはできません。");
        }

        storage.SetData(result);
    }
    public static GameResult Pop()
    {
        if (storage.IsNull())
        {
            throw new System.NullReferenceException(
                "GameResultが保持されていません。" +
                "既に取り出されているか、または保持し忘れていないかどうか確認してください。");
        }

        var retval = storage.CurrentData();

        storage.Exclude();

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
