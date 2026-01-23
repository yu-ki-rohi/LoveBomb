using UnityEngine;

public class EndingScene : MonoBehaviour
{
    [SerializeField] ContentManagement contentManagement;

    private readonly EndingSceneBGM bgm = new();

    void Start()
    {
        GameResult result = ResultDataHub.Pop();

        bgm.PlayFor(result);

        contentManagement.RunFirstContent();
    }
    void Update()
    {
        if (!contentManagement.IsAllContentEnd())
        {
            contentManagement.ContentUpdate();
        }

        PressMouseToSkip();
    }
    void PressMouseToSkip()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!contentManagement.IsAllContentEnd())
            {
                contentManagement.SkipContent();
            }
        }
    }
}
