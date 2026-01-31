using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private Image remainigTimeView;

    private float elapsedTime = 0.0f;
    private event Action OnTimeUp;
    private TimeInfomation timeInfomation;

    public TimeInfomation TimeInfomation { set { timeInfomation = value; } }

    public float ElapsedTime { get { return elapsedTime; } }

    public void TimerStart()
    {
        StartCoroutine(RemainingTimeViewCoroutine());
        StartCoroutine(ElapsedTimeEventCoroutine(timeInfomation.GameTime, TimeOver));
        // HACK: 前日なので速度優先
        StartCoroutine(RemainingTimeEventCoroutine(30.0f, CallOfAproachingFinish));
    }

    public float TimerStop()
    {
        StopAllCoroutines();
        return elapsedTime;
    }

    public void SetTimeUpEvent(Action action)
    {
        OnTimeUp += action;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void CallOfAproachingFinish()
    {
        // TODO: タイムアップ30秒前通知音
        AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.TimeUpWarning);
    }

    private void TimeOver()
    {
        StopAllCoroutines();
        DebugMessenger.Log("Time Up!!");
        AudioManager.Instance.PlaySEById(SEName.TimeUp);
        OnTimeUp?.Invoke();
    }

    private void ReflectUI()
    {
        remainigTimeView.fillAmount = elapsedTime / timeInfomation.GameTime;
    }

    private IEnumerator RemainingTimeViewCoroutine()
    {
        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            ReflectUI();
        }
    }

    private IEnumerator ElapsedTimeEventCoroutine(float elapsedTime, Action action)
    {
        yield return new WaitForSeconds(elapsedTime);
        action?.Invoke();
    }

    private IEnumerator RemainingTimeEventCoroutine(float remainingTime, Action action)
    {
        yield return new WaitForSeconds(timeInfomation.GameTime - remainingTime);
        action?.Invoke();
    }

    

    private IEnumerator LoopEventCoroutine(float loopTime, Action action)
    {
        while (true)
        {
            yield return new WaitForSeconds(loopTime);
            action?.Invoke();
        }
    }

}
