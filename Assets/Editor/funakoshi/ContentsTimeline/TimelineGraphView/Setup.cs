using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

public partial class TimelineGraphView
{
    /// <summary>
    /// TimelineGraphViewの初期設定を記述します
    /// </summary>
    public record Setup(TimelineGraphView Outer)
    {
        public Setup EnableMouseAction()
        {
            Outer.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); // ズーム
            Outer.AddManipulator(new ContentDragger()); // ドラッグ
            //Outer.AddManipulator(new RectangleSelector()); // ドラッグで範囲選択
            Outer.AddManipulator(new SelectionDragger()); // 複数選択したノードを一括で移動

            return this;
        }
        public Setup RightClickToMenu()
        {
            Outer.RegisterCallback<ContextualMenuPopulateEvent>(Outer.BuildContextualMenu);

            return this;
        }
        public Setup NodeSelectToInspector()
        {
            Outer.RegisterCallback<PointerDownEvent>(evt => Outer.OnMouseDown());

            return this;
        }
        public Setup BackgroundColor()
        {
            var almostBlack = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
            Outer.style.backgroundColor = almostBlack;

            return this;
        }
        public Setup GridForBackground()
        {
            var grid = new GridBackground();
            grid.StretchToParentSize();
            Outer.Insert(0, grid);

            return this;
        }
    }
}
