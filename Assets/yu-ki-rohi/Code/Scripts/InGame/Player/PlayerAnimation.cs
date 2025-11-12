using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : NormalPlayerComponent
{
    private Transform transform;
    private PlayerAnimationParameters parameters;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public PlayerAnimation(PlayerIndividualData player, Transform transform, PlayerAnimationParameters parameters, SpriteRenderer spriteRenderer, Animator animator) :
        base(player)
    {
        this.transform = transform;
        this.parameters = parameters;
        this.spriteRenderer = spriteRenderer;
        this.animator = animator;
    }

    public void FinishAction()
    {
        animator.SetTrigger("FinishAction");
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
            FlipX(mousePosition.x - transform.position.x);
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
            if (!player.IsShooting)
            {
                animator.SetBool("Shoot", true);
#if UNITY_EDITOR
                // 調整するときのために、Editor実行のときのみ毎回スピードを設定しなおす
                SetAnimationSpeed();
#endif
            }
        }

        // 離した瞬間
        else if (context.canceled)
        {
            if(player.State != Player.State.Aim) { return; }
            animator.SetBool("Shoot", false);
        }
    }

    public override void OnShootDir(Vector2 input)
    {

    }

    public override void OnDash(InputAction.CallbackContext context)
    {

    }

    #endregion

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

        animator.SetFloat("LeadInSpeed", leadInanimationTime / parameters.LeadInTime);
        animator.SetFloat("FollowThroughSpeed", followThroughanimationTime / parameters.FollowThroughTime);

    }
}
