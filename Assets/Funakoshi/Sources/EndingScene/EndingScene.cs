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

        // TODO : インゲーム内の最後の時間を取得
        // TODO : スコアを取得
        // TODO : スコアのクリア水準を取得

        // TODO : どのTimelineAssetを流すか判定

        runner.SetAndRun(contentManagement);
        // TODO : SetAndRunの引数をTimelineAssetに置き換える
    }
    void Update()
    {
        runner.Update();
    }
}
