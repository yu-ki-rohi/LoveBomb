using UnityEditor.Experimental.GraphView;

using UnityEngine;

public partial class TimelineGraphView
{
    public record CreateElement(TimelineGraphView Outer)
    {
        public TimelineNode Node(string nodeName, Vector2 nodePosition)
        {
            // ƒm[ƒh‚ğ¶¬
            TimelineNode node = new(nodeName);

            // ˆÊ’u‚ğİ’è
            node.SetPosition(new Rect(nodePosition, node.InitialNodeSize));

            // GraphView‚Éƒm[ƒh‚ğ“o˜^
            Outer.AddElement(node);

            return node;
        }

        public Edge Edge(Port port01, Port port02)
        {
            var edge = port01.ConnectTo(port02);

            Outer.AddElement(edge);

            return edge;
        }
    }
}
