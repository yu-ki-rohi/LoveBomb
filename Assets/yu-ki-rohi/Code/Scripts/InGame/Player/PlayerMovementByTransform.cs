using UnityEngine;

public class PlayerMovementByTransform : PlayerMovementBase
{
    public PlayerMovementByTransform(InfoPackage infoPackage) :
         base(infoPackage)
    {
        
    }

    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        transform.position += (Vector3)player.MoveDir * GetSpeed() * deltaTime;
        ClampPlayerPosition();
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        
    }
}
