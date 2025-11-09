using UnityEngine;

public class ClickContent : CoroutineContent
{
    public override void ForcedEnd()
    {
        contentEnd = true;
    }
    public override void ProcessStarted() { }
}
