using UnityEngine;

[CreateAssetMenu(fileName = "StageData[n]", menuName = "Data")]
public class StageData : ScriptableObject
{
    public GameObject StageObject;
    public float GameTime = 90.0f;


}
