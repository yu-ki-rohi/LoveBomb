using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 爆発による連続撃破した数を表示するためのクラス
public class DefeatNumViewer : MonoBehaviour
{
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private Sprite exclamation;

    [SerializeField] private Image[] numViewers;
    [SerializeField] private Image exclamationViewer;
    [SerializeField] private ScoreBonus scoreBonus;

    // HACK: 一旦ここで
    [SerializeField, Min(0.1f)] private float validityTime = 5.0f;

    private int defeatNum = 0;

    private Coroutine invalidationCoroutine = null;
    
    public int DefeatNum { get =>  defeatNum; }

    public void OnDefeatEnemy()
    {
        int tmp = defeatNum;
        defeatNum++;

        for(int i = 0; i < scoreBonus.Length; i++)
        {
            if(tmp < scoreBonus.BonusBorder[i] && defeatNum >= scoreBonus.BonusBorder[i])
            {
                //TODO: コンボ数一定値を越えた音
                AudioManager.Instance.PlaySEById(SEName.ComboThresholdReached);
                break;
            }
        }

        if(invalidationCoroutine != null)
        {
            StopCoroutine(invalidationCoroutine);
        }

        invalidationCoroutine = StartCoroutine(InvalidationCoroutine());

        SetViewerAlphaValue(1.0f);
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

    private IEnumerator InvalidationCoroutine()
    {
        float halfValideityTime = validityTime / 2.0f;

        yield return new WaitForSeconds(halfValideityTime);

        float remainingValideityTime = halfValideityTime;

        while (remainingValideityTime > 0)
        {
            SetViewerAlphaValue(remainingValideityTime / halfValideityTime);

            remainingValideityTime -= Time.deltaTime;

            yield return null;

        }

        defeatNum = 0;

        ReflectUI(defeatNum);

    }

    private void SetViewerAlphaValue(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);

        foreach (var numViewer in numViewers)
        {
            numViewer.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }

        exclamationViewer.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }
}
