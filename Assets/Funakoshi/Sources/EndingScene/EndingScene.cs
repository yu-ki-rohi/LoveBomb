using UnityEngine;

public class EndingScene : MonoBehaviour
{
    [SerializeField] ContentManagement contentManagement;

    private readonly EndingSceneBGM bgm = new();
    private readonly ContentsRunner runner = new();

    void Start()
    {
        GameResult result = ResultDataHub.Pop();

        bgm.PlayFor(result);

        runner.SetAndRun(contentManagement);
    }
    void Update()
    {
        runner.Update();
    }
}
