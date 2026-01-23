using TMPro;
using UnityEngine;

public class PopupCanvasView : MonoBehaviour
{
    public Transform backgroundRoot;
    public Transform controlRoot;

    [SerializeField] private TextMeshProUGUI stageText;

    public TextMeshProUGUI GetStageText()
    {
        return stageText;   
    }

}

