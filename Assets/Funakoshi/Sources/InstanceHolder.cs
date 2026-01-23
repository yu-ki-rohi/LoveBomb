public class InstanceHolder<T> where T : class
{
    private T instance = null;

    public void SetInstance(T instance)
    {
        if (IsAlreadySet())
            throw new ValueAlreadySetException($"instanceHolderには既に{this.instance}が設定されています。新しく{instance}を設定するには、先にRemove()を実行してください。");

        this.instance = instance;
    }

    public void Remove()
    {
        instance = null;
    }

    public bool IsNotSet() => instance == null;
    public bool IsAlreadySet() => instance != null;
    public T Current() => instance;
}
