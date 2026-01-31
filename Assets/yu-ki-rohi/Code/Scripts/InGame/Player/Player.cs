using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private UsedItemPoolManager usedItemPoolManager;

    [SerializeField] private ItemDataBase itemData;

    // 一旦プレイヤーから操作
    [SerializeField] private Image heartGauge;
    [SerializeField] private List<Image> selectedItems;
    [SerializeField] private List<TextMeshProUGUI> itemNumTexts;

    #endregion

    #region　その他のフィールド

    private PlayerIndividualData data;

    private List<NormalPlayerComponent> playerComponents = new();

    private event Action OnDamaged;

    private float itemSelectLockTimer = 0.0f;

    private int itemIndex = 0;

    private InputAction move;
    private InputAction shoot;
    private InputAction shootDir;
    private InputAction dash;
    private InputAction selectItem;
    private InputAction useItem;

    private bool canMove = false;

    #endregion

    #region プロパティ

    public EffectPoolManager EffectPoolManager { set => effectPoolManager = value; }
    public ExplosionPoolManager ExplosionPoolManager { set => arrowPoolManager.ExplosionPoolManager = value; }

    public bool CanMove { set => canMove = value; }

    #endregion

    #region  Player Input に登録するメソッド
    private void OnMove(InputAction.CallbackContext context)
    {
        
        // 入力情報の受け取り
        Vector2 input = context.ReadValue<Vector2>();
        if (canMove == false) 
        {
            input = Vector2.zero;
        }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnMove(input);
        }

    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (canMove == false) { return; }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnShoot(context);
        }

    }

    private void OnShootDir(InputAction.CallbackContext context)
    {
        if (canMove == false) { return; }

        //HACK:要リファクタリング

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
        if (canMove == false) { return; }

        foreach (var playerComoponent in playerComponents)
        {
            playerComoponent.OnDash(context);
            // ちょっとうるさすぎるので一旦抜きで
            //AudioManager.Instance.PlaySEById(SEName.DashMove);

        }
        // LayerMask.NameToLayerを使う方が安全だが、一旦直接id指定     
        // 10: PlayerInvincible
        if (gameObject.layer != 10)
        {
            gameObject.layer = 10;
            StartCoroutine(InvincibleCroutine());
        }
    }

    private void OnUseItem(InputAction.CallbackContext context)
    {
        if (canMove == false) { return; }

        if (data.State != State.Idle && data.State != State.Aim) { return; }
        
        if(context.performed)
        {
            UseItem();
        }
    }

    private void OnSelectItem(InputAction.CallbackContext context)
    {
        if (canMove == false) { return; }

        float input = context.ReadValue<float>();
        if (itemSelectLockTimer <= 0.0f && input != 0.0f)
        {
            SelectItem(input);
            itemSelectLockTimer = parameters.PlayerUseItem.SelectItemInterval;
        }
    }

    #endregion

    public void AddHeartEnergy(int energy)
    {
        data.AddHeartEnergy(energy);
        AudioManager.Instance.PlaySEById(SEName.PickupHeart);

    }

    public void AddItem(int id)
    {
        // 不正なidの場合か、所持上限をこえる場合はスキップ
        if( id < 0 || 
            id > itemData.Items.Count ||
            itemData.Items[id].NumberOfPossessions >= itemData.Items[id].MaxNum) { return; }
        itemData.Items[id].NumberOfPossessions++;
        AudioManager.Instance.PlaySEById(SEName.PickupBell);
        ReflectSelectedItemUI();
        
    }

    public void TakeDamage(int attack, DamageType damageType)
    {
        if (damageType != DamageType.Scaring ||
            data.State == State.Damaged) { return; }
        // LayerMask.NameToLayerを使う方が安全だが、一旦直接id指定     
        // 10: PlayerInvincible
        gameObject.layer = 10;
        AudioManager.Instance.PlaySEById(SEName.Damage);
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

        int length = Mathf.Min(selectedItems.Count, itemData.Items.Count);
        for(int i = 0;i<length;i++)
        {
            selectedItems[i].sprite = itemData.Items[i].Icon;
        }
        ReflectSelectedItemUI();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DebugMessenger.NullCheckError(parameters) ||
            DebugMessenger.NullCheckError(followCamera) ||
            DebugMessenger.NullCheckError(arrowPoolManager))
        { return; }

        arrowPoolManager.SetArrowParameters(parameters.PlayerShootParameters);

        // 展示会1日目の反応を見て、アイテム数の最低保証を追加
        for(int i = 0; i <itemData.Items.Count; i++)
        {
            itemData.Items[i].NumberOfPossessions = Mathf.Max(itemData.Items[i].NumberOfPossessions, 3);
        }
        ReflectSelectedItemUI();


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
        
        if (itemSelectLockTimer > 0)
        {
            itemSelectLockTimer -= Time.deltaTime;
        }

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
        else if(collision.CompareTag("Item") && 
            collision.TryGetComponent<CommonDropItem>(out var commonDropItem))
        {
            commonDropItem.Target = this;
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
        const int Length = 6;
        Action<InputAction.CallbackContext>[] actions = new Action<InputAction.CallbackContext>[Length]
        {
            OnMove,
            OnShoot,
            OnShootDir,
            OnDash,
            OnSelectItem,
            OnUseItem
        };

        InputAction[] inputActions = new InputAction[Length]
        {
            move,
            shoot,
            shootDir,
            dash,
            selectItem,
            useItem
        };

        // 登録処理
        if (enabled)
        {

            string[] actionName = new string[Length]
            {
            "Move",
            "Shoot",
            "ShootDir",
            "Dash",
            "SelectItem",
            "UseItem"
            };

            var inGame = playerInput.actions.FindActionMap("InGame");
            for (int i = 0; i < Length; i++)
            {
                inputActions[i] = inGame.FindAction(actionName[i]);
                inputActions[i].performed += actions[i];
                inputActions[i].canceled += actions[i];
            }

        }
        // 解除処理
        else
        {
            for (int i = 0; i < Length; i++)
            {
                if (DebugMessenger.NullCheckWarning(inputActions[i])) { continue; }
                inputActions[i].performed -= actions[i];
                inputActions[i].canceled -= actions[i];
            }

        }
    }

    private void SelectItem(float input)
    {
        int delta = 0;
        if(input > 0)
        {
            delta = -1;
        }
        else if(input < 0)
        {
            delta = 1;
        }
        itemIndex = LoopIndex(itemIndex, delta, itemData.Items.Count);
        ReflectSelectedItemUI();
    }

    private int LoopIndex(int currentIndex, int delta, int ArrayLength)
    {
        delta %= ArrayLength;
        int nextIndex = currentIndex + delta;
        if(nextIndex < 0)
        {
            nextIndex = ArrayLength + nextIndex;
        }
        else if(nextIndex > ArrayLength - 1)
        { 
            nextIndex = nextIndex - ArrayLength;
        }
        nextIndex %= ArrayLength;
        return nextIndex;
    }

    private void ReflectSelectedItemUI()
    {
        for(int i = 0; i < selectedItems.Count; i++)
        {

            // HACK: パラメーターの外だしなど
            if (i == itemIndex)
            {
                if(itemData.Items[i].NumberOfPossessions > 0)
                {
                    selectedItems[i].color = Color.white;
                }
                else
                {
                    selectedItems[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }
            }
            else
            {
                selectedItems[i].color = new Color(1.0f,1.0f, 1.0f, 0.2f);
            }
            itemNumTexts[i].text = "× "+ itemData.Items[i].NumberOfPossessions.ToString();
        }
    }

    private void UseItem()
    {
        ItemData item = itemData.Items[itemIndex];

        if (item.NumberOfPossessions < 1) { AudioManager.Instance.PlaySEById(SEName.ItemOutOfStock); return; }

        Vector3 position = transform.position;

        if (data.IsGamePadConnected == false)
        {
            // マウスポインターの座標を取得し、ワールド座標系に変換
            Vector2 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 dir = (Vector3)mousePosition - position;
            float distanceMax = parameters.PlayerUseItem.UseItemDistance;
            if (dir.sqrMagnitude <= distanceMax * distanceMax)
            {
                position = (Vector3)mousePosition;
            }
            else
            {
                position = position + dir.normalized * distanceMax;
            }
        }
        else
        {
            position += (Vector3)data.ShootDir * parameters.PlayerUseItem.UseItemDistance;
        }


        usedItemPoolManager.UseItem(item, position,item.Radius);

        // TODO: それぞれのアイテム使用時の効果音を再生
        switch(item.ItemType)
        {
            case ItemData.Type.Attract:
                AudioManager.Instance.PlaySEById(SEName.BellRing);

                break;
            case ItemData.Type.Barrier:
                AudioManager.Instance.PlaySEById(SEName.UseSphere);

                break;
            case ItemData.Type.Landmines:
                AudioManager.Instance.PlaySEById(SEName.UseFeatherPenWrite);
                break;

        }

        item.NumberOfPossessions--;
        ReflectSelectedItemUI();
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

    private IEnumerator InvincibleCroutine()
    {
        yield return new WaitForSeconds(parameters.PlayerMovementParameters.InvincibleTime);
        gameObject.layer = 0;
    }

    #endregion
}
