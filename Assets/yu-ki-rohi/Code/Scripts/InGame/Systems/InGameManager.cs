using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private StageDataBase stageDataBase;
    [SerializeField] private GameTimeManager gameTimeManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LightManager lightManager;
    [SerializeField] private DefeatNumViewer defeatNumViewer;
    [SerializeField] private TextMeshProUGUI enemyNumText;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2;
    [SerializeField] private GameState gameState;
    [SerializeField] private UsedItemPoolManager usedItemPoolManager;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private List<Image> pauseButtons;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private List<Sprite> pauseNorm;
    [SerializeField] private List<Sprite> pauseSelected;

    private InputAction ingamePause;
    private InputAction menuPause;
    private InputAction upInPause;
    private InputAction downInPause;

    private int pauseIndex = 0;

#if UNITY_EDITOR
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData stageData;
#endif

    public void Continue()
    {
        playerInput.SwitchCurrentActionMap("InGame");
        Time.timeScale = 1.0f;
        pauseCanvas.enabled = false;
        pauseIndex = 0;
    }

    public void Retry()
    {
        Time.timeScale = 1.0f;
        // TODO: フェード付きのものに差し替え
        SceneManager.LoadScene("InGameTest");
    }

    public void Return()
    {
        Time.timeScale = 1.0f;
        // TODO: フェード付きのものに差し替え
        SceneManager.LoadScene("StageSelect");
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if(pauseCanvas.enabled)
        {
            switch(pauseIndex)
            {
                case 0:
                    Continue();
                    break;
                    
                case 1:
                    Retry();
                    break;
                    
                case 2:
                    Return();
                    break;
            }
        }
        else
        {
            pauseIndex = 0;
            ReflectPauseUI();
            playerInput.SwitchCurrentActionMap("Menu");
            Time.timeScale = 0.0f;
            pauseCanvas.enabled = true;
        }
    }

    private void OnUp(InputAction.CallbackContext context)
    {
        pauseIndex--;
        if(pauseIndex < 0) { pauseIndex = 0; }
        ReflectPauseUI();
    }

    private void OnDown(InputAction.CallbackContext context)
    {
        pauseIndex++;
        if(pauseIndex > 2) { pauseIndex = 2; }
        ReflectPauseUI();
    }

    private void OnPointEnter(int index)
    {
        pauseIndex = index;
        ReflectPauseUI();
    }

    private void ReflectPauseUI()
    {
        int length = Mathf.Min(pauseNorm.Count, pauseSelected.Count);
        length = Mathf.Min(pauseButtons.Count, length);

        for(int i = 0; i < length; i++)
        {
            if(i == pauseIndex)
            {
                pauseButtons[i].sprite = pauseSelected[i];
            }
            else
            {
                pauseButtons[i].sprite = pauseNorm[i];
            }
        }
    }

    void Awake()
    {
        if(gameState.StageID < 0 || gameState.StageID >= stageDataBase.Stages.Count)
        {
            // TODO: ステージ選択に引き返させる処理の追加　※StageID側を変更したので不要になるかも
            return;
        }


#if UNITY_EDITOR
        StageData stageData;
        StageManager stageManager;
        if (this.stageData == null || this.stageManager == null)
        {
            stageData = stageDataBase.Stages[gameState.StageID];
            stageManager = Instantiate(stageData.StageManager);
        }
        else
        {
            stageData = this.stageData;
            stageManager = this.stageManager;
        }
#else
        StageData stageData = stageDataBase.Stages[gameState.StageID];
        StageManager stageManager = Instantiate(stageData.StageManager);
#endif
        stageManager.SetInitialPositionOfPlayer(player.transform);

        gameTimeManager.TimeInfomation = stageData.TimeInfomation;
        gameTimeManager.SetTimeUpEvent(OnTimeUp);

        lightManager.LightInfomation = stageData.LightInfomation;
        lightManager.HeartCoreLight = stageManager.HeartCoreLight;

        scoreManager.DefeatNumViewer = defeatNumViewer;
        scoreManager.ScoreInfomation = stageData.ScoreInfomation;
        scoreManager.LightManager = lightManager;
        scoreManager.SetOnTouchUpEvent(OnTouchUp);

        stageManager.HeartCore.ScoreFluctuate = scoreManager;
        stageManager.HeartCore.EnemyNumText = enemyNumText;
        stageManager.ManagedEnemyPoolManager.DefeatNumViewer = defeatNumViewer;

        player.EffectPoolManager = stageManager.EffectPoolManager;
        player.ExplosionPoolManager = stageManager.ExpsionPoolManager;

        usedItemPoolManager.ExplosionPoolManager = stageManager.ExpsionPoolManager;

        cinemachineConfiner2.BoundingShape2D = stageManager.VisibleArea;

        
        ingamePause = playerInput.actions.FindActionMap("InGame").FindAction("Pause");
        menuPause = playerInput.actions.FindActionMap("Menu").FindAction("Pause");
        upInPause = playerInput.actions.FindActionMap("Menu").FindAction("Up");
        downInPause = playerInput.actions.FindActionMap("Menu").FindAction("Down");

        playerInput.SwitchCurrentActionMap("InGame");

        for(int i = 0; i < pauseButtons.Count; i++)
        {
            var buttonHover = pauseButtons[i].gameObject.GetComponent<ButtonHover>();
            if(buttonHover == null ) { continue; }
            buttonHover.Index = i;
            buttonHover.SetOnPointerEnter(OnPointEnter);
        }

        Time.timeScale = 1.0f;

    }

    private void OnEnable()
    {
        ingamePause.performed += OnPause;
        menuPause.performed += OnPause;
        upInPause.performed += OnUp;
        downInPause.performed += OnDown;
    }

    private void OnDisable()
    {
        ingamePause.performed -= OnPause;
        menuPause.performed -= OnPause;
        upInPause.performed -= OnUp;
        downInPause.performed -= OnDown;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: 当日版では消す
        //Escが押された時
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
        }
    }

    private void OnTimeUp()
    {
        scoreManager.LockScoreFluctuation();
        // TODO: 演出追加
        GameSet();
    }

    private void OnTouchUp()
    {
        scoreManager.LockScoreFluctuation();
        gameTimeManager.TimerStop();
        // TODO: 演出追加
        GameSet();
    }

    private void GameSet()
    {
        gameState.Score = scoreManager.CurrentScore;
        gameState.ClearTime = stageDataBase.Stages[gameState.StageID].TimeInfomation.GameTime - gameTimeManager.ElapsedTime;

        Time.timeScale = 1.0f;
        // TODO: フェード付きのものに差し替え
        SceneManager.LoadScene("InGameTest");
    }
}
