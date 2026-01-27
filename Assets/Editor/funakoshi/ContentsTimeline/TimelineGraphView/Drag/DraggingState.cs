using UnityEngine.UIElements;

public class DraggingState<TElement> where TElement : VisualElement
{
    private bool somethingDragging = false;
    private TElement target;

    public void StartDragging(TElement target)
    {
        somethingDragging = true;

        this.target = target;
    }
    public void EndDragging()
    {
        somethingDragging = false;

        target = null;
    }

    public bool NowSomethingDragging(out TElement target)
    {
        target = this.target;
        return somethingDragging;
    }

    public bool NowSomethingDragging() => somethingDragging;
    public bool NotYet() => somethingDragging == false;
}
