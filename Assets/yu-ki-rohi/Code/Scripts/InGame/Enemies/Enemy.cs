using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// HACK：エネミー毎の動きはそれぞれ別のクラスに委譲したい

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(Animator))]
public class Enemy : MonoBehaviour, IPooledObject<Enemy>, IDamageable
{
    public enum Type
    {
        GoToCore,
        ChasePlayer
    }

    [Flags]
    public enum Drops
    {
        None = 0,
        HeartEnergy = 1 << 0,
        LoveScore = 1 << 1,
        Bell = 1 << 2,
        Pen = 1 << 3,
        Sphere = 1 << 4,
    }

    private IObjectPool<Enemy> pool;
    protected List<IUpdatable> enemyComponents = new List<IUpdatable>();
    protected EnemyCommonData commonData;

    private Coroutine anxietyEffectGenerator;

    protected event Action OnDie;
    protected event Action OnAttack;
    protected event Action OnMove;

    private bool isBlockedHoldingHands = false;

    private EnemyIndividualData individualData;
    private PoolsEnemyUse pools = new PoolsEnemyUse();

    // HACK: 個々に持たせるのはイマイチな気がする
    private DefeatNumViewer defeatNumViewer;

    public IObjectPool<Enemy> ObjectPool { set { pool = value; } }

    public bool IsAttacking
    { 
        get
        {
            if(individualData.HeartCore != null)
            {
                return true;
            }
            return false;
        }
            
    }

    public HeartCore HeartCore { get { return individualData.HeartCore; } }
    public Enemy HoldingHandsEnemy { get { return individualData.HoldingHandsEnemy; } }

    public Vector3 AnxietyEffectPos { get { return transform.position + (Vector3)individualData.BasicData.AxietyEffectOffset; } }

    public int Strength { get { return individualData.BasicData.Strength; } }

    public void ConnectedHands()
    {
        individualData.ConcatenatingNum++;
    }

    public void DisconnectedHands()
    {
        individualData.ConcatenatingNum--;
    }


    public void OnCreate(EnemyCommonData commonData, PoolsEnemyUse pools, DefeatNumViewer defeatNumViewer)
    {
        this.commonData = commonData;
        this.pools = pools;
        this.defeatNumViewer = defeatNumViewer;

        DebugMessenger.NullCheckWarning(this.commonData);
        DebugMessenger.NullCheckWarning(pools.ExplosionPool, "It won't Explode");
    }

    public void Initialize(Vector3 position, Transform target, EnemyData data)
    {
        if (DebugMessenger.NullCheckError(data)) { Deactivate(); return; }

        transform.position = position;

        // アニメーション
        individualData.Animator.runtimeAnimatorController = data.Controller;

        // HACK: ここの初期化の場合分けはもっと上手くまとめたい
        if (data.Type == Type.GoToCore)
        {
            // LayerMask.NameToLayerを使う方が安全だが、一旦直接id指定
            // 7: PassThroughStageAndEnemy
            gameObject.layer = 7;

            // 動き
            var movement = new EnemyMovementToHeartCoreByAddForce(transform, target, individualData);
            OnAttack += movement.OnAttack;
            OnMove += movement.OnMove;
            OnDie += movement.OnDie;
            enemyComponents.Add(movement);

            // アニメーション
            var animationController = new EnemyAnimationController(individualData.Animator);
            OnAttack += animationController.OnAttack;
            OnMove += animationController.OnMove;
            OnDie += animationController.OnDie;
        }
        else if(data.Type == Type.ChasePlayer)
        {
            // 9: PassThroughStageAndEnemy
            gameObject.layer = 9;

            // 動き
            var movement = new EnemyMovementChasePlayer(transform, target, individualData);
            OnAttack += movement.OnAttack;
            OnMove += movement.OnMove;
            OnDie += movement.OnDie;
            enemyComponents.Add(movement);

        }


        individualData.BasicData = data;
        transform.localScale = new Vector3(data.Scale,data.Scale, 1.0f);

        individualData.CurrentHitPoint = data.MaxHitPoint;
        individualData.ConcatenatingNum = 0;
    }

    public void Initialize()
    {

    }

    public virtual void Deactivate()
    {
        OnAttack = null; 
        OnMove = null;
        OnDie = null;
        enemyComponents.Clear();
        pool?.Release(this);
    }

