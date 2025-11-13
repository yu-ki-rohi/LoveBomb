using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.UI;

// Input System関連の参考資料：https://nekojara.city/unity-input-system-player-input

// HACK:要リファクタリング

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    #region 列挙型
    public enum State
    {
        Idle,
        Aim,
        Shoot
    }

    // 提案用に挙動パターンを複数個用意するとき、
    // こんな感じでenumを用意して切り替えると
    // 他の人に伝える時に便利だと思います
    enum MovePattern
    {
        AddForceAndEscape,
        AddForce,
        RigidbodyVelocity,
        Transform,
        Translate
    }

    #endregion

    #region シリアライズするフィールド

    [SerializeField] private MovePattern movePattern = MovePattern.Transform;

    [SerializeField] private PlayerParameters parameters;

    // Input System 利用のため
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private FollowCamera followCamera;

    [SerializeField] private ArrowPoolManager arrowPoolManager;
    // 一旦プレイヤーから操作
    [SerializeField] private Image heartGauge;

    #endregion

    #region　その他のフィールド

    private PlayerIndividualData data;

    private List<NormalPlayerComponent> playerComponents = new();

    // ゲームパッドが接続されているか
    private bool isGamePadConnected = false;

    #endregion

    #region プロパティ
    

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
            if (data.IsShooting) { return; }
            data.ChangeState(State.Aim);
          
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

    public void AddHeartEnergy(int energy)
    {
        data.AddHeartEnergy(energy);
    }

    #region Enable, Disable, Destroyの際のふるまい
    private void OnEnable()
    {
        // ヌルチェック + エラーメッセージ
        if (DebugMessenger.NullCheckError(playerInput)) { return; }

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
        if (DebugMessenger.NullCheckError(playerInput)) { return; }

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

    #endregion

    #region 初期化
    void Awake()
    {
        data = new PlayerIndividualData(parameters, heartGauge);
        // データ部にゲームオブジェクトのTransformへの参照を書き込み
        data.Transform = transform;
        
        data.HeartEnergy = parameters.PlayerShootParameters.InitialHeartEnergy;
        data.ReflectUI();

        // 移動コンポーネント
        var infoPackage = new PlayerMovementBase.InfoPackage(
                    data,
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
        var playerAnimation = new PlayerAnimation(data, transform, parameters.PlayerAnimationParameters, GetComponent<SpriteRenderer>(), GetComponent<Animator>());
        playerComponents.Add(playerAnimation);

        // カメラオフセットコンポーネント
        playerComponents.Add(new CameraOffsetController(data, followCamera, parameters.CameraOffsetParameters));

        // 射撃コンポーネント
        var arrowShooter = new ArrowShooter(data, arrowPoolManager, parameters.PlayerShootParameters, parameters.PlayerAnimationParameters, playerAnimation);
        playerComponents.Add(arrowShooter);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DebugMessenger.NullCheckError(parameters) ||
            DebugMessenger.NullCheckError(followCamera) ||
            DebugMessenger.NullCheckError(arrowPoolManager))
        { return; }

        arrowPoolManager.SetArrowParameters(parameters.PlayerShootParameters);

        data.HeartEnergy = parameters.PlayerShootParameters.InitialHeartEnergy;

        // ゲームパッド接続確認
        CheckGamePadIsConnected();


        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.Start();
        }
    }


    #endregion

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

    #region 当たり判定系
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("HeartEnergy") && 
            collision.TryGetComponent<HeartEnergy>(out var heartEnergy))
        {
            heartEnergy.Target = this;
        }
    }
    #endregion

    #region ヘルパーメソッド
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
        if (enabled)
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
    #endregion

}
