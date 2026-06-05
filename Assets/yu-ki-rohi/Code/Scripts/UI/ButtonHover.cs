using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


// Chat GPTのコードから改造
// UI上にマウスが来た時のふるまいを登録
public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{
    private event Action<int> onPointerEnter;
    private int index;

    public int Index { set { index = value; } }

    public void SetOnPointerEnter(Action<int> onEnter)
    {
        onPointerEnter = null;
        onPointerEnter += onEnter;
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(index);
        Debug.Log("On The Button");
    }


}
