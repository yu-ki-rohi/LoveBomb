public record Value
{
    protected float value;

    public Value(float value)
    {
        this.value = value;
    }

    public float Get() => value;
}
