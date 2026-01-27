using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// TODO : ノードの位置が変わるタイミングの処理を追加
// Content.ActionStartTime = TimelineLane.CalculateNodeTime(nodePosition)
public class TimelineNode : Node
{
    // 定数
    public const float NODE_HEIGHT = 140f;
    public const float MIN_WIDTH = 50f;
    public const float WIDTH_PER_SECOND = 100f;

    // メンバー
    private readonly TimelineNode.NodeSize nodeSize;
    private readonly TimelineContent content;

    // 公開
    public TimelineContent Content => content;
    public float ActionDuration => Content.Duration();
    public Vector2 InitialNodeSize => new(100, NODE_HEIGHT);
    public float Width => GetPosition().width;

    // コンストラクタ
    public TimelineNode(string title, TimelineContent content)
    {
        // メンバー初期化
        nodeSize = new NodeSize(this);
        this.content = content;

        // セットアップ
        new NodeSetup(this)
            .Title(title)
            .NodeContents();

        UpdateNode();
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
        public NodeSetup IgnoreLabel()
        {
            var titleLabel = Outer.Q<Label>("title-label");
            if (titleLabel != null)
            {
                titleLabel.pickingMode = PickingMode.Ignore;
            }
            return this;
        }
        public NodeSetup NodeContents()
        {
            return this;
        }
    }

    public record NodeSize(TimelineNode Outer)
    {
        public void Update()
        {
            // ActionDurationに基づいてノードの幅を設定
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
