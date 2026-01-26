using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView : GraphView
{
    private readonly CreateElement createNew;
    private readonly MenuOparation menuOparation;

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
        var node01 = createNew.Node("Start", new Vector2(100, 100));

        // もう一つ追加
        var node02 = createNew.Node("Event1", new Vector2(300, 100));

        // 接続
        createNew.Edge(node01.OutputPort, node02.InputPort);
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
