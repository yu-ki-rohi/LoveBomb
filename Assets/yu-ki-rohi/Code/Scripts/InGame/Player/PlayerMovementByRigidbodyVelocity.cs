using UnityEngine;

public class PlayerMovementByRigidbodyVelocity : PlayerMovementBase
{
    private Rigidbody2D rigidbody;

    public PlayerMovementByRigidbodyVelocity(InfoPackage infoPackage, Rigidbody2D rigidbody) :
        base(infoPackage)
    {
        this.rigidbody = rigidbody;
    }

    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        ClampPlayerPosition();
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        // 速度の設定
        rigidbody.linearVelocity = player.MoveDir * GetSpeed();

        // 制限範囲を超えさせない処理
        // とりあえず安直な方法で

        // 条件式の意味：制限範囲を超えている かつ 制限範囲を超える方向の速度があるとき
        if (transform.position.x > MoveLimitRight &&
            rigidbody.linearVelocityX > 0)
        {
            rigidbody.linearVelocityX = 0;
        }
        else if (transform.position.x < MoveLimitLeft &&
            rigidbody.linearVelocityX < 0)
        {
            rigidbody.linearVelocityX = 0;
        }
        if (transform.position.y > MoveLimitUp &&
            rigidbody.linearVelocityY > 0)
        {
            rigidbody.linearVelocityY = 0;
        }
        else if (transform.position.y < MoveLimitDown &&
            rigidbody.linearVelocityY < 0)
        {
            rigidbody.linearVelocityY = 0;
        }
    }
}
