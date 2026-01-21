public class ResultScene : Scene
{
    public override void SetUp()
    {
        GameResult result = ResultDataHub.Pop();
    }
    public override void Update()
    {

    }
}
