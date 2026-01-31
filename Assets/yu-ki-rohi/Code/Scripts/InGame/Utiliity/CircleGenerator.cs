using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CircleGenerator : GeneratorBase
{
    [SerializeField, HideInInspector]
    private float innerRadius = 5.0f;
    [SerializeField, HideInInspector]
    private float outerRadius = 8.0f;

    public float InnerRadius { get { return innerRadius; } }
    public float OuterRadius { get { return outerRadius; } }


    #region Unity Editor
#if UNITY_EDITOR
    public void SetInnerRadius(float value)
    { 
        innerRadius = value;
    }

    public void SetOuterRadius(float value)
    {
        outerRadius = value;
    }
#endif
    #endregion

    public override Vector3 DecideGeneratePosition()
    {
        float theta = Random.Range(-Mathf.PI, Mathf.PI);
        float distance = Random.Range(innerRadius, outerRadius);
        float z = -1.0f;
        return transform.position + new Vector3(distance * Mathf.Sin(theta), distance * Mathf.Cos(theta), z);
    }

    #region Unity Editor
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showAlways) { return; }
        DrawGenerateField();
    }

    private void OnDrawGizmosSelected()
    {
        if (showAlways) { return; }
        DrawGenerateField();
    }

    private void DrawGenerateField()
    {
        Handles.color = fillColor;
        Handles.DrawSolidDisc(transform.position, -Vector3.forward, outerRadius);
        Handles.color = innerColor;
        Handles.DrawSolidDisc(transform.position, -Vector3.forward, innerRadius);
    }
#endif
    #endregion

}
