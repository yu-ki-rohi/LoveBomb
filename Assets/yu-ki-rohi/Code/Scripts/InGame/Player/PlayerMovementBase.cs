using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerMovementBase : NormalPlayerComponent
{
    public struct InfoPackage
    {
        public PlayerIndividualData PlayerIndividualData;
        public Transform Transform;
        public PlayerMovementParameters Parameters;
        public Vector2 StageRange;
        public Vector2 StageCenter;

        public InfoPackage(PlayerIndividualData playerIndividualData, Transform transform, PlayerMovementParameters parameters, Vector2 stageRange, Vector2 stageCenter)
        {
            PlayerIndividualData = playerIndividualData;
            Transform = transform;
            Parameters = parameters;
            StageRange = stageRange;
            StageCenter = stageCenter;
        }
    }

    protected Transform transform;

    protected PlayerMovementParameters parameters;
   

    // 移動範囲
    private readonly Vector2 stageRange;
    private readonly Vector2 stageCenter;

    private bool isDash = false;

    // プレイヤーが移動できる各方向への限界
    // ラムダ式で定義
    public float MoveLimitRight { get => stageCenter.x + (stageRange.x / 2) - parameters.PlayerRadius; }

    public float MoveLimitLeft { get => stageCenter.x - (stageRange.x / 2) + parameters.PlayerRadius; }

    public float MoveLimitUp { get => stageCenter.y + (stageRange.y / 2) - parameters.PlayerRadius; }

    public float MoveLimitDown { get => stageCenter.y - (stageRange.y / 2) + parameters.PlayerRadius; }

    public PlayerMovementBase(InfoPackage infoPackage) :
       base(infoPackage.PlayerIndividualData)
    {
        transform = infoPackage.Transform;
        parameters = infoPackage.Parameters;
        stageRange = infoPackage.StageRange;
        stageCenter = infoPackage.StageCenter;
    }

    // IUpdatableによって保証されているメソッド
   
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
        player.MoveDir = input;
    }


    public override void OnShoot(InputAction.CallbackContext context)
    {

    }

    public override void OnShootDir(Vector2 input)
    {


    }


    public override void OnDash(InputAction.CallbackContext context)
    {

        // 押した瞬間
        if (context.performed)
        {
            isDash = true;
        }

        // 離した瞬間
        else if (context.canceled)
        {
            isDash = false;
        }
    }

    #endregion




    protected float GetSpeed()
    {
        float multiplier = 1.0f;

        
        if (player.IsShooting)
        {
            multiplier *= parameters.AimSpeedMultiplier;
        }

        else if (isDash)
        {
            multiplier *= parameters.DashSpeedMultiplier;
        }

        return parameters.Agility * multiplier;
    }

    protected void ClampPlayerPosition()
    {
        // X方向補正
        float posX = Mathf.Clamp(transform.position.x, MoveLimitLeft, MoveLimitRight);

        // Y方向補正
        float posY = Mathf.Clamp(transform.position.y, MoveLimitDown, MoveLimitUp);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
   
}