    public void AddForce(Vector3 dir, float power, ForceMode2D forceMode2D = ForceMode2D.Impulse)
    {
        individualData.Rigidbody.AddForce(dir * power, forceMode2D);
        if(HeartCore != null || HoldingHandsEnemy != null)
        {
            FinishAttack();
        }
    }

    void Awake()
    {
        individualData = new EnemyIndividualData();
        individualData.Animator = GetComponent<Animator>();
        individualData.Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        foreach (var enemyComoponent in enemyComponents)
        {
            enemyComoponent.Start();
        }
    }

    void FixedUpdate()
    {
        foreach (var enemyComoponent in enemyComponents)
        {
            enemyComoponent.FixedUpdate(Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        foreach (var enemyComoponent in enemyComponents)
        {
            enemyComoponent.Update(Time.deltaTime);
        }
    }

    public void TakeDamage(int attack, DamageType damageType)
    {
        // 既に体力がない場合は判定を行わない
        if (individualData.CurrentHitPoint <= 0) { return; }
        // 通常攻撃を受けた場合はヒットエフェクトを出す
        if (damageType == DamageType.Piercing &&
            DebugMessenger.NullCheckWarning(pools.EffectPool) == false)
        {
            var data = individualData.BasicData;
            var position = transform.position + new Vector3(data.AxietyEffectOffset.x, data.AxietyEffectOffset.y, 0.0f);
            pools.EffectPool.PlayEffect(position, EffectData.EffectType.HitEffect);

            // TODO: 通常被弾音の再生
            AudioManager.Instance.PlaySEById(SEName.EnemyDamage);
        }
        // 体力を減らす
        individualData.CurrentHitPoint -= attack;
        if(individualData.CurrentHitPoint <= 0)
        {
            Die(damageType);
        }
    }

    private void Die(DamageType damageType)
    {
        if(DebugMessenger.NullCheck(anxietyEffectGenerator) == false)
        {
            StopCoroutine(anxietyEffectGenerator);
        }

        OnDie?.Invoke();

        

        if(DebugMessenger.NullCheckError(individualData)) { Deactivate(); return; }
        
        individualData.HeartCore?.ReduceEnemyCount();
        individualData.HoldingHandsEnemy?.DisconnectedHands();

        individualData.HeartCore = null;
        individualData.HoldingHandsEnemy = null;


        if (DebugMessenger.NullCheckError(commonData)) { Deactivate(); return; }

       
        switch (damageType)
        {
            case DamageType.Piercing:
                Invoke("Disapear", commonData.DelayToDisapeear);
                break;
            case DamageType.Explosion:
                Invoke("Explode", commonData.DelayToExplosion);
                break;
        }
    }

    // Invokeで起動
    protected void Disapear()
    {
        ItemDrop(individualData.BasicData.DropsNomal);
        Deactivate();
    }

    // Invokeで起動
    protected void Explode()
    {
        if(DebugMessenger.NullCheckError(pools.ExplosionPool) == false)
        {
            pools.ExplosionPool.Explode(individualData.BasicData.ExplosionPower, transform.position, individualData.BasicData.ExplosionScale);
        }
        ItemDrop(individualData.BasicData.DropsExplosion);
        CountDefeatEnemy();
        Deactivate();
    }

    protected void CheckHoldingHands()
    {
        // 攻撃中のみ行う処理
        if (individualData.HeartCore != null &&
            individualData.HoldingHandsEnemy != null)
        {
            // コアに繋いでいるエネミーが消えていたら解除
            if(individualData.HoldingHandsEnemy.IsAttacking == false)
            {
                DebugMessenger.Log("Holding Enemy has gone");
                FinishAttack();
                return;
            }

            // 自分の後ろに誰かが繋がったら自身は攻撃を行わない
            if( anxietyEffectGenerator != null && 
                individualData.ConcatenatingNum > 0)
            {
                StopCoroutine(anxietyEffectGenerator);
                anxietyEffectGenerator = null;
            }
            // 自身の後ろに誰もつながっていなければ攻撃開始
            else if (anxietyEffectGenerator == null &&
                individualData.ConcatenatingNum == 0)
            {
                anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");
            }
        }
    }

    protected void FinishAttack()
    {
        individualData.HeartCore?.ReduceEnemyCount();
        individualData.HoldingHandsEnemy?.DisconnectedHands();
        individualData.HeartCore = null;
        individualData.HoldingHandsEnemy = null;
        OnMove?.Invoke();
        if (anxietyEffectGenerator != null)
        {
            StopCoroutine(anxietyEffectGenerator);
            anxietyEffectGenerator = null;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        AttackHeartCore(collision);
        if (individualData.BasicData.Type == Type.ChasePlayer &&
            collision.gameObject.tag == "Player" &&
            collision.TryGetComponent<IDamageable>(out var player))
        {
            player.TakeDamage(individualData.BasicData.Power, DamageType.Scaring);
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        AttackHeartCore(collision);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Player")  &&
            collision.collider.TryGetComponent<IDamageable>(out var player))
        {
            player.TakeDamage(individualData.BasicData.Power, DamageType.Scaring);
        }
    }

    private void AttackHeartCore(Collider2D collision)
    {
        if (individualData.BasicData.Type != Type.GoToCore || 
            individualData.CurrentHitPoint <= 0 ||
            IsAttacking) { return; }

        // タグが"HeartCore"ならば<HeartCore>コンポーネントを取得し、近づいたことをコアへ通知
        if (collision.gameObject.tag =="HeartCore" &&
           collision.TryGetComponent<HeartCore>(out var heartCore))
        {
            this.individualData.HeartCore = heartCore;
            heartCore.AddEnemyCount();
            OnAttack?.Invoke();
            anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");

        }

        // 攻撃中のエネミーに近づいたら加勢
        /*
            NOTE:
                条件がやたら複雑なのは
                「コアにつながっていない状態なのに、エネミー同士のつながりが循環して攻撃状態が解除されない」
                という事態をさけるため

                確実に上の事態を避けるのには、
                再帰呼び出しして「循環していないか」と「コアにつながっているか」を確認する
                という手が考えられるが、攻撃中の全てのエネミーが毎フレーム行う処理であることから、
                それなりに時間がかさむかもしれないという予測により採用していない
        */
        else if (isBlockedHoldingHands == false &&
                 collision.gameObject.tag == "Enemy" &&
                 Vector3.Dot(collision.transform.position - transform.position, individualData.MoveDir ) > 0 && // 進行方向側に限定
                 collision.TryGetComponent<Enemy>(out var enemy) &&
                 enemy.IsAttacking &&
                 enemy.HoldingHandsEnemy != this)
        {

            individualData.HoldingHandsEnemy = enemy;
            this.individualData.HeartCore = enemy.HeartCore;
            this.individualData.HeartCore.AddEnemyCount();
            OnAttack?.Invoke();
            enemy.ConnectedHands();
            anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");

            BlockHoldingHands();
        }

    }

    private void BlockHoldingHands()
    {
        // 循環してしまう問題への対処
        // 30f間があれば大丈夫やろの精神
        isBlockedHoldingHands = true;
        Invoke("UnlockHoldingHands", 0.5f);
    }

    private void UnlockHoldingHands()
    {
        isBlockedHoldingHands = false;
    }

    private void ItemDrop(Drops drops)
    {
        if((drops & Drops.HeartEnergy) != 0 &&
            DebugMessenger.NullCheckError(pools.HeartEnergyPool) == false)
        {
            pools.HeartEnergyPool.GenerateHeart(individualData.BasicData.Enegy, transform.position);
        }
        if ((drops & Drops.LoveScore) != 0 &&
             DebugMessenger.NullCheckError(pools.EnemyDropsPool) == false)
        {
            pools.EnemyDropsPool.DropEnergy(individualData.BasicData.BaseScore, transform.position);
        }
        // HACK: idの指定方法は要検討
        if ((drops & Drops.Bell) != 0)
        {
            pools.DropItemPool.DropItem(0, transform.position);
        }
        if ((drops & Drops.Pen) != 0)
        {
            pools.DropItemPool.DropItem(1, transform.position);
        }
        if ((drops & Drops.Sphere) != 0)
        {
            pools.DropItemPool.DropItem(2, transform.position);
        }
    }

    /*
        HACK:
            簡略化のために一旦ここで行っているができれば他のところが請け負うべき内容
            爆発時しかカウントしないためネーミングもイマイチ
     */
    private void CountDefeatEnemy()
    {
        if (DebugMessenger.NullCheckError(defeatNumViewer)) { return; }
        defeatNumViewer.OnDefeatEnemy();
    }

    private IEnumerator GenerateAnxietyEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(commonData.AnxietyPropagateInterval);
            pools.AnxietyPropagationEffectPool?.AnxietyPopagate(individualData.HoldingHandsEnemy, AnxietyEffectPos, individualData.BasicData.Strength);
        }
    }
}
