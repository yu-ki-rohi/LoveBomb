using UnityEngine;

public abstract class TimelineContent : MonoBehaviour
{
    public float ActionStartTime;

    public abstract float Duration();

    public void Animation(float globalTime)
    {
        if (globalTime < ActionStartTime) // localTime‚ª¬‚³‚¢Žž
        {
            BeforeBegin();
        }
        else if (Duration() + ActionStartTime < globalTime) // localTime‚ª‘å‚«‚¢Žž
        {
            AfterFinished();
        }
        else // localTime‚ªduration‚Ì”ÍˆÍ‚ÉŽû‚Ü‚Á‚Ä‚¢‚éŽž
        {
            float localTime = globalTime - ActionStartTime;

            Evalute(localTime);
        }
    }

    protected abstract void BeforeBegin();
    protected abstract void Evalute(float localTime);
    protected abstract void AfterFinished();
}
