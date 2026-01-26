using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    public record MenuOparation(TimelineGraphView Outer)
    {
        public void AddNode(ContextualMenuPopulateEvent evt)
        {
            var mousePos = evt.localMousePosition;
            Outer.createNew.Node("Timeline Node", mousePos);
        }

        private const float STRETCH_TO_LEFT = 100f;
        private const float STRETCH_TO_TOP = 100f;

        /// <summary>
        /// 横軸上でノードを整列します
        /// </summary>
        public void AutoArrangeNodes()
        {
            var sortedNodeList = Outer.graphElements
                .OfType<TimelineNode>()
                .OrderBy(node => node.GetPosition().x)
                .ToList();

            if (sortedNodeList.Count == 0) return;

            Vector2 nodeAtThisPosition = new(STRETCH_TO_LEFT, STRETCH_TO_TOP);

            foreach (var node in sortedNodeList)
            {
                node.SetPosition(nodeAtThisPosition);

                nodeAtThisPosition += Vector2.right * node.Width;
            }

            Debug.Log($"タイムラインを整列しました : 計 {sortedNodeList.Count} ノード");
        }
    }
}