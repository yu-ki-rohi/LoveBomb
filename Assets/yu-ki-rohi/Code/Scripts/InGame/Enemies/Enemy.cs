using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// HACK：作りがイマイチ、もう少し工夫したい

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(Animator))]
public class Enemy : MonoBehaviour, IPooledObject<Enemy>, IDamageable
{
    public enum Type
    {
        GoToCore
    }

    private IObjectPool<Enemy> pool;
    protected List<IUpdatable> enemyComponents = new List<IUpdatable>();
    protected EnemyCommonData commonData;

    private Coroutine anxietyEffectGenerator;

    protected event Action OnDie;
    protected event Action OnAttack;
    protected event Action OnMove;

    private bool isBlockedHoldingHands = false;

    EnemyIndividualData individualData = new EnemyIndividualData();
    PoolsEnemyUse pools = new PoolsEnemyUse();

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


    public void OnCreate(EnemyCommonData commonData, PoolsEnemyUse pools)
    {
        this.commonData = commonData;
        this.pools = pools;

        DebugMessenger.NullCheckWarning(this.commonData);
        DebugMessenger.NullCheckWarning(pools.ExplosionPool, "It won't Explode");
    }

    public void Initialize(Vector3 position, Transform target, EnemyData data)
    {
        if (DebugMessenger.NullCheckError(data)) { Deactivate(); return; }

        transform.position = position;

        // 動き
        var movement = new EnemyMovementToHeartCoreByAddForce(transform, GetComponent<Rigidbody2D>(), target, individualData);
        OnAttack += movement.OnAttack;
        OnMove += movement.OnMove;
        OnDie += movement.OnDie;
        enemyComponents.Add(movement);

        // アニメーション
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = data.Controller;
        var animationController = new EnemyAnimationController(animator);
        OnAttack += animationController.OnAttack;
        OnMove += animationController.OnMove;
        OnDie += animationController.OnDie;

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

    void Awake()
    {

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

    public void TakeDamage(int attack, DamageType damageType, float bonus)
    {
        if(individualData.CurrentHitPoint <= 0) { return; }
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
        if(DebugMessenger.NullCheckError(pools.HeartEnergyPool) == false)
        {
            pools.HeartEnergyPool.GenerateHeart(individualData.BasicData.Enegy, transform.position);
        }
        Deactivate();
    }

    // Invokeで起動
    protected void Explode()
    {
        if(DebugMessenger.NullCheckError(pools.ExplosionPool) == false)
        {
            pools.ExplosionPool.Explode(individualData.BasicData.ExplosionPower, transform.position, individualData.BasicData.ExplosionScale);
        }
        if(DebugMessenger.NullCheckError(pools.EnemyDropsPool) == false)
        {
            pools.EnemyDropsPool.DropEnergy(individualData.BasicData.BaseScore, transform.position);
        }
        Deactivate();
    }

    protected void CheckHoldingHands()
    {
        // 攻撃中のみ行う処理
        if (individualData.HeartCore != null &&
            individualData.HoldingHandsEnemy != null)
        {
            if(individualData.HoldingHandsEnemy.IsAttacking == false)
            {
                DebugMessenger.Log("Holding Enemy has gone");
                individualData.HeartCore.ReduceEnemyCount();
                individualData.HoldingHandsEnemy.DisconnectedHands();
                individualData.HeartCore = null;
                individualData.HoldingHandsEnemy = null;
                OnMove?.Invoke();
                if(anxietyEffectGenerator != null)
                {
                    StopCoroutine(anxietyEffectGenerator);
                    anxietyEffectGenerator = null;
                }
                return;
            }

            if( anxietyEffectGenerator != null && 
                individualData.ConcatenatingNum > 0)
            {
                StopCoroutine(anxietyEffectGenerator);
                anxietyEffectGenerator = null;
            }

            else if (anxietyEffectGenerator == null &&
                individualData.ConcatenatingNum == 0)
            {
                anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        AttackHeartCore(collision);
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        AttackHeartCore(collision);
    }

    private void AttackHeartCore(Collider2D collision)
    {
        if (individualData.CurrentHitPoint <= 0 ||
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
        // 循環してしまう不具合への対処
        // 30f間があれば大丈夫やろの精神
        isBlockedHoldingHands = true;
        Invoke("UnlockHoldingHands", 0.5f);
    }

    private void UnlockHoldingHands()
    {
        isBlockedHoldingHands = false;
    }

    private IEnumerator GenerateAnxietyEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(individualData.BasicData.AnxietyPropagateInterval);
            pools.AnxietyPropagationEffectPool?.AnxietyPopagate(individualData.HoldingHandsEnemy, AnxietyEffectPos, individualData.BasicData.Strength);
        }
    }
}
