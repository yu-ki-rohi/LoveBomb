using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Input System関連の参考資料：https://nekojara.city/unity-input-system-player-input

// HACK:要リファクタリング

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(SpriteRenderer))]
public class Player : MonoBehaviour, IDamageable
{
    #region 列挙型
    public enum State
    {
        Idle,
        Aim,
        Shoot,
        Damaged
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

    [SerializeField] private EffectPoolManager effectPoolManager;

    [SerializeField] private ItemDataBase itemData;

    // 一旦プレイヤーから操作
    [SerializeField] private Image heartGauge;

    #endregion

    #region　その他のフィールド

    private PlayerIndividualData data;

    private List<NormalPlayerComponent> playerComponents = new();

    private event Action OnDamaged;
    

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

    }

    private void OnShootDir(InputAction.CallbackContext context)
    {
        //HACK:要リファクタリング

        // ShootDir以外では処理しない
        if (context.action.name != "ShootDir") { return; }

        Vector3 input = context.ReadValue<Vector2>();
        if (!data.IsGamePadConnected)
        {
            input = Camera.main.ScreenToWorldPoint(input) - transform.position;
        }
        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnShootDir(((Vector2)(input)).normalized);
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

    private void OnUseItem(InputAction.CallbackContext context)
    {
        // UseItem以外では処理しない
        if (context.action.name != "UseItem") { return; }

        //foreach (var playerComoponent in playerComponents)
        //{
        //    playerComoponent.OnDash(context);
        //}

    }

    private void OnSelectItem(InputAction.CallbackContext context)
    {
        // SelectItem以外では処理しない
        if (context.action.name != "SelectItem") { return; }

        DebugMessenger.Log(context.ReadValue<float>().ToString());

        //foreach (var playerComoponent in playerComponents)
        //{
        //    playerComoponent.OnDash(context);
        //}

    }

    #endregion

    public void AddHeartEnergy(int energy)
    {
        data.AddHeartEnergy(energy);
    }

    public void AddItem(int id)
    {
        // 不正なidの場合か、所持上限をこえる場合はスキップ
        if( id < 0 || 
            id > itemData.Items.Count ||
            itemData.Items[id].NumberOfPossessions >= itemData.Items[id].MaxNum) { return; }
        itemData.Items[id].NumberOfPossessions++;
    }

    public void TakeDamage(int attack, DamageType damageType)
    {
        if (damageType != DamageType.Scaring ||
            data.State == State.Damaged) { return; }
        // LayerMask.NameToLayerを使う方が安全だが、一旦直接id指定     
        // 10: PlayerInvincible
        gameObject.layer = 10;
        OnDamaged.Invoke();
        data.LoseHeartEnergy(attack);
        data.ChangeState(State.Damaged);
        StartCoroutine(RigidCoroutine());
        
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
        OnDamaged = null;
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

        data.AddHeartEnergy(parameters.PlayerShootParameters.InitialHeartEnergy);

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
        var playerAnimation = new PlayerAnimation(data, parameters.PlayerAnimationParameters, GetComponent<SpriteRenderer>(), GetComponent<Animator>());
        playerComponents.Add(playerAnimation);

        // カメラオフセットコンポーネント
        playerComponents.Add(new CameraOffsetControllerByCinemachine(data, transform.Find("CameraTarget")));

        // 射撃コンポーネント
        var arrowShooter = new ArrowShooter(data, arrowPoolManager, effectPoolManager, parameters.PlayerShootParameters, parameters.PlayerAnimationParameters, playerAnimation);
        playerComponents.Add(arrowShooter);

        foreach (var component in playerComponents)
        {
            OnDamaged += component.OnDamaged;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DebugMessenger.NullCheckError(parameters) ||
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

        data.IsGamePadConnected = false;
        foreach (var device in devices)
        {
            if (device is Gamepad)
            {
                //デバイスがゲームパッド(コントローラー)の時だけ処理
                Gamepad gamepad = device as Gamepad;
                Debug.Log($"Ditect Contoroller: {gamepad.displayName}");
                data.IsGamePadConnected = true;
                break;
            }
        }

#if UNITY_EDITOR
        // ゲームパッドが検出されたかをログへ出力
        string gamepadExist = data.IsGamePadConnected ? "GamePad" : "KeyBoard and Mouse";
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
            OnSelectItem
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

    #region コルーチン

    private IEnumerator RigidCoroutine()
    {
        yield return new WaitForSeconds(parameters.PlayerAnimationParameters.DamagedRigidTime);
        if(data.State == State.Damaged)
        {
            data.ChangeState(State.Idle);
        }
    }

    #endregion
}
