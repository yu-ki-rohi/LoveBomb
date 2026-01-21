public class CurrentScene
{
    private Scene currentScene;

    public void Is(Scene scene)
    {
        if (currentScene != null)
            throw new ValueAlreadySetException("Scene‚ÍŠù‚Éİ’è‚³‚ê‚Ä‚¢‚Ü‚·");

        currentScene = scene;
    }

    public void SetUp()
    {
        if (currentScene == null)
            throw new System.NullReferenceException("scene‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");

        currentScene.SetUp();
    }

    public void Update()
    {
        if (currentScene == null)
            throw new System.NullReferenceException("scene‚ªw’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");

        currentScene.Update();
    }
}
