using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public partial class TimelineGraphView : GraphView
{
    private readonly TimelineGraphView.CreateElement createNew;
    private readonly TimelineGraphView.MenuOparation menuOparation;
    private readonly TimelineGraphView.TimelineLane timelineLane;
    private readonly TimelineGraphView.NodeSelect select;

    public TimelineGraphView()
    {
        // メンバーの初期化
        createNew = new CreateElement(this);
        menuOparation = new MenuOparation(this);
        timelineLane = new TimelineLane(this);
        select = new NodeSelect(this);

        // セットアップ
        new Setup(this)
            .EnableMouseAction()
            .RightClickToMenu()
            .NodeSelectToInspector()
            .BackgroundColor()
            .GridForBackground();

        timelineLane.Setup();

        // イベント登録
        graphViewChanged += OnGraphViewChanged;
    }

    public void Load(List<TimelineContent> loadData)
    {
        foreach (var content in loadData)
        {
            createNew.Node(content);
        }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {

        return change;
    }
    public void OnMouseDown()
    {
        // TimelineNodeが選択された時、TimelineNodeに紐づくオブジェクトをインスペクターで表示します
        if (select.IfSelected<TimelineNode>(out var selectedNode))
        {
            select.ViewInTheInspector(selectedNode.Content.gameObject);
        }
    }

    /// <summary>
    /// 右クリックメニューの内容を設定します
    /// </summary>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Add SerifNode", (a) => { menuOparation.AddSerifNode(evt); });

        evt.menu.AppendAction("---", (a) => { });

        evt.menu.AppendAction("--未実装です--", (a) => { });
    }
}
