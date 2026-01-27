using UnityEditor.Experimental.GraphView;

using UnityEngine;

public partial class TimelineGraphView
{
    public record CreateElement(TimelineGraphView Outer)
    {
        public TimelineNode Node(TimelineContent content)
        {
            Vector2 nodePosition = TimelineLane.CalculatePosition(content);

            return Node(nodePosition, content);
        }

        public TimelineNode Node(Vector2 nodePosition, TimelineContent content)
        {
            string nodeName = content.gameObject.name;

            return Node(nodeName, nodePosition, content);
        }

        public TimelineNode Node(string nodeName, Vector2 nodePosition, TimelineContent content)
        {
            // ÉmÅ[ÉhÇê∂ê¨
            TimelineNode node = new(nodeName, content);

            // à íuÇê›íË
            node.SetPosition(new Rect(nodePosition, node.InitialNodeSize));

            // GraphViewÇ…ÉmÅ[ÉhÇìoò^
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
