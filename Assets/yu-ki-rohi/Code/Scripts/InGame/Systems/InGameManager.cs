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
    [SerializeField] private DefeatNumViewer defeatNumViewer;
    [SerializeField] private TextMeshProUGUI enemyNumText;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2;
    [SerializeField] private GameState gameState;
    [SerializeField] private UsedItemPoolManager usedItemPoolManager;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private PlayerInput playerInput;

    private InputAction ingamePause;
    private InputAction menuPause;

#if UNITY_EDITOR
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData stageData;
#endif



    private void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("Pause is Called");
        if(pauseCanvas.enabled)
        {
            playerInput.SwitchCurrentActionMap("InGame");
            Time.timeScale = 1.0f;
            pauseCanvas.enabled = false;
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Menu");
            Time.timeScale = 0.0f;
            pauseCanvas.enabled = true;
        }
    }

    void Awake()
    {
        if(gameState.StageID < 0 || gameState.StageID >= stageDataBase.Stages.Count)
        {
            // TODO: ステージ選択に引き換えさせる処理の追加
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

        scoreManager.DefeatNumViewer = defeatNumViewer;
        scoreManager.ScoreInfomation = stageData.ScoreInfomation;
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


    }

    private void OnEnable()
    {
        ingamePause.performed += OnPause;
        menuPause.performed += OnPause;
    }

    private void OnDisable()
    {
        ingamePause.performed -= OnPause;
        menuPause.performed -= OnPause;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        GameSet();
    }

    private void OnTouchUp()
    {
        scoreManager.LockScoreFluctuation();
        gameTimeManager.TimerStop();
        GameSet();
    }

    private void GameSet()
    {
        gameState.Score = scoreManager.CurrentScore;
        gameState.ClearTime = stageDataBase.Stages[gameState.StageID].TimeInfomation.GameTime - gameTimeManager.ElapsedTime;

        SceneManager.LoadScene("InGameTest");
    }
}
