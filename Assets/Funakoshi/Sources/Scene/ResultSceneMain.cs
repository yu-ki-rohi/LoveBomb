using UnityEngine;

public class ResultSceneMain : Main
{
    protected override void Awake()
    {
        currentScene.Is(new ResultScene());
    }
}