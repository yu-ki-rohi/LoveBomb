using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private Image remainigTimeView;

    private float elapsedTime = 0.0f; 
    private TimeInfomation timeInfomation;

    public TimeInfomation TimeInfomation { set { timeInfomation = value; } }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(RemainingTimeViewCoroutine());
        StartCoroutine(ElapsedTimeEventCoroutine(timeInfomation.GameTime, TimeOver)); // CS1503
    }

    private void TimeOver()
    {
        StopAllCoroutines();
        DebugMessenger.Log("Time is Over!!");
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
