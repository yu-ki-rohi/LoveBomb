using TMPro;
using UnityEngine;

public class ApproachingEnemyNumDiplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text currentNumText;

    [SerializeField]
    private TMP_Text maxNumText;

    [SerializeField]
    private CoreController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!currentNumText || !maxNumText || !controller) { return; }

        currentNumText.text = controller.CurrentEnemyCount.ToString();
        maxNumText.text = controller.MaxEnemyCount.ToString();
    }
}
