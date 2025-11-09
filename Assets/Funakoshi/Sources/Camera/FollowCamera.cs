using UnityEditor;
using UnityEngine;

// yu-ki-rohi追加分
// RequireComponentでCameraの存在を保証
[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Camera cameraComponent;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField, Header("ステージの範囲を指定します")]
    private Vector2 stageRange;
    [SerializeField]
    private Vector2 stageCenter = Vector2.zero;

    private Vector2 cameraRange;


    // yu-ki-rohi追加分
    [SerializeField]
    private Vector2 cameraOffset = Vector2.zero;
    private Vector2 previousCameraOffset = Vector2.zero;
    [SerializeField, Range(0.01f, 3.0f)]
    private float offsetLerpDuration = 0.5f;
    private float offsetLerpElapsedTime = 0.0f;
    public Vector2 StageRange { get { return stageRange; } }
    public Vector2 StageCenter { get {  return stageCenter; } }
    public Vector2 CameraRange { get { return cameraRange; } }
    
    public void ChangeCameraOffSet(Vector2 offset)
    {
        float progress = Mathf.Clamp01(offsetLerpElapsedTime * offsetLerpDuration);
        if (progress < 1)
        {
            previousCameraOffset = Vector2.Lerp(previousCameraOffset, cameraOffset, progress);
        }
        else
        {
            previousCameraOffset = cameraOffset;
        }
        cameraOffset = offset;
        offsetLerpElapsedTime = 0.0f;
    }

    void Awake()
    {
        // yu-kirohi追加
        // 流石にカメラはGetComponentじゃないとめんどい
        // というかCamera.main.orthographicSizeにしてない理由はなんだろう
        cameraComponent = GetComponent<Camera>();

        float viewHeight = cameraComponent.orthographicSize * 2;
        float viewWidth = viewHeight * cameraComponent.aspect;
        cameraRange = new Vector2(viewWidth, viewHeight);

        // yu-ki-rohi追加分
        // 必要なのは逆数なので、先に変換しておく
        offsetLerpDuration = 1.0f / offsetLerpDuration;
    }


    void Update()
    {
        // yu-ki-rohi変更点 cameraOffSetを反映
        // target.positionの方をキャストしたほうがいい気がするけど、元の部分を残すために他方をキャスト
        Vector2 currentCameraOffset = LerpCameraOffset();
        Vector3 cameraPosition = (Vector3)StageLimited(targetTransform.position + (Vector3)currentCameraOffset) + new Vector3(0, 0, -10);
        cameraComponent.transform.position = cameraPosition;
    }

    private Vector2 StageLimited(Vector2 currentPos)
    {
        float rightLimit = stageCenter.x + (stageRange.x / 2) - (cameraRange.x / 2);
        float leftLimit = stageCenter.x - (stageRange.x / 2) + (cameraRange.x / 2);
        float upLimit = stageCenter.y + (stageRange.y / 2) - (cameraRange.y / 2);
        float downLimit = stageCenter.y - (stageRange.y / 2) + (cameraRange.y / 2);

        float posX = Mathf.Clamp(currentPos.x, leftLimit, rightLimit);
        float posY = Mathf.Clamp(currentPos.y, downLimit, upLimit);

        return new Vector2(posX, posY);
    }

    private Vector2 LerpCameraOffset()
    {
        float progress = Mathf.Clamp01(offsetLerpElapsedTime * offsetLerpDuration);
        if(progress >= 1) { return cameraOffset; }

        offsetLerpElapsedTime += Time.deltaTime;
        progress = Mathf.Clamp01(offsetLerpElapsedTime * offsetLerpDuration);
        return Vector2.Lerp(previousCameraOffset, cameraOffset, progress);
    }

    // yu-ki-rohi追加分
    #region Unity Editor
#if UNITY_EDITOR
   
    // Sceneビュー上に範囲を表示
    private void OnDrawGizmosSelected()
    {
        float rightLimit = stageCenter.x + (stageRange.x / 2) - (cameraRange.x / 2);
        float leftLimit = stageCenter.x - (stageRange.x / 2) + (cameraRange.x / 2);
        float upLimit = stageCenter.y + (stageRange.y / 2) - (cameraRange.y / 2);
        float downLimit = stageCenter.y - (stageRange.y / 2) + (cameraRange.y / 2);

        Vector3[] verts = new Vector3[]
        {
            transform.position + new Vector3(leftLimit, downLimit, 0),
            transform.position + new Vector3(leftLimit, upLimit,  0),
            transform.position + new Vector3(rightLimit, upLimit,  0),
            transform.position + new Vector3(rightLimit, downLimit, 0)
        };

        Color fillColor = new Color(1.0f, 1.0f, 0.0f, 0.2f);

        Handles.DrawSolidRectangleWithOutline(verts, fillColor, Color.yellow);
    }
#endif
    #endregion
}
