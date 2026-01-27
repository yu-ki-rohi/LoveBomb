using System;
using UnityEngine.UIElements;

public class DragTracker<TElement> where TElement : VisualElement
{
    private readonly DraggingState<TElement> dragging;

    public event Action<TElement> OnDragStarted;
    public event Action<TElement> OnDragEnded;

    public bool NowDragging(out TElement target) => dragging.NowSomethingDragging(out target);

    public void OnPointerDown(PointerDownEvent evt)
    {
        var click = new Click(evt);

        if (click.IsLeftClick() && click.PickedSomething() && dragging.NotYet())
        {
            // ƒNƒŠƒbƒN‚³‚ê‚½‘ÎÛ‚ªTElement‚©‚Ç‚¤‚©‚ð”»’è‚µ‚Ü‚·
            var picked = click.TargetElement();

            if (picked is TElement node)
            {
                dragging.StartDragging(node);

                OnDragStarted?.Invoke(node);
            }
        }
    }
    public void OnPointerUp()
    {
        if (dragging.NowSomethingDragging(out TElement draggedTarget))
        {
            dragging.EndDragging();

            OnDragEnded?.Invoke(draggedTarget);
        }
    }

    public record Click(PointerDownEvent Evt)
    {
        public bool IsLeftClick() => Evt.button == 0;

        public bool PickedSomething() => Evt.target is VisualElement;

        public VisualElement TargetElement() => Evt.target as VisualElement;
    }
}
