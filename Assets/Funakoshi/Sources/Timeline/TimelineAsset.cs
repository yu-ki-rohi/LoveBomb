using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeline", menuName = "Timeline/Timeline Asset")]
public class TimelineAsset : ScriptableObject
{
    public readonly List<TimelineContent> Contents = new();

    public void MakeFrame(float time)
    {
        foreach (var content in Contents)
        {
            content.Animation(time);
        }
    }
}
