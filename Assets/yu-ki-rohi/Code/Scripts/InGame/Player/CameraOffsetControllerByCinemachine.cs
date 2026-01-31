using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOffsetControllerByCinemachine : NormalPlayerComponent
{
    private Transform cameraTarget;
    private bool isAimMode = false;
    private CameraOffsetByCinemachineParameters parameters;
    private CancellationTokenSource changeMoveModeCts;
    public CameraOffsetControllerByCinemachine(PlayerIndividualData player, Transform cameraTarget) :
       base(player)
    {
        this.cameraTarget = cameraTarget;
        parameters = player.CameraOffsetByCinemachineParameters;
    }

    // IUpdatableによって保証されているメソッド
    public override void Start()
    {
        DebugMessenger.NullCheckWarning(cameraTarget);
    }

    public override void Update(float deltaTime)
    {
        if(cameraTarget == null) { return; }

        if (player.IsShooting)
        {
            isAimMode = true;
            changeMoveModeCts?.Cancel();
            changeMoveModeCts?.Dispose();
            changeMoveModeCts = null;
        }
        else if (changeMoveModeCts == null)
        {
            changeMoveModeCts = new CancellationTokenSource();
            ChangeToMoveModeAsync(changeMoveModeCts.Token).Forget();
        }

           
        if (isAimMode)
        {
            cameraTarget.position = player.Transform.position + (Vector3)player.ShootDir * parameters.Distance;
        }
        else
        {
            cameraTarget.position = player.Transform.position + (Vector3)player.MoveDir * parameters.Distance;
        }
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
      
    }

    public override void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           
        }

        if (context.performed)
        {
           
        }
    }

    public override void OnShootDir(Vector2 input)
    {

    }

    public override void OnDash(InputAction.CallbackContext context)
    {

    }

    #endregion
    private async UniTaskVoid ChangeToMoveModeAsync(CancellationToken token)
    {
        try
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < parameters.TimeOfChangeAimToMove)
            {
                // フレーム待ち（Updateタイミング）
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                // 経過時間加算
                elapsedTime += Time.deltaTime;
            }
            isAimMode = false;
        }
        catch (OperationCanceledException)
        {
            DebugMessenger.Log("Continue AimMode");
        }
        finally
        {
            // CTSの破棄 ヌルチェック + 実行
            changeMoveModeCts?.Dispose();
            changeMoveModeCts = null;
        }
    }
}
