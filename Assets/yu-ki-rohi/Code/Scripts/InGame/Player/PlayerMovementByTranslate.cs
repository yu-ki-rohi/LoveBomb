using UnityEngine;

public class PlayerMovementByTranslate : PlayerMovementBase
{
    public PlayerMovementByTranslate(InfoPackage infoPackage) :
        base(infoPackage)
    {

    }

    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        transform.position += (Vector3)moveDir * GetSpeed() * deltaTime;
        ClampPlayerPosition();
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {

    }
}
