using UnityEngine;

[CreateAssetMenu(fileName = "StageData[n]", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    public GameObject StageObject;
    public float GameTime = 90.0f;


}
