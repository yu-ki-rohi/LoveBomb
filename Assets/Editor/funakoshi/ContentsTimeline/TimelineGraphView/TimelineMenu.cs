using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    public record MenuOparation(TimelineGraphView Outer)
    {
        private readonly TimelineGameObjectGenerator unity = new();

        public void AddSerifNode(ContextualMenuPopulateEvent evt)
        {
            TimelineContent content = unity.CreateGameObject(TimelineNodeType.Serif);

            Vector2 nodePos = evt.localMousePosition;

            Outer.createNew.Node(nodePos, content);
        }
    }
}