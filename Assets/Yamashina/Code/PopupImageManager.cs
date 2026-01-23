using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupImageManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Sprite[] images;

    private GameObject canvasObject;
    private GameObject imageObject;
    private PopupCanvasView canvasView;

    private int currentImageIndex = 0;

    private void Start()
    {
        SpawnTutorial();
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

        Image image = imageObject.GetComponentInChildren<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }

        EventTrigger eventTrigger = imageObject.gameObject.AddComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter });
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
    }

    public void ShowNextImage()
    {
        if (currentImageIndex < images.Length - 1)
        {
            currentImageIndex++;
            ChangeImage(images[currentImageIndex]);
        }
    }

    public void ShowPreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            ChangeImage(images[currentImageIndex]);
        }
    }

    public void ChangeImage(Sprite sprite)
    {
        if (imageObject == null) return;

        Image image = imageObject.GetComponentInChildren<Image>();
        if (image != null)
        {
            image.sprite = sprite;
        }
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
        Debug.Log("船越さんのシーン遷移関数と差し替え予定");
    }
}
