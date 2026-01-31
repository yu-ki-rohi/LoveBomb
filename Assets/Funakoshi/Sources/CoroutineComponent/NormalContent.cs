using UnityEngine;

public class NormalContent : CoroutineContent
{
    [SerializeField] private GameObject content;
    [SerializeField] private bool isActive = true;

    public override void ProcessStarted()
    {
        content.SetActive(isActive);
        contentEnd = true;
    }
    public override void ForcedEnd()
    {
        content.SetActive(isActive);
        contentEnd = true;
    }
}
