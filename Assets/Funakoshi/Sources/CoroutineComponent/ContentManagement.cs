using System.Collections.Generic;
using UnityEngine;

public class ContentManagement : MonoBehaviour
{
    [SerializeField] List<CoroutineContentSkipUnit> skipUnits;

    private int skipUnitIndex = 0;
    private int coroutineContentIndex = 0;
    private CoroutineContent CurrentContent => skipUnits[skipUnitIndex].CoroutineContents[coroutineContentIndex];
    private bool IsValidIndex => skipUnitIndex < skipUnits.Count &&
        coroutineContentIndex < skipUnits[skipUnitIndex].CoroutineContents.Count;
    private bool isAllContentsEnd = false;

    public bool IsAllContentEnd()
    {
        return isAllContentsEnd;
    }
    public void RunFirstContent()
    {
        CurrentContent.ProcessStarted();
    }

    public void ContentUpdate()
    {
        if (!IsValidIndex)
        {
            Debug.LogError("インデックスエラーが発生しました");
        }
        if (CurrentContent.IsContentEnd())
        {
            NextContent();
        }
    }
    public void SkipContent()
    {
        if (!IsValidIndex)
        {
            Debug.LogError("インデックスエラーが発生しました");
            return;
        }

        foreach (var content in skipUnits[skipUnitIndex].CoroutineContents)
        {
            content.ForcedEnd();
        }

        AdvandeNextUnit();

        if (!IsValidIndex)
        {
            isAllContentsEnd = true;
            return;
        }

        CurrentContent.ProcessStarted();
    }

    private void NextContent()
    {
        AdvanceContent();

        if (!IsValidIndex)
        {
            isAllContentsEnd = true;
            return;
        }

        CurrentContent.ProcessStarted();
    }
    private void AdvanceContent()
    {
        if (!IsValidIndex)
        {
            Debug.LogError("インデックスエラーが発生しました");
        }

        coroutineContentIndex++;

        if (!IsValidIndex)
        {
            AdvandeNextUnit();
        }
    }
    private void AdvandeNextUnit()
    {
        skipUnitIndex++;
        coroutineContentIndex = 0;
    }
}
