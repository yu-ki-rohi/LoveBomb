using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private FadeComponent fade;

    #region シングルトン

    private readonly static InstanceHolder<ScreenFader> singleton = new();

    public static ScreenFader Instance()
    {
        if (singleton.IsNotSet())
            throw new GameObjectNotPlacedException("ScreenFaderがシーン内に配置されていません");

        return singleton.Current();
    }

    void Awake()
    {
        if (singleton.IsAlreadySet())
        {
            Destroy(gameObject);
            return;
        }
        if (singleton.IsNotSet())
        {
            singleton.SetInstance(this);

            DontDestroyOnLoad(gameObject);
        }
    }
    
    #endregion

    public async UniTask FadeOutAsync(CancellationToken cancellationToken = default)
    {
        if (fade == null)
            throw new NullReferenceException("FadeComponentがアタッチされていません");

        fade.Animator.SetTrigger(fade.FadeOutTrigger);

        await UniTask.WaitUntil(
            () => fade.Animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOutState"),
            cancellationToken: cancellationToken);

        AnimatorStateInfo stateInfo = fade.Animator.GetCurrentAnimatorStateInfo(0);

        await UniTask.Delay(TimeSpan.FromSeconds(stateInfo.length), cancellationToken: cancellationToken);
    }

    public async UniTask FadeInAsync(CancellationToken cancellationToken = default)
    {
        if (fade == null)
            throw new NullReferenceException("FadeComponentがアタッチされていません");

        fade.Animator.SetTrigger(fade.FadeInTrigger);

        await UniTask.WaitUntil(
            () => fade.Animator.GetCurrentAnimatorStateInfo(0).IsName("FadeInState"),
            cancellationToken: cancellationToken);

        AnimatorStateInfo stateInfo = fade.Animator.GetCurrentAnimatorStateInfo(0);

        await UniTask.Delay(TimeSpan.FromSeconds(stateInfo.length), cancellationToken: cancellationToken);
    }
}
