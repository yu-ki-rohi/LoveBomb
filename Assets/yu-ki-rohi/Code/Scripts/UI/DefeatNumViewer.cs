using UnityEngine;
using UnityEngine.UI;

public class DefeatNumViewer : MonoBehaviour
{
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite exclamation;

    [SerializeField] private Image[] numViewers;
    [SerializeField] private Image exclamationViewer;

    private int defeatNum = 0;

    
    public void OnDefeatEnemy()
    {
        defeatNum++;

        ReflectUI(defeatNum);
    }

    private void ReflectUI(int defeatNum)
    {
        // 数字が足りないときはエラー
        if (numbers.Length < 10)
        {
            DebugMessenger.LogError("Numbers is too short");
            return;
        }

        if(defeatNum == 0)
        {
            exclamationViewer.enabled = false;
        }
        else
        {
            exclamationViewer.enabled = true;
        }

        for (int i = 0; i < numViewers.Length; i++)
        {
            if(defeatNum == 0)
            {
                numViewers[i].enabled = false;
                continue;
            }

            int num = defeatNum % 10;
            numViewers[i].sprite = numbers[num];
            numViewers[i].enabled = true;
            // 端数切捨て
            // 除算は切り捨ての仕様だった気がするから不要だろうが、念のため
            defeatNum -= num;
            defeatNum /= 10;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ReflectUI(defeatNum);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
