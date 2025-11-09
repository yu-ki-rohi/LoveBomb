using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

// Input System関連の参考資料：https://nekojara.city/unity-input-system-player-input

// HACK:要リファクタリング

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    // 提案用に挙動パターンを複数個用意するとき、
    // こんな感じでenumを用意して切り替えると
    // プランナーとかに伝えやすいと思います
    enum MovePattern
    {
        AddForceAndEscape,
        AddForce,
        RigidbodyVelocity,
        Transform,
        Translate
    }
    [SerializeField] private MovePattern movePattern = MovePattern.Transform;

    [SerializeField] private PlayerParameters parameters;

    // Input System 利用のため
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private FollowCamera followCamera;

    [SerializeField] private ArrowPoolManager arrowPoolManager;

    public enum PlayerState
    { 
        Idle,
        Aim,
        Shoot
    }
    private PlayerState state;


    private List<NormalPlayerComponent> playerComponents = new ();
    private IShootable shootable;
    // ゲームパッドが接続されているか
    private bool isGamePadConnected = false;

    #region プロパティ
    // 定義が面倒なのでラムダ式
    public PlayerState State { get => state; }

    public bool IsShooting { get => state == PlayerState.Aim ||  state == PlayerState.Shoot; }

    #endregion

    #region Animation Clip から起動する予定のメソッド
    [Preserve]
    public void Shoot()
    {
        ChangeState(PlayerState.Shoot);
        shootable?.Shoot();
    }

    [Preserve]
    public void FinishShooting()
    {
        ChangeState(PlayerState.Idle);
    }

    #endregion

    #region  Player Input に登録するメソッド
    private void OnMove(InputAction.CallbackContext context)
    {
        // Move以外では処理しない
        if (context.action.name != "Move") { return; }

        // 入力情報の受け取り
        Vector2 input = context.ReadValue<Vector2>();
        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnMove(input);
        }

    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        // Shoot以外では処理しない
        if (context.action.name != "Shoot") { return; }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnShoot(context);
        }

        // 押した瞬間
        if (context.performed)
        {
            if (IsShooting) { return; }
            ChangeState(PlayerState.Aim);
          
        }

    }

    private void OnShootDir(InputAction.CallbackContext context)
    {
        //HACK:要リファクタリング

        // ShootDir以外では処理しない
        if (context.action.name != "ShootDir") { return; }

        Vector3 input = context.ReadValue<Vector2>();
        if (!isGamePadConnected)
        {
            input = Camera.main.ScreenToWorldPoint(input);
        }
        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnShootDir((Vector2)(input - transform.position));
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        // Dash以外では処理しない
        if (context.action.name != "Dash") { return; }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnDash(context);
        }

    }

    #endregion


    private void OnEnable()
    {
        // ヌルチェック + エラーメッセージ
        // todo : 後でデバッグ用機能はまとめたい
        if(playerInput == null)
        {
            Debug.LogError("Player Input is Null!!");
            return;
        }

        foreach (var playerComponent in playerComponents)
        {
            playerComponent.OnEnable();
        }

        // Player Inputにメソッドを登録
        SetInputEnabled(true);
    }

    private void OnDisable()
    {
        // ヌルチェック + エラーメッセージ
        // todo : 後でデバッグ用機能はまとめたい
        if (playerInput == null)
        {
            Debug.LogError("Player Input is Null!!");
            return;
        }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnDisable();
        }

        // Player Inputのメソッドを解除
        SetInputEnabled(false);
    }

    private void OnDestroy()
    {
        playerComponents.Clear();
    }

    void Awake()
    {
        // 移動コンポーネント
        var infoPackage = new PlayerMovementBase.InfoPackage(
                    this,
                    transform,
                    parameters.PlayerMovementParameters,
                    followCamera.StageRange,
                    followCamera.StageCenter
            );

        switch (movePattern)
        {
            case MovePattern.AddForceAndEscape:
                playerComponents.Add(new PlayerMovementByAddForceAndEscape(infoPackage, GetComponent<Rigidbody2D>()));
                break;

            case MovePattern.AddForce:
                playerComponents.Add(new PlayerMovementByAddForce(infoPackage, GetComponent<Rigidbody2D>()));
                break;

            case MovePattern.RigidbodyVelocity:
                playerComponents.Add(new PlayerMovementByRigidbodyVelocity(infoPackage, GetComponent<Rigidbody2D>()));
                break;

            case MovePattern.Transform:
                playerComponents.Add(new PlayerMovementByTransform(infoPackage));
                break;

            case MovePattern.Translate:
                playerComponents.Add(new PlayerMovementByTranslate(infoPackage));
                break;
        }

        // アニメーションコンポーネント
        var playerAnimation = new PlayerAnimation(this, transform, parameters.PlayerAnimationParameters, GetComponent<SpriteRenderer>(), GetComponent<Animator>());
        playerComponents.Add(playerAnimation);

        // カメラオフセットコンポーネント
        playerComponents.Add(new CameraOffsetController(this, followCamera, parameters.CameraOffsetParameters));

        // 射撃コンポーネント
        var arrowShooter = new ArrowShooter(this, arrowPoolManager, parameters.PlayerShootParameters, parameters.PlayerAnimationParameters, playerAnimation);
        playerComponents.Add(arrowShooter);
        shootable = arrowShooter;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if( DebugMessenger.NullCheckError(parameters) ||
            DebugMessenger.NullCheckError(followCamera) ||
            DebugMessenger.NullCheckError(arrowPoolManager))
        { return; }

        arrowPoolManager.SetArrowParameters(parameters.PlayerShootParameters);

        // ゲームパッド接続確認
        CheckGamePadIsConnected();

       
        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.Start();
        }
    }

    private void FixedUpdate()
    {
        // Rigidbodyを扱う場合はFixedUpdateを使用してください
        foreach(var playerComoponent in playerComponents)
        {
            playerComoponent.FixedUpdate(Time.fixedDeltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.Update(Time.deltaTime);
        }

    }


    private void CheckGamePadIsConnected()
    {
        // 参考資料：https://kan-kikuchi.hatenablog.com/entry/InputSystem_onDeviceChange

        // 全デバイスを取得
        var devices = InputSystem.devices;

        isGamePadConnected = false;
        foreach (var device in devices)
        {
            if (device is Gamepad)
            {
                //デバイスがゲームパッド(コントローラー)の時だけ処理
                Gamepad gamepad = device as Gamepad;
                Debug.Log($"Ditect Contoroller: {gamepad.displayName}");
                isGamePadConnected = true;
                break;
            }
        }

#if UNITY_EDITOR
        // ゲームパッドが検出されたかをログへ出力
        string gamepadExist = isGamePadConnected ? "GamePad" : "KeyBoard and Mouse";
        Debug.Log(gamepadExist + " Mode");
#endif

    }

    public void ChangeState(PlayerState nextState)
    {
        Debug.Log("PlayerState: " + state + " → " + nextState);
        state = nextState;
    }

    private void SetInputEnabled(bool enabled)
    {
        Action<InputAction.CallbackContext>[] actions =
        {
            OnMove,
            OnShoot,
            OnShootDir,
            OnDash,
        };
        // 登録処理
        if(enabled)
        {
            foreach (var action in actions)
            {
                playerInput.onActionTriggered += action;
            }

        }
        // 解除処理
        else
        {
            foreach (var action in actions)
            {
                playerInput.onActionTriggered -= action;
            }

        }
    }
}
