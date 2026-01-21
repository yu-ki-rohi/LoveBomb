public class EnemyCountManager
{
    private EnemyCount currentCount = new(0);

    public void CountUp()
    {
        currentCount = currentCount.Add(new EnemyCount(1));
    }

    public void Reset()
    {
        currentCount = new EnemyCount(0);
    }

    public void SetCount(EnemyCount newValue)
    {
        currentCount = newValue;
    }

    public EnemyCount Current() => currentCount;
}
