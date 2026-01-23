using UnityEngine.SceneManagement;

public class SceneTransition
{
    public static void ToResultScene(GameResult result)
    {
        ResultDataHub.HoldData(result);

        SceneManagerWithFade.LoadScene("ResultScene");
    }
}
