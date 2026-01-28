using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
public class UsedItem : MonoBehaviour, IPooledObject<UsedItem>
{

    [SerializeField] private ItemDataBase dataBase;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator animator;

    private ItemData data;

    private Coroutine durationCoroutine = null;
    private Coroutine ringBellCoroutine = null;

    private ExplosionPoolManager explosionPool;

    int noticeNum = 0;

    #region ObjectPool
    private IObjectPool<UsedItem> pool;

    public IObjectPool<UsedItem> ObjectPool { set { pool = value; } }


    public void Initialize()
    {
    }

    public void Initialize(ItemData data)
    {
        this.data = data;
        animator.runtimeAnimatorController = data.Controller;

        if (data.ItemType == ItemData.Type.Barrier)
        {
            spriteRenderer.sortingLayerName = "Effects";
        }
        else
        {
            spriteRenderer.sortingLayerName = "OnFloor";
        }


        durationCoroutine = StartCoroutine(DurationCoroutine(data.Duration));
        switch (data.ItemType)
        {
            case ItemData.Type.Attract:
                ringBellCoroutine = StartCoroutine(RingBellCoroutine(dataBase.Interval));
                break;
            case ItemData.Type.Landmines:
                break;
            case ItemData.Type.Barrier:
                StartCoroutine(SeemsToDisappearCoroutine(dataBase.BlinkTime));
                break;
        }
    }

    public void Deactivate()
    {
        switch (data.ItemType)
        {
            case ItemData.Type.Attract:
                StopCoroutine(ringBellCoroutine);
                break;
            case ItemData.Type.Landmines:
                explosionPool.Explode(dataBase.ExplosionPower, transform.position, dataBase.ExplosionScale);
                break;
            case ItemData.Type.Barrier:
                break;
        }
        durationCoroutine = null;
        pool.Release(this);
    }

    public void OnCreated(ExplosionPoolManager pool)
    {
        explosionPool = pool;
    }

    #endregion

    void Awake()
    {
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag( "Enemy")) { return; }
        if (data.ItemType == ItemData.Type.Landmines)
        {
            noticeNum++;
            if (noticeNum >= dataBase.NumOfIgnit)
            {
                StopCoroutine(durationCoroutine);
                Deactivate();
            }
            return;
        }

        if (!collision.TryGetComponent<Enemy>(out var enemy)) { return; }

        if (data.ItemType == ItemData.Type.Barrier)
        {
            // TODO: ’e‚­‰¹‚Ì’Ç‰Á(?)
            enemy.AddForce((collision.transform.position - transform.position).normalized, dataBase.KnockPower);
        }
        else if (data.ItemType == ItemData.Type.Attract)
        {
            enemy.AddForce((transform.position - collision.transform.position).normalized, dataBase.VacuumPower);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }
        if (data.ItemType == ItemData.Type.Barrier &&
            collision.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.AddForce((collision.transform.position - transform.position).normalized, dataBase.PushPower, ForceMode2D.Force);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }
        if (data.ItemType == ItemData.Type.Landmines)
        {
            noticeNum--;
        }
    }


    private IEnumerator DurationCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }

    private IEnumerator RingBellCoroutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            // TODO: ƒxƒ‹‚ð–Â‚ç‚·‰¹‚ð’Ç‰Á
            animator.SetTrigger("RingBell");
        }
    }

    private IEnumerator SeemsToDisappearCoroutine(float duration)
    {
        yield return new WaitForSeconds(Mathf.Max(data.Duration - duration, 0));
        animator.SetTrigger("SoonDisappear");
    }
}
