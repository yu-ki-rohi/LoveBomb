using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputCallbackOfPlayer
{
    // æ‚É•ÏŠ·‚µ‚Ä‚©‚ç“n‚·
    public void OnMove(Vector2 input);

    public void OnShoot(InputAction.CallbackContext context);


    public void OnShootDir(Vector2 input);

    public void OnDash(InputAction.CallbackContext context);

}
