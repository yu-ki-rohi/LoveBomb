using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    public record MenuOparation(TimelineGraphView Outer)
    {
        private readonly TimelineContentFactory unity = new();

        public void AddSerifNode(ContextualMenuPopulateEvent evt)
        {
            TimelineContent content = unity.CreateGameObject(TimelineNodeType.Serif);

            Outer.createNew.Node(evt.localMousePosition, content);
        }
    }
}