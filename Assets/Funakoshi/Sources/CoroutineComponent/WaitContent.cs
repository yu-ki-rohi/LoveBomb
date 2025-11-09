using System.Collections;
using UnityEngine;

public class WaitContent : CoroutineContent
{
    [SerializeField] float waitSeconds;

    public override void ProcessStarted()
    {
        Debug.Log(contentEnd);
        StartCoroutine(WaitFewSeconds());
    }
    public override void ForcedEnd()
    {
        contentEnd = true;
    }
    private IEnumerator WaitFewSeconds()
    {
        yield return new WaitForSeconds(waitSeconds);
        contentEnd = true;
    }
}
