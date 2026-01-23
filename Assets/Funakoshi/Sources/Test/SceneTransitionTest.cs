using UnityEngine;

public class SceneTransitionTest : MonoBehaviour
{
    public void ResultSceneTest()
    {
        var result = new ClearResult
        (
            Score : 0,
            KillCount : 0
        );

        SceneTransition.ToResultScene(result);
    }
}
