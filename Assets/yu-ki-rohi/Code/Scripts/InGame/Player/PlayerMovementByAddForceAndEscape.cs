using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementByAddForceAndEscape : PlayerMovementByAddForce
{
    private float burstCoolTime = 0.0f;

    private bool CanBurst { get => burstCoolTime <= 0.0f; }

    public PlayerMovementByAddForceAndEscape(InfoPackage infoPackage, Rigidbody2D rigidbody) :
       base(infoPackage, rigidbody)
    {
        
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if(!CanBurst)
        {
            burstCoolTime -= deltaTime;
        }
    }

    public override void OnDash(InputAction.CallbackContext context)
    {
        // ‰Ÿ‚µ‚½uŠÔ
        if (CanBurst && context.performed)
        {
            burstCoolTime = parameters.BurstCoolTime;
            rigidbody.AddForce(moveDir * parameters.BurstForce * GetSpeed());
        }
    }
}
