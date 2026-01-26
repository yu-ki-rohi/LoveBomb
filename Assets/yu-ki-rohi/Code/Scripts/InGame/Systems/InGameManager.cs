using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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


     


    void Awake()
    {
        if(gameState.StageID < 0 || gameState.StageID >= stageDataBase.Stages.Count)
        {
            // TODO: ステージ選択に引き換えさせる処理の追加
            return;
        }

        StageData stageData = stageDataBase.Stages[gameState.StageID];
        StageManager stageManager = Instantiate(stageData.StageManager);
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
