public class ResultScene : Scene
{
    public override void SetUp()
    {
        GameResult result = ResultDataHub.Pop();

        switch (result)
        {
            case ClearResult:
                break;
            default:
                throw new System.NotImplementedException(
                    $"{result}‚É‘Î‚·‚écase‚ª‹Lq‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
        }
    }
    public override void Update()
    {

    }
}
