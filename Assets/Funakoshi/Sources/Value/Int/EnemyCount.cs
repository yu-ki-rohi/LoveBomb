public record EnemyCount : Count
{
    public EnemyCount(int count) : base(count) { }

    public EnemyCount Add(EnemyCount other)
    {
        int result = count + other.count;
        return new EnemyCount(result);
    }
}