using UnityEngine;

public abstract class CoroutineContent : MonoBehaviour
{
    protected bool contentEnd = false;

    public bool IsContentEnd()
    {
        return contentEnd;
    }
    public abstract void ProcessStarted();
    public abstract void ForcedEnd();
}
