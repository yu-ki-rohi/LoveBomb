using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoreEnemyCounter : MonoBehaviour
{
    [Header("当たっている敵の数を表示するテキスト")]
    [SerializeField] private TMP_Text coreEnemyCounterText;

    [Header("当たっている敵の数を表示するスライダー")]
    [SerializeField] private Slider coreEnemyCounterSlider;

    [Header("表示モード切り替え")]
    [Tooltip("true = 当たっている敵の数 / false = 残り許容数")]
    [SerializeField] private bool showCurrentCount = true;

    [Header("Core（CoreControllerがついたオブジェクト）")]
    [SerializeField] private CoreController coreController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 表示
        UpdateEnemyCount();
    }

    // Update is called once per frame
    void Update()
    {
        // 表示
        UpdateEnemyCount();
    }

    // 当たっている敵の数のUIを更新して表示
    private void UpdateEnemyCount()
    {
        // UI設定チェック
        if (coreEnemyCounterText == null && coreEnemyCounterSlider == null)
        {
            Debug.LogWarning("UI未設定");
            return;
        }
        else
        {
            // テキスト設定チェック
            if (coreEnemyCounterText != null)
            {
                UpdateEnemyCountText();
            }

            // スライダー設定チェック
            if (coreEnemyCounterSlider != null)
            {
                UpdateEnemyCountSlider();
            }
        }
    }

    // テキスト更新
    private void UpdateEnemyCountText()
    {
        if (showCurrentCount == true)
        {
            // 「当たっている敵の数」を表示
            coreEnemyCounterText.text = coreController.CurrentEnemyCount.ToString();
        }
        else
        {
            // 「残り耐えられる敵の数」を表示
            int remaining = Mathf.Max(0, coreController.MaxEnemyCount - coreController.CurrentEnemyCount);
            coreEnemyCounterText.text = "残り" + remaining.ToString() + "体";
        }
    }

    // スライダー更新
    private void UpdateEnemyCountSlider()
    {
        if (showCurrentCount == true)
        {
            // 0～1に変換
            float value = (float)coreController.CurrentEnemyCount / coreController.MaxEnemyCount;
            // 「当たっている敵の数」を表示
            coreEnemyCounterSlider.value = value;
        }
        else
        {
            // 「残り耐えられる敵の数」を表示
            int remaining = Mathf.Max(0, coreController.MaxEnemyCount - coreController.CurrentEnemyCount);
            // 0～1に変換
            float value = (float)remaining / coreController.MaxEnemyCount;
            // 「当たっている敵の数」を表示
            coreEnemyCounterSlider.value = value;
        }
    }
}
