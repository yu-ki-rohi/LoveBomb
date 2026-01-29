using UnityEngine;

using UnityEngine.EventSystems;

public class UIButtonSE : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler
{

    public void OnPointerClick(PointerEventData e)
    {
        AudioManager.Instance.PlaySEById(SEName.Click); ;
    }

    public void OnPointerEnter(PointerEventData e)
    {
    }
}

