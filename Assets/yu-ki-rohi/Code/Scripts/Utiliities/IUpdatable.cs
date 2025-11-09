
public interface IUpdatable
{
    public void Start();
    public void Update(float deltaTime);
    public void FixedUpdate(float fixedDeltaTime);

    public void OnEnable();

    public void OnDisable();
}
