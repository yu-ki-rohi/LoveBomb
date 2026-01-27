using Unity.VisualScripting;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    private readonly DragTracker<TimelineNode> dragTracker = new();

    public record TimelineLane(TimelineGraphView Outer)
    {
        public void Setup()
        {
            Outer.RegisterCallback<PointerDownEvent>(evt => Outer.dragTracker.OnPointerDown(evt));
            Outer.RegisterCallback<PointerUpEvent>(evt => Outer.dragTracker.OnPointerUp());
        }
        public void OnDragStarted(TimelineNode node)
        {
            // ノードを薄くする
            node.style.opacity = 0.4f;
        }
        public void OnDragEnded(TimelineNode node)
        {
            // ノードの透明度を元に戻す
            node.style.opacity = 1.0f;
        }
    }
}
