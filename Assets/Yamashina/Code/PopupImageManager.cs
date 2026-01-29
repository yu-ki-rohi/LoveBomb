using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupImageManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Sprite[] images;
    [SerializeField] private string[] stageNames; // images と同じ長さ
    private GameObject canvasObject;
    private GameObject imageObject;
    private PopupCanvasView canvasView;
    [SerializeField] private float fadeDuration = 0.5f; // フェード時間
    [SerializeField] private GameState gameState;

    private int currentImageIndex = 0;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.StageSelect);
        // GameState に保存されている StageID を反映
        currentImageIndex = Mathf.Clamp(
            gameState.StageID,
            0,
            images.Length - 1
        );
        SpawnTutorial();

    }

    private void Update()
    {
        //TODO：InputSystemに差し替えたいけどスピード重視で一旦旧システムで実装
        if(Input.GetKeyDown(KeyCode.A))
        {
            ShowPreviousImage();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ShowNextImage();    
        }
    }

    public void SpawnTutorial()
    {
        SpawnCanvasWithImage(images[currentImageIndex]);
    }

    public void SpawnCanvasWithImage(Sprite sprite)
    {
        if (canvasObject != null)
        {
            Destroy(canvasObject);
        }

        canvasObject = Instantiate(canvasPrefab);

        canvasView = canvasObject.GetComponent<PopupCanvasView>();
        if (canvasView == null)
        {
            Debug.LogError("PopupCanvasView が CanvasPrefab に付いていません");
            return;
        }

        // 画像生成（レイヤー固定）
        imageObject = Instantiate(imagePrefab, canvasView.backgroundRoot);

        Image image = imageObject.transform.Find("MapImage").gameObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
            image.color = new Color(1, 1, 1, 1); // 初期は不透明
        }
      
        EventTrigger eventTrigger = imageObject.transform.Find("MapImage").gameObject.AddComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerClick });
            eventTrigger.triggers[0].callback.AddListener((data) => { Scene(); });


        }

        // ボタン設定（挙動は元のまま）
        Button nextButton =
            canvasView.controlRoot.Find("NextButton")?.GetComponent<Button>();

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNextImage);
        }

        Button prevButton =
            canvasView.controlRoot.Find("ChangeImage_Return")?.GetComponent<Button>();

        if (prevButton != null)
        {
            prevButton.onClick.RemoveAllListeners();
            prevButton.onClick.AddListener(ShowPreviousImage);
        }
        Button returnButton = canvasView.Return;
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => SceneTransitionManager.Instance.TransitionToPreviousScene());
        }
        UpdateStageText();
    }

    public void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1)
        {
            currentImageIndex++;
            StartFadeChange(images[currentImageIndex]);

        }
    }

    public void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            StartFadeChange(images[currentImageIndex]);
        }
    }
    private void StartFadeChange(Sprite newSprite)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeImageCoroutine(newSprite));
    }

    //アニメーション違和感あり（でも言語化できない）
    private IEnumerator FadeImageCoroutine(Sprite newSprite)
    {
        if (imageObject == null) yield break;

        Image image = imageObject.transform.Find("MapImage").gameObject.GetComponent<Image>();
        if (image == null) yield break;

        // フェードアウト
        float elapsed = 0f;
        Color originalColor = image.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, elapsed / fadeDuration));
            yield return null;
        }

        // 画像を切り替え
        image.sprite = newSprite;

        // ステージ名も更新
        UpdateStageText();

        // フェードイン
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, elapsed / fadeDuration));
            yield return null;
        }

        image.color = Color.white; // 完全不透明に戻す
    }
    private void UpdateStageText()
    {
        if (canvasView == null) return;

      
        // ボタン表示/非表示制御
        Button nextButton = canvasView.controlRoot.Find("NextButton")?.GetComponent<Button>();
        Button prevButton = canvasView.controlRoot.Find("ChangeImage_Return")?.GetComponent<Button>();

        if (nextButton != null)
        {
            CanvasGroup nextCg = nextButton.GetComponent<CanvasGroup>();
            if (nextCg == null) nextCg = nextButton.gameObject.AddComponent<CanvasGroup>();

            // 最後ならフェードアウト、それ以外はフェードイン
            StartCoroutine(FadeButton(nextCg, currentImageIndex < images.Length - 1));
        }

        if (prevButton != null)
        {
            CanvasGroup prevCg = prevButton.GetComponent<CanvasGroup>();
            if (prevCg == null) prevCg = prevButton.gameObject.AddComponent<CanvasGroup>();

            // 最初ならフェードアウト、それ以外はフェードイン
            StartCoroutine(FadeButton(prevCg, currentImageIndex > 0));
        }
    }
    private IEnumerator FadeButton(CanvasGroup cg, bool show)
    {
        float duration = 0.3f; // フェード時間
        float start = cg.alpha;
        float end = show ? 1f : 0f;
        float elapsed = 0f;

        cg.interactable = show; // 透明でも操作可能にならないように
        cg.blocksRaycasts = show;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;
    }
    public void ChangeImage(Sprite sprite)
    {
        if (imageObject == null) return;

        Image image = imageObject.transform.Find("MapImage").gameObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }
        UpdateStageText();
    }

    public void DestroyCanvasWithImage()
    {
        if (canvasObject != null)
        {
            Destroy(canvasObject);
        }
    }


    private void Scene()
    {
       gameState.StageID= currentImageIndex;    
        SceneTransitionManager.Instance.TransitionToNextScene(FadeMode.SimpleColor);
    }
}
