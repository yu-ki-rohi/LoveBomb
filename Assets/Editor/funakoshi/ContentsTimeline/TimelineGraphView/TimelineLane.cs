using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    private readonly DragTracker<TimelineNode> dragTracker = new();

    public record TimelineLane(TimelineGraphView Outer)
    {
        public void Setup()
        {
            Outer.RegisterCallback<PointerDownEvent>(evt => Outer.dragTracker.OnPointerDown(evt));
            Outer.RegisterCallback<PointerMoveEvent>(evt => Outer.dragTracker.OnPointerMove(evt));
            Outer.RegisterCallback<PointerUpEvent>(evt => Outer.dragTracker.OnPointerUp());
            Outer.dragTracker.OnDragStarted += OnNodeDragStarted;
            Outer.dragTracker.OnDragEnded += OnNodeDragEnded;
            Outer.dragTracker.WhileDragging += WhileNodeDragging;
        }

        public void OnNodeDragStarted(TimelineNode node)
        {
            // ノードを薄くする
            node.style.opacity = 0.4f;
        }
        public void WhileNodeDragging(TimelineNode node, PointerMoveEvent evt)
        {
            AutoArrangeNodes();
        }
        public void OnNodeDragEnded(TimelineNode node)
        {
            // ノードの透明度を元に戻す
            node.style.opacity = 1.0f;

            AutoArrangeNodes();
        }

        private const float STRETCH_TO_LEFT = 100f;
        private const float STRETCH_TO_TOP = 100f;

        /// <summary>
        /// 横軸上でノードを整列します
        /// </summary>
        public void AutoArrangeNodes()
        {
            var sortedNodeList = Outer.graphElements
                .OfType<TimelineNode>()
                .OrderBy(node => node.GetPosition().x)
                .ToList();

            Vector2 nodeAtThisPosition = new(STRETCH_TO_LEFT, STRETCH_TO_TOP);

            foreach (var node in sortedNodeList)
            {
                node.SetPosition(nodeAtThisPosition);

                nodeAtThisPosition += Vector2.right * node.Width;
            }

            Debug.Log($"タイムラインを整列しました : 計 {sortedNodeList.Count} ノード");
        }

        public static Vector2 CalculatePosition(TimelineContent content)
        {
            float offsetX = content.ActionStartTime * TimelineNode.WIDTH_PER_SECOND;

            return new Vector2(STRETCH_TO_LEFT, STRETCH_TO_TOP) + Vector2.right * offsetX;
        }
        public static float CalculateNodeTime(Vector2 position)
        {
            float localPosX = position.x - STRETCH_TO_LEFT;

            return localPosX / TimelineNode.WIDTH_PER_SECOND;
        }
    }
}
