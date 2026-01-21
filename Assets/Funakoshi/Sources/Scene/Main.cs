using UnityEngine;

public abstract class Main : MonoBehaviour
{
    protected readonly CurrentScene currentScene = new();

    // Awake‚ÅcurrentScene‚ğİ’è‚µ‚Ü‚·
    protected abstract void Awake();

    void Start()
    {
        Application.targetFrameRate = 60;

        currentScene.SetUp();
    }
    void Update()
    {
        currentScene.Update();
    }
}