using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class HeartEnergy : MonoBehaviour,IPooledObject<HeartEnergy>
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float lifeTime = 10.0f;
    private IObjectPool<HeartEnergy> pool;
    private Player target;

    private int energy;

    private Coroutine lifeTimerCoroutine;

    public IObjectPool<HeartEnergy> ObjectPool { set { pool = value; } }

    public Player Target { set { target = value; } }

    public void Initialize(int energy)
    {
        this.energy = energy;
        Initialize();
    }

    public void Initialize()
    {
        target = null;
        lifeTimerCoroutine = StartCoroutine(LifeTimerCoroutine());
    }

    public void Deactivate()
    {
        StopCoroutine(lifeTimerCoroutine);
        pool.Release(this);
    }

    public int GainEnergy()
    {
        Deactivate();
        return energy;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) { return; }

        Vector3 dir = target.transform.position - transform.position;
        transform.position += speed * Time.deltaTime * dir.normalized;

        // 要検討：判定のボーダーラインをどうするか
        if(dir.sqrMagnitude < 1.0f)
        {
            target.AddHeartEnergy(energy);
            Deactivate();
        }
    }

    private IEnumerator LifeTimerCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Deactivate();
    }
}
