
/// <summary>
/// SEの名前のEnum
/// </summary>
internal enum SEName
{
    #region 弓矢関連

    ArrowShot,//矢を射る音
    BowStringRelease, //弦を弾く音
    Explosion,//爆発音
    ArrowHitStage,//矢がステージに当たって消えたときの音

    #endregion

    #region チャージ

    ChargeFinish,//チャージ完了音
    ChargeLoop,//チャージ中の音

    #endregion

    #region 被弾関連

    Damage,//通常被弾音
    EnemyDamage,//敵被弾音

    #endregion

    #region アイテム使用時

    BellRing,//ベルを鳴らす音
    UseSphere, //スフィア使用時
    UseFeatherPenWrite,//羽ペン使用時
    ItemOutOfStock,//アイテム使用不可音(所持数0の時)
    #endregion

    #region アイテム入手
    PickupHeart,//ハート入手音
    PickupBell,//ベル入手音
    PickupPen,//羽ペン入手音
    PickupSphere,//スフィア入手音
    #endregion

    #region スコア
    AddScore,//スコア加算音
    DecreaseScore,//スコア減少音
    #endregion

    #region ポーズ
    PauseOn,//ポーズ起動音
    PauseOff, //ポーズ解除
    #endregion

    #region UI

    Click, //クリック時のSE
    MouseOver,//マウスオーバー時のSE（？）

    #endregion

    #region タイムアップ関連


    TimeUp,//タイムアップのSE

    #endregion

    BarrierBlock, //バリアが敵を弾く音
    CoreEnemyWarning,//コア接近エネミー一定値以上通知音
    ClearScoreReached,//スコア一定値を知らせる音(特にクリアボーダーのところ)
    ComboThresholdReached,//コンボ数一定数達成音
    DashMove,//ダッシュ(回避)音
    GameStart,//ゲームスタート

    #region 勝敗判定

    PerfectClear,//完全勝利時の音
    PerfectLose,//完全敗北時の音


    #endregion

    #region こうもり

    BatChargePrepare,//こうもり型の突進準備音
    BatCharge,//こうもり型の突進音
    BatFlap//こうもり型の羽ばたき音

    #endregion

}


