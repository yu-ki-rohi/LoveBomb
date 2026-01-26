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
        var startNode = graphView.CreateNewNode("Start", new Vector2(100, 100));
        graphView.AddElement(startNode);

        // もう一つ追加
        var eventNode = graphView.CreateNewNode("Event1", new Vector2(300, 100));
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
}
