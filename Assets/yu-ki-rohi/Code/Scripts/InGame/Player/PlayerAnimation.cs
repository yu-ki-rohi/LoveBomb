using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : NormalPlayerComponent
{
    public enum ShootStage
    {
        IDLE,
        LEAD_IN,
        STANDBY,
        FALLOW_THROUGH
    }
    private PlayerAnimationParameters parameters;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private CancellationTokenSource damagedCts;

    public PlayerAnimation(PlayerIndividualData player, PlayerAnimationParameters parameters, SpriteRenderer spriteRenderer, Animator animator) :
        base(player)
    {
        this.parameters = parameters;
        this.spriteRenderer = spriteRenderer;
        this.animator = animator;
    }

    public void SetShootStage(int index)
    {
        if (DebugMessenger.NullCheckError(animator)) { return; }
        animator.SetInteger("ShootStage", index);
    }

    // IUpdatableによって保証されているメソッド
    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        if(!player.IsShooting)
        {
            FlipX(player.MoveDir.x);
        }
        else if(player.State == Player.State.Aim)
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            FlipX(mousePosition.x - player.Transform.position.x);
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
    public override void OnShoot(InputAction.CallbackContext context)
    {
        // 押した瞬間
        if (context.performed)
        { 
            if (player.IsIdle)
            {
                animator.SetInteger("ShootStage", (int)ShootStage.LEAD_IN);
#if UNITY_EDITOR
                // 調整するときのために、Editor実行のときのみ毎回スピードを設定しなおす
                SetAnimationSpeed();
#endif
            }
        }

        // 離した瞬間
        else if (context.canceled)
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

    public override void OnDamaged()
    {
        animator.SetBool("IsDamaged", true);
        animator.SetInteger("ShootStage", (int)ShootStage.IDLE);

        damagedCts?.Cancel();
        damagedCts?.Dispose();
        damagedCts = new CancellationTokenSource();
        RigidAsync(damagedCts.Token).Forget();
#if UNITY_EDITOR
        // 調整するときのために、Editor実行のときのみ毎回スピードを設定しなおす
        SetAnimationSpeed();
#endif

    }

    private void FlipX(float horizontalValue)
    {
        if (horizontalValue > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalValue < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void SetAnimationSpeed()
    {
        // 元のアニメーションクリップの総時間
        // 現状は1.0秒で作っているので直入
        float leadInanimationTime = 1.0f;
        float followThroughanimationTime = 1.0f;
        float damagedTime = 1.0f;

        animator.SetFloat("LeadInSpeed", leadInanimationTime / parameters.LeadInTime);
        animator.SetFloat("FollowThroughSpeed", followThroughanimationTime / parameters.FollowThroughTime);
        animator.SetFloat("DamagedSpeed", damagedTime / parameters.DamagedRigidTime);

    }

    private async UniTaskVoid RigidAsync(CancellationToken token)
    {
        try
        {
            /* 
             * NOTE:
             * 名前付き引数
             * cancellationToken: token
             * 
             * public static UniTask Delay(
             *     TimeSpan delayTime,
             *     bool ignoreTimeScale = false,
             *     PlayerLoopTiming timing = PlayerLoopTiming.Update,
             *     CancellationToken cancellationToken = default
             * )
             * 
             */
            await UniTask.Delay(TimeSpan.FromSeconds(parameters.DamagedRigidTime), cancellationToken: token);
            animator.SetBool("IsDamaged", false);
            BlinkAsync(token).Forget();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Release stiffness");
            // CTSの破棄 ヌルチェック + 実行
            damagedCts?.Dispose();
            damagedCts = null;
        }
    }

    private async UniTaskVoid BlinkAsync(CancellationToken token)
    {
        try
        {
            ChaildBlinkAsync(token).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(parameters.DamagedInvincibleTime), cancellationToken: token);

            // HACK: 役割を考えると、あまりここでやるべき内容でないが一旦簡略化のためここで
            // LayerMask.NameToLayerを使う方が安全だが、一旦直接id指定     
            // 0: default
            player.Transform.gameObject.layer = 0;
            damagedCts.Cancel();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Blinking is canceled");
        }
        finally
        {
            // CTSの破棄 ヌルチェック + 実行
            damagedCts?.Dispose();
            damagedCts = null;
        }
    }

    private async UniTaskVoid ChaildBlinkAsync(CancellationToken token)
    {
        try
        {
            while (true)
            {
                // HACK: 点滅間隔は外に出すべきだが一旦マジックナンバー
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f), cancellationToken: token);
                spriteRenderer.enabled = false;

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
                spriteRenderer.enabled = true;
            }

        }
        catch (OperationCanceledException)
        {
            DebugMessenger.Log("Finish Blink");
        }
        finally
        {
            if(!DebugMessenger.NullCheckError(spriteRenderer)) 
            {
                spriteRenderer.enabled = true;
            }
        }
    }
}
