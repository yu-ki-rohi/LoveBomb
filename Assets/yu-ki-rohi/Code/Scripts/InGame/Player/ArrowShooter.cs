using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


// チャージ処理実装のためにUniTaskを導入
// 参考資料:https://shibuya24.info/entry/unity-start-unitask
// ※UniTask周りのソースはChat GPTで生成したコードから抜粋

public class ArrowShooter : NormalPlayerComponent, IShootable
{
    private Transform transform;
    private ArrowPoolManager poolManager;
    private EffectPoolManager effectPoolManager;
    private PlayerShootParameters parameters;
    private PlayerAnimationParameters animParameters;

    private PlayerAnimation playerAnimation;

    private Arrow.Type type = Arrow.Type.Normal;
    private CancellationTokenSource chargeCts;

    private PooledEffect chargeEffect;

    private bool isPreparedToShoot = false;

    public ArrowShooter(PlayerIndividualData player, ArrowPoolManager poolManager, EffectPoolManager effectPoolManager, PlayerShootParameters parameters, PlayerAnimationParameters animParameters, PlayerAnimation playerAnimation) :
        base(player)
    {
        transform = player.Transform;
        this.poolManager = poolManager;
        this.effectPoolManager = effectPoolManager;
        this.parameters = parameters;
        this.animParameters = animParameters;
        this.playerAnimation = playerAnimation;
    }

    public override void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (player.IsIdle == false) { return; }

            player.ChangeState(Player.State.Aim);
            chargeCts = new CancellationTokenSource();
            ChargeAsync(chargeCts.Token).Forget(); // チャージ処理開始

        }
        
        else if(context.canceled)
        {
            if (player.State != Player.State.Aim) { return; }
            isPreparedToShoot = true;
        }
    }

    public override void OnMove(Vector2 input)
    {
        if( player.IsGamePadConnected == false ||
            player.IsRStickInput == true) { return; }
        if(input != Vector2.zero )
        {
            player.ShootDir = input;
        }
    }

    public override void OnShootDir(Vector2 input)
    {
        if(player.IsGamePadConnected == false) { return; }
        if(input == Vector2.zero ) 
        { 
            player.IsRStickInput = false;
        }
        else
        {
            player.IsRStickInput = true;
            player.ShootDir = input;
        }
    }

    public override void OnDamaged()
    {
        chargeEffect?.Deactivate();
        chargeEffect = null;
        type = Arrow.Type.Normal;

        chargeCts?.Cancel();
        chargeCts?.Dispose();
        chargeCts = null;
    }

    public override void OnDisable()
    {
        // オブジェクト破棄時に安全にキャンセル
        chargeCts?.Cancel();
        chargeCts?.Dispose();
        chargeCts = null;
    }

    public override void Start()
    {

    }

    public override void Update(float deltaTime)
    {
        if(chargeEffect != null)
        {
            chargeEffect.transform.position = player.Transform.position + (Vector3)parameters.ShootPosAdjust;
        }

        if (player.State == Player.State.Aim &&
            player.IsGamePadConnected == false)
        {
            // マウスポインターの座標を取得し、ワールド座標系に変換
            Vector2 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            // 発射方向決定
            player.ShootDir = (mousePosition - (Vector2)transform.position).normalized;
        }
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {

    }

    public void Shoot()
    {
        if(player.ConsumeHeartEnergy(poolManager.GetCost(type)))
        {
            if(player.IsGamePadConnected == false)
            {
                // マウスポインターの座標を取得し、ワールド座標系に変換
                Vector2 mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                // 発射方向決定
                player.ShootDir = (mousePosition - (Vector2)transform.position).normalized;
            }

            // チャージせずに射る場合はエフェクトを消去
            if(chargeEffect != null)
            {
                chargeEffect.Deactivate();
                chargeEffect = null;
            }

            // 矢の生成位置を決定
            Vector3 firePosition = transform.position + (Vector3)player.ShootDir * parameters.ShootPositionDistance + (Vector3)parameters.ShootPosAdjust;
            // オブジェクトプールから取り出し
            poolManager?.Shoot(firePosition, player.ShootDir, type);

            // TODO: 矢を射る音
            AudioManager.Instance.PlaySEById(SEName.ArrowShot);
        }
        else
        {
            AudioManager.Instance.PlaySEById(SEName.BowStringRelease);
            // TODO: 弦を弾く音
        }
        // 矢を溜め無し状態に戻す
        type = Arrow.Type.Normal;

        player?.ChangeState(Player.State.Shoot);
        playerAnimation?.SetShootStage((int)PlayerAnimation.ShootStage.FALLOW_THROUGH);
        isPreparedToShoot = false;
    }

    private async UniTaskVoid ChargeAsync(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(animParameters.LeadInTime), cancellationToken: token);
            playerAnimation?.SetShootStage((int)PlayerAnimation.ShootStage.STANDBY);

            int cost = poolManager.GetCost(Arrow.Type.Explosion);
            while (player.HeartEnergy < cost)
            {
                if (isPreparedToShoot)
                {
                    Shoot();
                    FollowThroughAsync(token).Forget();
                    return;
                }
                // フレーム待ち（Updateタイミング）
                await UniTask.Yield(PlayerLoopTiming.Update, token);

            }

            float currentCharge = 0f;
            chargeEffect = effectPoolManager.PlayEffect(player.Transform.position + (Vector3)parameters.ShootPosAdjust, EffectData.EffectType.Charge, 1.0f / parameters.ChargeTime);
            
            while (currentCharge < parameters.ChargeTime)
            {
                if (isPreparedToShoot)
                {
                    Shoot();
                    AudioManager.Instance.PlaySEById(SEName.ChargeLoop);


                    FollowThroughAsync(token).Forget();
                    return;
                }
                // フレーム待ち（Updateタイミング）
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                // 経過時間加算
                currentCharge += Time.deltaTime;

            }

            chargeEffect = null;
            if (player.ConsumeHeartEnergy(poolManager.GetCost(type)))
            {
                // TODO: チャージ完了音
                type = Arrow.Type.Explosion;
                AudioManager.Instance.PlaySEById(SEName.ChargeFinish);
                DebugMessenger.Log("Fully Charged!");
                // HACK: アニメーション速度の指定の仕方
                effectPoolManager.PlayEffect(player.Transform.position + (Vector3)parameters.ShootPosAdjust, EffectData.EffectType.ChargeEnd, 1.0f);
            }
            while (isPreparedToShoot == false) 
            {
                // フレーム待ち（Updateタイミング）
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            Shoot();
            FollowThroughAsync(token).Forget();
        }
        catch (OperationCanceledException)
        {
            DebugMessenger.Log("Charge canceled");
            // CTSの破棄 ヌルチェック + 実行
            chargeCts?.Dispose();
            chargeCts = null;
        }
    }

    private async UniTaskVoid FollowThroughAsync(CancellationToken token)
    {
        try
        {
            float followThroughTime = 0.0f;
            while (followThroughTime < animParameters.FollowThroughTime)
            {
                // フレーム待ち（Updateタイミング）
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                // 経過時間加算
                followThroughTime += Time.deltaTime;

            }

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Follow Through canceled");
        }
        finally
        {
            player?.ChangeState(Player.State.Idle);
            playerAnimation?.SetShootStage((int)PlayerAnimation.ShootStage.IDLE);
            // CTSの破棄 ヌルチェック + 実行
            chargeCts?.Dispose();
            chargeCts = null;
        }
    }
}
