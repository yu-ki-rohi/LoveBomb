using UnityEditor;
using UnityEngine.UIElements;

public class TimelineWindow : EditorWindow
{
    [MenuItem("Window/Scene Timeline")]
    public static void ShowWindow()
    {
        GetWindow<TimelineWindow>("Scene Timeline"); 
    }

    void CreateGUI()
    {
        var timeline = CreateTimeline();

        timeline.StretchToParentSize();

        timeline.PlaceTheSample();
    }

    public TimelineGraphView CreateTimeline()
    {
        var timeline = new TimelineGraphView();

        rootVisualElement.Add(timeline);

        return timeline;
    }
}
