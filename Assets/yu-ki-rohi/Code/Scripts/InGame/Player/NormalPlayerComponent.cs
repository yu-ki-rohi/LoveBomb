using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NormalPlayerComponent : PlayerComponentBase, IUpdatable, IInputCallbackOfPlayer
{
    public NormalPlayerComponent(PlayerIndividualData owner) :
        base(owner)
    {

    }

    // IUpdatableによって保証されているメソッド
    public abstract void Start();

    public abstract void Update(float deltaTime);

    public abstract void FixedUpdate(float fixedDeltaTime);

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }
    // ここまで

    #region Input System関連
    // 先に変換してから渡す
    public virtual void OnMove(Vector2 input)
    {

    }

    public virtual void OnShoot(InputAction.CallbackContext context)
    {

    }

    public virtual void OnShootDir(Vector2 input)
    {


    }

    public virtual void OnDash(InputAction.CallbackContext context)
    {

    }

    #endregion


}
