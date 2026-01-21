using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class DropItemBase : MonoBehaviour
{
    [SerializeField] protected ItemData data;
    private ItemCommonData commonData;

    protected SpriteRenderer spriteRenderer;

    protected Player target;

    public Player Target { set { target = value; } }

    public void OnCreated(ItemCommonData commonData)
    {
        this.commonData = commonData;
    }

    public virtual void Initialize()
    {
        target = null;
        StartCoroutine(LifeTimeCoroutine());
    }

    public abstract void Deactivate();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) { return; }

        Vector3 dir = target.transform.position - transform.position;
        transform.position += commonData.Speed * Time.deltaTime * dir.normalized;

        // 要検討：判定のボーダーラインをどうするか
        if (dir.sqrMagnitude < 1.0f)
        {
            StopAllCoroutines();
            spriteRenderer.enabled = true;
            OnReachPlayer();
            Deactivate();
        }
    }

    protected abstract void OnReachPlayer();

    private IEnumerator LifeTimeCoroutine()
    {
        yield return new WaitForSeconds(Mathf.Max(0, data.LifeTime - commonData.StartBlinkRemainingTimeFirstPhase));
        Coroutine blinkCoroutine = StartCoroutine(BlinkCoroutine(commonData.VisibleTimeFirstPhase,commonData.InvisibleTimeFirstPhase));

        yield return new WaitForSeconds(Mathf.Max(0, commonData.StartBlinkRemainingTimeFirstPhase - commonData.StartBlinkRemainingTimeSecondPhase));
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkCoroutine(commonData.VisibleTimeSecondPhase, commonData.InvisibleTimeSecondPhase));
        
        yield return new WaitForSeconds(commonData.StartBlinkRemainingTimeSecondPhase);
        StopCoroutine(blinkCoroutine);
        Deactivate();
    }

    private IEnumerator BlinkCoroutine(float visibleTime, float invisibleTime)
    {
        while (true)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(invisibleTime);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(visibleTime);
        }
    }
}
