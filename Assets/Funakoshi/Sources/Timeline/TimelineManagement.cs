using UnityEngine;

public class TimelineManagement
{
    private TimelineAsset timeline;
    public TimelineManagement(TimelineAsset timeline)
    {
        this.timeline = timeline;
    }

    private float time;

    public void SetAndRun(TimelineAsset timeline)
    {
        this.timeline = timeline;

        time = 0f;
    }

    public void Update()
    {
        time += Time.deltaTime;

        timeline.MakeFrame(time);
    }

    public void SkipToNextStop()
    {
        throw new System.NotImplementedException();
    }
}
