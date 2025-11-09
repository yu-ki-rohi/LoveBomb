using Cysharp.Threading.Tasks;
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
    private EnemyData data;
    private EnemyCommonData commonData;
    private ExplosionPoolManager explosionPool;
    private AnxietyPropagationEffectPoolManager anxietyPropagationEffectPool;
    private EnemyDropsPoolManager enemyDropsPool;

    private Coroutine anxietyEffectGenerator;

    protected HeartCore heartCore;
    protected Enemy holdingHandsEnemy;

    protected event Action OnDie;
    protected event Action OnAttack;
    protected event Action OnMove;

    private int currentHitPoint = 0;

    public IObjectPool<Enemy> ObjectPool { set { pool = value; } }

    public bool IsAttacking
    { 
        get
        {
            if(heartCore != null)
            {
                return true;
            }
            return false;
        }
            
    }

    public HeartCore HeartCore { get { return heartCore; } }
    public Enemy HoldingHandsEnemy { get { return holdingHandsEnemy; } }

    public Vector3 AnxietyEffectPos { get { return transform.position + (Vector3)data.AxietyEffectOffset; } }

    public void OnCreate(EnemyCommonData commonData, ExplosionPoolManager explosionPoolManager, AnxietyPropagationEffectPoolManager anxietyPropagationEffectPoolManager, EnemyDropsPoolManager enemyDropsManager)
    {
        this.commonData = commonData;
        explosionPool = explosionPoolManager;
        anxietyPropagationEffectPool = anxietyPropagationEffectPoolManager;
        enemyDropsPool = enemyDropsManager;

        DebugMessenger.NullCheckWarning(this.commonData);
        DebugMessenger.NullCheckWarning(explosionPool, "It won't Explode");
    }

    public void Initialize(Vector3 position, Transform target, EnemyData data)
    {
        if (DebugMessenger.NullCheckError(data)) { Deactivate(); return; }

        transform.position = position;

        // 動き
        var movement = new EnemyMovementToHeartCoreByAddForce(transform, GetComponent<Rigidbody2D>(), target, data);
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

        this.data = data;
        transform.localScale = new Vector3(data.Scale,data.Scale, 1.0f);

        currentHitPoint = data.MaxHitPoint;
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
        if(currentHitPoint <= 0) { return; }
        currentHitPoint -= attack;
        if(currentHitPoint <= 0)
        {
            Die(damageType);
        }
    }

    private void Die(DamageType damageType)
    {
        if(!DebugMessenger.NullCheckWarning(anxietyEffectGenerator))
        {
            StopCoroutine(anxietyEffectGenerator);
        }

        OnDie?.Invoke();
        if(DebugMessenger.NullCheckError(commonData)) { Deactivate(); return; }

        if(heartCore != null)
        {
            heartCore.ReduceEnemyCount();
        }

        heartCore = null;
        holdingHandsEnemy = null;

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
    protected void Disapear()
    {
        
        Deactivate();
    }

    protected void Explode()
    {
        if(DebugMessenger.NullCheckError(explosionPool) == false)
        {
            explosionPool.Explode(data.ExplosionPower, transform.position, data.ExplosionScale);
        }
        if(DebugMessenger.NullCheckError(enemyDropsPool) == false)
        {
            enemyDropsPool.DropEnergy(data.BaseScore, transform.position);
        }
        Deactivate();
    }

    protected void CheckHoldingHands()
    {
        if (heartCore != null &&
            holdingHandsEnemy != null &&
            holdingHandsEnemy.IsAttacking == false)
        {
            Debug.Log("Holding Enemy has gone");
            heartCore.ReduceEnemyCount();
            heartCore = null;
            holdingHandsEnemy = null;
            OnMove?.Invoke();

            StopCoroutine(anxietyEffectGenerator);
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
        if (currentHitPoint <= 0 || IsAttacking) { return; }
        // タグが"HeartCore"ならば<HeartCore>コンポーネントを取得し、近づいたことをコアへ通知
        if (collision.gameObject.tag =="HeartCore" &&
           collision.TryGetComponent<HeartCore>(out var heartCore))
        {
            this.heartCore = heartCore;
            heartCore.AddEnemyCount();
            OnAttack?.Invoke();
            anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");
        }
        // 攻撃中のエネミーに近づいたら加勢
        else if (collision.gameObject.tag == "Enemy" &&
                collision.TryGetComponent<Enemy>(out var enemy) &&
                enemy.IsAttacking &&
                enemy.HoldingHandsEnemy != this)
        {
            holdingHandsEnemy = enemy;
            this.heartCore = enemy.HeartCore;
            this.heartCore.AddEnemyCount();
            OnAttack?.Invoke();
            anxietyEffectGenerator = StartCoroutine("GenerateAnxietyEffect");
        }

    }

    private IEnumerator GenerateAnxietyEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(data.AnxietyPropagateInterval);
            anxietyPropagationEffectPool?.AnxietyPopagate(holdingHandsEnemy, AnxietyEffectPos, data.Strength);
        }
    }
}
