using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TimelineWindow : EditorWindow
{
    [MenuItem("Window/Scene Timeline")]
    public static void ShowWindow()
    {
        GetWindow<TimelineWindow>("Scene Timeline"); 
    }

    void CreateGUI()
    {
        var graphView = new TimelineGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);

        // サンプルノードを追加
        var startNode = CreateNewNode("Start", new Vector2(100, 100));
        graphView.AddElement(startNode);

        // もう一つ追加
        var eventNode = CreateNewNode("Event1", new Vector2(300, 100));
        graphView.AddElement(eventNode);

        // 接続
        var outputPort = startNode.outputContainer.Q<Port>("Out");
        var inputPort = eventNode.inputContainer.Q<Port>("In");

        if (outputPort != null && inputPort != null)
        {
            var edge = outputPort.ConnectTo(inputPort);
            graphView.AddElement(edge);
        }
    }

    private Node CreateNewNode(string nodeName, Vector2 nodePosition)
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
