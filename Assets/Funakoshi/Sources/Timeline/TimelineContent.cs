using UnityEngine;

public abstract class TimelineContent : MonoBehaviour
{
    [Header("これをアタッチするオブジェクトはTimelineAssetから生成してください")]

    [HideInInspector]
    public float ActionStartTime;

    public abstract float Duration();

    public void Animation(float globalTime)
    {
        if (globalTime < ActionStartTime) // durationの前
        {
            BeforeBegin();
        }
        else if (globalTime < ActionStartTime + Duration()) // localTimeがdurationの範囲に収まっている時
        {
            float localTime = globalTime - ActionStartTime;

            Evalute(localTime);
        }
        else // durationの後
        {
            AfterFinished();
        }
    }

    protected abstract void BeforeBegin();
    protected abstract void Evalute(float localTime);
    protected abstract void AfterFinished();
}
