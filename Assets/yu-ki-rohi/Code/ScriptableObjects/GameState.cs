using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject
{
    public int StageID;
    public int Score;
    public float ClearTime;
}
