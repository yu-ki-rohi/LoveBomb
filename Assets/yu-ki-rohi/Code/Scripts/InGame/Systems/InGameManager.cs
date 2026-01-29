using System.Collections;
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
    [SerializeField] private GameObject readyGo;
    [SerializeField] private GameObject timeUp;
    [SerializeField] private DemoDirectionData demoDirectionData;

    private InputAction ingamePause;
    private InputAction menuPause;
    private InputAction upInPause;
    private InputAction downInPause;
    
    private EnemiesGeneratorManager enemiesGeneratorManager;

    private bool canPause = false;
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
        AudioManager.Instance.PlaySEById(SEName.PauseOff);

        pauseIndex = 0;
    }

    public void Retry()
    {
        Time.timeScale = 1.0f;
        LockEveryThing();
        // TODO: 
        SceneTransitionManager.Instance.TransitionToCurrentScene();
    }

    public void Return()
    {
        Time.timeScale = 1.0f;
        LockEveryThing();
        // TODO:
        SceneTransitionManager.Instance.TransitionToPreviousScene();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if(canPause == false) { return; }
        if (pauseCanvas.enabled)
        {
            switch (pauseIndex)
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
            AudioManager.Instance.PlaySEById(SEName.PauseOn);
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
        if (pauseIndex < 0) { pauseIndex = 0; }
        ReflectPauseUI();
    }

    private void OnDown(InputAction.CallbackContext context)
    {
        pauseIndex++;
        if (pauseIndex > 2) { pauseIndex = 2; }
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

        for (int i = 0; i < length; i++)
        {
            if (i == pauseIndex)
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
        if (gameState.StageID < 0 || gameState.StageID >= stageDataBase.Stages.Count)
        {
            // TODO: 
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

        enemiesGeneratorManager = stageManager.EnemiesGeneratorManager;

        player.EffectPoolManager = stageManager.EffectPoolManager;
        player.ExplosionPoolManager = stageManager.ExpsionPoolManager;

        usedItemPoolManager.ExplosionPoolManager = stageManager.ExpsionPoolManager;

        cinemachineConfiner2.BoundingShape2D = stageManager.VisibleArea;


        ingamePause = playerInput.actions.FindActionMap("InGame").FindAction("Pause");
        menuPause = playerInput.actions.FindActionMap("Menu").FindAction("Pause");
        upInPause = playerInput.actions.FindActionMap("Menu").FindAction("Up");
        downInPause = playerInput.actions.FindActionMap("Menu").FindAction("Down");

        playerInput.SwitchCurrentActionMap("InGame");

        for (int i = 0; i < pauseButtons.Count; i++)
        {
            var buttonHover = pauseButtons[i].gameObject.GetComponent<ButtonHover>();
            if (buttonHover == null) { continue; }
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
        switch (gameState.StageID)
        {
            case 0:
                // TODO: Stage1のBGM

                AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Stage1);

                break;
            case 1:
                // TODO: Stage2のBGM
                AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Stage2);

                break;
            case 2:
                // TODO: Stage3のBGM
                AudioManager.Instance.PlayBGMIfNotPlaying(BGMName.Stage3);

                break;

        }

        readyGo.SetActive(true);
        StartCoroutine(GameStartCoroutine());

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTimeUp()
    {
        scoreManager.LockScoreFluctuation();
        // TODO:
        GameSet();
    }

    private void OnTouchUp()
    {
        scoreManager.LockScoreFluctuation();
        gameTimeManager.TimerStop();
        // TODO: 
        GameSet();
    }

    private void GameSet()
    {
        gameState.Score = scoreManager.CurrentScore;
        gameState.ClearTime = stageDataBase.Stages[gameState.StageID].TimeInfomation.GameTime - gameTimeManager.ElapsedTime;

        Time.timeScale = 1.0f;
        // TODO: 
        SceneTransitionManager.Instance.TransitionToNextScene();
    }

    private IEnumerator GameStartCoroutine()
    {
        yield return new WaitForSeconds(demoDirectionData.StartTime);
        player.CanMove = true;
        enemiesGeneratorManager.BootGenerators();
        canPause = true;
        
    }

    private void LockEveryThing()
    {
        player.CanMove = true;
        canPause = true;
        scoreManager.LockScoreFluctuation();
        gameTimeManager.TimerStop();
    }
}

