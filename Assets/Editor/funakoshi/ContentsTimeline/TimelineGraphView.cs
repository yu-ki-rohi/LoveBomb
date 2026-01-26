using UnityEditor.Experimental.GraphView;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TimelineGraphView : GraphView
{
    public TimelineGraphView()
    {
        // 機能を有効化
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMinScale); // ズーム
        this.AddManipulator(new ContentDragger()); // ドラッグ
        this.AddManipulator(new RectangleSelector()); // ドラッグで範囲選択
        this.AddManipulator(new SelectionDragger()); // 複数選択したノードを一括で移動
        RegisterCallback<ContextualMenuPopulateEvent>(BuildContextualMenu); // 右クリックでメニューを表示

        // 背景にグリッドを表示
        var grid = new GridBackground();
        grid.StretchToParentSize();
        Insert(0, grid);

        // 背景色の設定
        var almostBlack = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        style.backgroundColor = almostBlack;

        graphViewChanged += OnGraphViewChanged;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {

        return change;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        // ポートの接続方法を設定します
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&  // A出力 - B入力 の接続のみ許可
            endPort.node != startPort.node               // 自分自身に繋がない
        ).ToList();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Add TimelineNode", (a) =>
        {
            var mousePos = evt.localMousePosition;
            var node = CreateNewNode("Timeline Node", mousePos);
            AddElement(node);
        });
    }

    public Node CreateNewNode(string nodeName, Vector2 nodePosition)
    {
        // ノードを生成
        Node node = new TimelineNode(nodeName);

        // ノードの位置
        var rect = new Rect(nodePosition, new Vector2(200, 100));
        node.SetPosition(rect);

        // 入力ポートを生成
        var input = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        input.portName = "In";
        node.inputContainer.Add(input);

        // 出力ポートを生成
        var output = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        output.portName = "Out";
        node.outputContainer.Add(output);

        return node;
    }
}
