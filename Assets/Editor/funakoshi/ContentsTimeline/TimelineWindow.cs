using UnityEditor;
using UnityEngine.UIElements;

public class TimelineWindow : EditorWindow
{
    public static void OpenWithAsset(TimelineAsset asset)
    {
        var window = GetWindow<TimelineWindow>("Timeline Editor - " + asset.name);

        window.Load(asset);
    }

    private void Load(TimelineAsset asset)
    {
        var timeline = CreateTimeline();

        timeline.StretchToParentSize();

        timeline.Load(asset.Contents);
    }

    public TimelineGraphView CreateTimeline()
    {
        var timeline = new TimelineGraphView();

        rootVisualElement.Add(timeline);

        return timeline;
    }
}
