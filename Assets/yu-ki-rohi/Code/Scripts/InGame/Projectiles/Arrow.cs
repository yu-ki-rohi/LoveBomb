using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SpriteRenderer))]
public class Arrow : ProjectileBase, IPooledObject<Arrow>
{
    // Œã‚ÅObjectPool‚Ì•û‚É•Ï‚¦‚é
    [SerializeField] private GameObject explosionPrefab;
    public enum Type
    {
        Normal,
        Explosion
    }
    private Type type;
    private ArrowParams parameters;
    private PlayerShootParameters parametersOfArrows;

    private ExplosionPoolManager explosionPoolManager;

    private SpriteRenderer spriteRenderer;

    private IObjectPool<Arrow> objectPool;

    private bool isAttackable = true;

    public IObjectPool<Arrow> ObjectPool { set => objectPool = value; }

    public PlayerShootParameters ParametersOfParameters { set => parametersOfArrows = value; }

    public ExplosionPoolManager ExplosionPoolManager { set => explosionPoolManager = value; }

    public void Initialize(Type type)
    {
        Initialize();
        this.type = type;
        if(parametersOfArrows == null)
        {
            Debug.LogError("Where is Parameter List ?");
            return;
        }

        foreach (var arrowParams in parametersOfArrows.Arrows)
        {
            if(arrowParams.ArrowType == type)
            {
                parameters = arrowParams;
                break;
            }
        }

        if(parameters == null || parameters.ArrowType != type)
        {
            Debug.LogError("Arrow Type " + type + " is Not Found");
            return;
        }
        isAttackable = true;
        spriteRenderer.sprite = parameters.Image;
    }

    public override void Deactivate()
    {
        if(type == Type.Explosion)
        {
            Explode();
        }

        if(objectPool == null)
        {
            Debug.LogAssertion("ObjectPool is Null");
            Destroy(gameObject); return;
        }
        objectPool.Release(this);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GoStraight(parameters.Speed, parameters.EffectiveRange);
    }

    private void Explode()
    {
        if (explosionPoolManager == null)
        {
            Debug.LogError("ExplosionPoolManager is Null");
            return;
        }
        explosionPoolManager.Explode(parameters.Power, transform.position, parametersOfArrows.ExplosionScale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isAttackable == false) { return; }
        if(collision.CompareTag("Enemy"))
        {

            if(type == Type.Normal)
            {
                var damageable = collision.GetComponent<IDamageable>();
                if (DebugMessenger.NullCheckError(damageable)) { Deactivate(); return; }
                damageable.TakeDamage(parameters.Power, DamageType.Piercing, 1.0f);
            }
            isAttackable = false;
            Deactivate();
        }
    }
}
