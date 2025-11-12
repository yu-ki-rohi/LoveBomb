using UnityEditor;
using UnityEngine;

public class BoxGenerator : GeneratorBase
{

    [SerializeField, HideInInspector]
    private Vector2 minLength = new Vector2(5.0f, 5.0f);
    [SerializeField, HideInInspector]
    private Vector2 maxLength = new Vector2(8.0f, 8.0f);

    public Vector2 MinLength { get { return minLength; } }

    public Vector2 MaxLength { get { return maxLength; } }


    #region Unity Editor
#if UNITY_EDITOR
    private Color outlineColor = Color.red;      // エネミーが出現する範囲の色

    public void SetRangeX(float min, float max)
    {
        minLength.x = min;
        maxLength.x = max;
    }

    public void SetRangeY(float min, float max)
    {
        minLength.y = min;
        maxLength.y = max;
    }
#endif
    #endregion

    public override Vector3 DecideGeneratePosition()
    {
        // 方向を０～３の数字で決定
        // ０：上　１：右　２：下　３：左
        int decideAppearDir = Random.Range(0, 4);

        float z = -0.5f;
        Vector3 offset;
        switch(decideAppearDir)
        {
            case 0:
                offset =  new Vector3(Random.Range(-maxLength.x, maxLength.x), Random.Range(minLength.y, maxLength.y), z);
                break;
                
            case 1:
                offset = new Vector3(Random.Range(minLength.x, maxLength.x), Random.Range(-maxLength.y, maxLength.y), z);
                break;

            case 2:
                offset = new Vector3(Random.Range(-maxLength.x, maxLength.x), Random.Range(-minLength.y, -maxLength.y), z);
                break;

            case 3:
                offset = new Vector3(Random.Range(-minLength.x, -maxLength.x), Random.Range(-maxLength.y, maxLength.y), z);
                break;
            default:
                Debug.LogWarning("Detect invalid values");
                offset = new Vector3(Random.Range(-maxLength.x, maxLength.x), Random.Range(-maxLength.y, maxLength.y), z);
                break;
        }

        return transform.position + offset;
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
        Vector3[] verts = new Vector3[]
        {
            transform.position + new Vector3(-maxLength.x, -maxLength.y, 0),
            transform.position + new Vector3(-maxLength.x, maxLength.y,  0),
            transform.position + new Vector3( maxLength.x, maxLength.y,  0),
            transform.position + new Vector3( maxLength.x, -maxLength.y, 0)
        };

        Handles.DrawSolidRectangleWithOutline(verts, fillColor, outlineColor);

       verts = new Vector3[]
       {
            transform.position + new Vector3(-minLength.x, -minLength.y, 0),
            transform.position + new Vector3(-minLength.x, minLength.y,  0),
            transform.position + new Vector3( minLength.x, minLength.y,  0),
            transform.position + new Vector3( minLength.x, -minLength.y, 0)
       };

        Handles.DrawSolidRectangleWithOutline(verts, innerColor, outlineColor);
    }
#endif
    #endregion

}
