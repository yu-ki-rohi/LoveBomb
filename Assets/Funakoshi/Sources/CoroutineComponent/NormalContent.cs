using UnityEngine;

public class NormalContent : CoroutineContent
{
    [SerializeField] private GameObject content;

    public override void ProcessStarted()
    {
        content.SetActive(true);
        contentEnd = true;
    }
    public override void ForcedEnd()
    {
        content.SetActive(true);
        contentEnd = true;
    }
}
