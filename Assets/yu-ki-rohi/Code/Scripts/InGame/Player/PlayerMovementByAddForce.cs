using System;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovementByAddForce : PlayerMovementBase
{
    protected Rigidbody2D rigidbody;

    public PlayerMovementByAddForce(InfoPackage infoPackage, Rigidbody2D rigidbody) :
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
        Vector2 addedForce = player.MoveDir * parameters.AccelerationForce;

        // 最高速度を設定
        float speedLimit = GetSpeed();

        // 現在の速度ベクトルと力の加わる向きの関係を調べる
        float dotOfVelocityAndAddedForce = Vector2.Dot(rigidbody.linearVelocity, addedForce);

        // 速度超過している + 加速方向に力が加わっている
        // 大小比較なので平方根はとらない
        if (rigidbody.linearVelocity.sqrMagnitude > speedLimit * speedLimit &&
            dotOfVelocityAndAddedForce > 0)
        {
            // 速度ベクトルを正規化
            Vector2 normalizedVelocity = rigidbody.linearVelocity.normalized;
            // normalizedVelocityと直交するベクトル
            Vector2 VectorAtRightAnglesToVelocity = new (normalizedVelocity.y, -normalizedVelocity.x);

            // 加える力から加速成分を抜く
            float cos = Vector2.Dot(VectorAtRightAnglesToVelocity, addedForce);
            addedForce = VectorAtRightAnglesToVelocity * cos;

            // 現在の速度ベクトルと力の加わる向きの関係を更新
            dotOfVelocityAndAddedForce = Vector2.Dot(rigidbody.linearVelocity, addedForce);
        }

        // 速度ベクトル方向に力が加わっていないとき、制動をかける
        if (dotOfVelocityAndAddedForce <= 0)
        {
            addedForce += -rigidbody.linearVelocity * parameters.DampingForce;
        }

        // 質量の影響を無視
        addedForce *= rigidbody.mass;
        rigidbody.AddForce(addedForce, ForceMode2D.Force);

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
