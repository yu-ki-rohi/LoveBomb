using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

// 基本的なエフェクト
[RequireComponent(typeof(Animator))]
public class PooledEffect : MonoBehaviour, IPooledObject<PooledEffect>
{
    private IObjectPool<PooledEffect> pool;
    private Animator animator;
    private bool isLoop;

    public IObjectPool<PooledEffect> ObjectPool { set =>  pool = value; }

    public void Initialize(EffectData effectData, float animSpeed)
    {
        isLoop = effectData.IsLoop;
        animator.runtimeAnimatorController = effectData.Controller;
        animator.SetFloat("AnimSpeed", animSpeed);
    }
    
    public void Initialize()
    {

    }

    public void Deactivate()
    {
        pool.Release(this);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoop == true) { return; }

        // アニメーション再生中かを取得
        // Chat GPTで生成したコードを改造して作成
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
        {
            Deactivate();
        }
    }
}
