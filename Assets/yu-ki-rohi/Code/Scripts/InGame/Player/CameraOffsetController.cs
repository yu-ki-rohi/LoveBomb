using UnityEngine;
using UnityEngine.InputSystem;

// TODO:UniTaskで動かすVerを作る
public class CameraOffsetController : NormalPlayerComponent
{
    private FollowCamera followCamera;
    private CameraOffsetParameters parameters;

    // 移動方向
    private Vector2 moveDir = Vector2.zero;


    private float offsetTimerX = 0.0f;
    private float offsetTimerY = 0.0f;
    private Vector2 cameraOffset = Vector2.zero;

    public CameraOffsetController(Player player, FollowCamera followCamera, CameraOffsetParameters parameters) :
        base(player)
    {
        this.followCamera = followCamera;
        this.parameters = parameters;
    }

    // IUpdatableによって保証されているメソッド
    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        OffsetOnMove(deltaTime);
        OffsetOnAim(deltaTime);
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {

    }

    public override void OnEnable()
    {

    }

    public override void OnDisable()
    {

    }
    // ここまで

    #region Input System関連
    // 先に変換してから渡す
    public override void OnMove(Vector2 input)
    {
        // 入力値が小さい場合は0に丸める
        // borderをパラメーターとして外部に出すかは要検討
        float border = 0.2f;
        if(Mathf.Abs(input.x) < border)
        {
            input.x = 0;
        }
        if (Mathf.Abs(input.y) < border)
        {
            input.y = 0;
        }

        // 無入力状態になった時はタイマーリセット
        if (input == Vector2.zero)
        {
            ResetOffsetTimer();
        }

        // 入力方向が反対になった時は該当タイマーをリセット
        if(input.x * moveDir.x < 0)
        {
            offsetTimerX = 0.0f;
        }
        if (input.y * moveDir.y < 0)
        {
            offsetTimerY = 0.0f;
        }
        moveDir = input;
    }

    public override void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ResetOffsetTimer(true);
            OffsetOnAim();
        }

        if (context.performed)
        {
            ResetOffsetTimer();
        }
    }

    public override void OnShootDir(Vector2 input)
    {


    }

    public override void OnDash(InputAction.CallbackContext context)
    {

    }

    #endregion

    private void OffsetOnMove(float deltaTime)
    {
        if (player.IsShooting) { return; }

        float offsetX = NextOffsetOnMove(cameraOffset.x, moveDir.x, ref offsetTimerX, deltaTime, parameters.OffsetX);
        float offsetY = NextOffsetOnMove(cameraOffset.y, moveDir.y, ref offsetTimerY, deltaTime, parameters.OffsetY);

        // 変化がない場合はここで終わる
        Vector2 newOffset = new(offsetX, offsetY);
        if (cameraOffset == newOffset) { return; }

        cameraOffset = newOffset;
        followCamera.ChangeCameraOffSet(cameraOffset);
    }

    private void OffsetOnAim(float deltaTime = -1.0f)
    {
        // playerのStateがAimじゃないとき
        // 例外として引数に-1を渡したとき(引数無しで実行したとき)は処理を実行
        if (player.State != Player.PlayerState.Aim &&
            deltaTime != -1.0f) { return; }

        Vector2 mousePosition = Input.mousePosition;

        // 要検討:borderをパラメーターとして外に出すか
        float border = 0.1f;

        Vector2 cameraSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        float newOffsetX = NextOffsetOnShoot(cameraOffset.x, mousePosition.x, ref offsetTimerX, deltaTime, parameters.OffsetX, cameraSize.x, border);
        float newOffsetY = NextOffsetOnShoot(cameraOffset.y, mousePosition.y, ref offsetTimerY, deltaTime, parameters.OffsetY, cameraSize.y, border);

        if( cameraOffset.x == newOffsetX &&
            cameraOffset.y == newOffsetY) { return; }

        cameraOffset = new Vector2(newOffsetX, newOffsetY);
        followCamera.ChangeCameraOffSet(cameraOffset);
    }

    private void ResetOffsetTimer(bool reverse = false)
    {
        if(reverse)
        {
            offsetTimerX = 0.0f;
            offsetTimerY = 0.0f;
        }
        else
        {
            float Time = parameters.ChangeOffsetTime;
            offsetTimerX = Time;
            offsetTimerY = Time;
        }
    }

    private float NextOffsetOnMove(float currentOffset, float moveDir, ref float offsetTimer, float deltaTime, float offsetRange)
    {
        if (!HasTimeElapsedOnMove(currentOffset, moveDir, ref offsetTimer, deltaTime)) { return currentOffset; }

        if(moveDir < 0)
        {
            return -offsetRange;
        }
        else if(moveDir > 0)
        {
            return offsetRange;
        }
        else
        {
            return 0;
        }
    }

    private bool HasTimeElapsedOnMove(float offset, float moveDir, ref float offsetTimer, float deltaTime)
    {
        // 入力方向と偏り方向が同じ場合はスキップ
        if(  offset * moveDir > 0 ||
            (offset == 0 && moveDir == 0)) { return false; }


        offsetTimer -= deltaTime;


        if(offsetTimer < 0)
        {
            offsetTimer = parameters.ChangeOffsetTime;
            return true;
        }
        return false;
    }


    private float NextOffsetOnShoot(float currentOffset, float mousePosition, ref float offsetTimer, float deltaTime, float offsetRange, float cameraSize, float border)
    {
        if (offsetTimer > 0)
        {
            offsetTimer -= deltaTime;
            return currentOffset;
        }
        float newOffset = currentOffset;
        // 正方向
        if (mousePosition > (1.0f - border) * cameraSize &&
            currentOffset < offsetRange)
        {
            newOffset += offsetRange;
        }
        // 負方向
        else if (mousePosition < border * cameraSize &&
            currentOffset > -offsetRange)
        {
            newOffset -= offsetRange;
        }

        if (newOffset == currentOffset) { return currentOffset; }

        offsetTimer = parameters.ChangeOffsetTime;
        return newOffset;
    }

}
