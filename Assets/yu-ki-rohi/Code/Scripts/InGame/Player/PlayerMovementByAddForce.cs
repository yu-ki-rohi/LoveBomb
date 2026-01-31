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

    public override void OnDamaged()
    {
        rigidbody.linearVelocity = Vector3.zero;
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
        if(player.State == Player.State.Damaged) { return; }
        Vector2 addedForce = player.MoveDir * parameters.AccelerationForce;

        // 最高速度を設定
        float speedLimit = GetSpeed();

        // 現在の速度ベクトルと力の加わる向きの関係を調べる
        float dotOfVelocityAndAddedForce = Vector2.Dot(rigidbody.linearVelocity, addedForce);
#if true
        // 速度超過している + 加速方向に力が加わっている
        // 大小比較なので平方根はとらない
        if (rigidbody.linearVelocity.sqrMagnitude > speedLimit * speedLimit &&
            dotOfVelocityAndAddedForce > 0)
        {
            addedForce += -rigidbody.linearVelocity * parameters.AccelerationForce / speedLimit;
        }

        // 速度ベクトル方向に力が加わっていないとき、制動をかける
        if (dotOfVelocityAndAddedForce <= 0)
        {
            addedForce += -rigidbody.linearVelocity * parameters.DampingForce;
        }
#else
        // 速度超過している + 加速方向に力が加わっている
        // 大小比較なので平方根はとらない
        if (rigidbody.linearVelocity.sqrMagnitude > speedLimit * speedLimit &&
            dotOfVelocityAndAddedForce > 0)
        {
            // 速度ベクトルを正規化
            Vector2 normalizedVelocity = rigidbody.linearVelocity.normalized;
            // normalizedVelocityと直交するベクトル
            Vector2 VectorAtRightAnglesToVelocity = new(normalizedVelocity.y, -normalizedVelocity.x);

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
#endif
        // 質量の影響を無視
        addedForce *= rigidbody.mass;
        rigidbody.AddForce(addedForce, ForceMode2D.Force);
    }
}
