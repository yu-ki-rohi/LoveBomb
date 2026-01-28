using UnityEngine;

public class TWaitContent : TimelineContent
{
    [SerializeField] float waitSecond;

    public override float Duration()
    {
        return waitSecond;
    }

    protected override void BeforeBegin() { }
    protected override void Evalute(float localTime) { }
    protected override void AfterFinished() { }
}
