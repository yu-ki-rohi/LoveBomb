using Unity.Cinemachine;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private StageData stageData;
    [SerializeField] private GameTimeManager gameTimeManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private DefeatNumViewer defeatNumViewer;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2;

    [SerializeField] private HeartCore heartCore;

    private StageManager stageManager;

    void Awake()
    {
        gameTimeManager.TimeInfomation = stageData.TimeInfomation;
        scoreManager.DefeatNumViewer = defeatNumViewer;
        scoreManager.ScoreInfomation = stageData.ScoreInfomation;
        heartCore.ScoreFluctuate = scoreManager;

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
}
