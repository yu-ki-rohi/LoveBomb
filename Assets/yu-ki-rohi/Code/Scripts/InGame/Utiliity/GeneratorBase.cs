using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;


// 設計の変更からGeneratorBaseを名乗りつつ、出現範囲を取得することとSceneビュー上に描画することが主な役割になっている
public abstract class GeneratorBase : MonoBehaviour
{
    public enum Type
    {
        Circle,
        Box
    }

    [SerializeField, Min(0.0f)] private float initialGenerateDelay = 5.0f;
    [SerializeField, Min(0.1f)] private float generateInterval = 3.0f;
    [SerializeField, Min(0.0f)] private float generateIntervalRandomOffset = 0.0f;
    private CancellationTokenSource generateCts;
    private event Action onGenerate;

    
    #region Unity Editor
#if UNITY_EDITOR
    protected bool showAlways = false;  // 表示切替用のフラグ
    protected Color innerColor = new Color(0.0f, 0.0f, 1.0f, 0.25f);     // エネミーが出現しない範囲の色
    protected Color fillColor = new Color(1.0f, 0.0f, 0.0f, 0.25f);      // エネミーが出現する範囲の色
    public float InitialGenerateDelay { get => initialGenerateDelay; set => initialGenerateDelay = value; }
    public float GenerateInterval { get => generateInterval; set => generateInterval = value; }
    public float GenerateIntervalRandomOffset { get => generateIntervalRandomOffset; set => generateIntervalRandomOffset = value; }

    public void SetShowAlways()
    {
        showAlways = true;
    }

    public void SetShowWhenSelected()
    {
        showAlways = false;
    }

#endif
    #endregion

    public void RegisterCallback(Action callback)
    {
        onGenerate += callback;
    }

    public void UnlinkCallback(Action callback)
    {
        onGenerate -= callback;
    }

    public abstract Vector3 DecideGeneratePosition();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateCts = new CancellationTokenSource();
        GenerateAsync(generateCts.Token).Forget();
    }

    void OnDisable()
    {
        // オブジェクト破棄時に安全にキャンセル
        generateCts?.Cancel();
        generateCts?.Dispose();
    }

    private void OnDestroy()
    {
        onGenerate = null;
    }

    private async UniTaskVoid GenerateAsync(CancellationToken token)
    {
        float currentTime = 0f;
        float delay = initialGenerateDelay;
        try
        {
            while (true)
            {
                while (currentTime < delay)
                {
                    // フレーム待ち（Updateタイミング）
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                    // 経過時間加算
                    currentTime += Time.deltaTime;
                }

                // コールバックで生成処理
                onGenerate?.Invoke();
                currentTime = 0f;
                delay = generateInterval;
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Fail to generate Enemy");
        }
        finally
        {
            // CTSの破棄 ヌルチェック + 実行
            generateCts?.Dispose();
            generateCts = null;
        }
    }
}
