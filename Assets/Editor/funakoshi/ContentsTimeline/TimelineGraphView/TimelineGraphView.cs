using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public partial class TimelineGraphView : GraphView
{
    private readonly TimelineGraphView.CreateElement createNew;
    private readonly TimelineGraphView.MenuOparation menuOparation;

    public TimelineGraphView()
    {
        // メンバーの初期化
        createNew = new CreateElement(this);
        menuOparation = new MenuOparation(this);

        // セットアップ
        new Setup(this)
            .EnableMouseAction()
            .RightClickToMenu()
            .BackgroundColor()
            .GridForBackground();

        // イベント登録
        graphViewChanged += OnGraphViewChanged;
    }

    public void PlaceTheSample()
    {
        // サンプルノードを追加
        createNew.Node("Start", new(100, 100));

        // もっと追加
        createNew.Node("Event1", new(300, 100));
        createNew.Node("Event2", new(400, 100));
        createNew.Node("Event3", new(600, 100));
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {

        return change;
    }

    /// <summary>
    /// 右クリックメニューの内容を設定します
    /// </summary>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Add TimelineNode", (a) => { menuOparation.AddNode(evt); });
        
        evt.menu.AppendSeparator(); // 区切り線

        evt.menu.AppendAction("Auto Arrange Nodes", (a) => { menuOparation.AutoArrangeNodes(); });
    }
}
