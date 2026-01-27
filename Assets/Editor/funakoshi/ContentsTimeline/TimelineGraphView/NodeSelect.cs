using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine;

public partial class TimelineGraphView
{
    public record NodeSelect(TimelineGraphView Outer)
    {
        public bool IfSelected<TElement>(out TElement selectedElement)
        {
            selectedElement = Outer.selection.OfType<TElement>().FirstOrDefault();

            return selectedElement != null;
        }
        public void ViewInTheInspector(Object viewData)
        {
            Selection.activeObject = viewData;
        }
    }
}
