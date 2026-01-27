using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class TimelineNode : Node
{
    // 定数
    private const float NODE_HEIGHT = 140f;
    private const float MIN_WIDTH = 50f;
    private const float WIDTH_PER_SECOND = 100f;

    // メンバー
    private readonly TimelineNode.NodeSize nodeSize;
    private readonly DurationField actionDuration;

    // 公開
    public float ActionDuration
    {
        get => actionDuration.GetValue();
        set => actionDuration.SetValue(value);
    }
    public Vector2 InitialNodeSize => new(100, NODE_HEIGHT);
    public float Width => GetPosition().width;

    // コンストラクタ
    public TimelineNode(string title)
    {
        // メンバー初期化
        nodeSize = new NodeSize(this);
        actionDuration = new DurationField();

        new NodeSetup(this)
            .Title(title)
            .NodeContents();

        // イベント登録
        SpecifyUpdateCallTiming();

        UpdateNode();
    }

    private void SpecifyUpdateCallTiming()
    {
        // 値が書き換えられた時
        actionDuration.OnValueChanged += UpdateNode;

    }

    public void UpdateNode()
    {
        nodeSize.Update();

    }

    public void SetPosition(Vector2 position)
    {
        var rect = GetPosition();

        rect.position = position;

        SetPosition(rect);
    }

    public record NodeSetup(TimelineNode Outer)
    {
        public NodeSetup Title(string title)
        {
            Outer.title = title;
            return this;
        }
        public NodeSetup NodeContents()
        {
            Outer.extensionContainer.Add(Outer.actionDuration);
            return this;
        }
    }

    public record NodeSize(TimelineNode Outer)
    {
        public void Update()
        {
            // 1秒 = 100pxとして幅を設定
            var newWidth = Mathf.Max(MIN_WIDTH, Outer.ActionDuration * WIDTH_PER_SECOND);

            SetSize(newWidth, NODE_HEIGHT);
        }

        public void SetSize(float width, float height)
        {
            var pos = Outer.GetPosition();

            pos.width = width;
            pos.height = height;

            Outer.SetPosition(pos);
        }
    }

    public Port CreateInputPort()
    {
        var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        port.portName = "In";
        inputContainer.Add(port);
        return port;
    }
    public Port CreateOutputPort()
    {
        var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        port.portName = "Out";
        outputContainer.Add(port);
        return port;
    }
}
