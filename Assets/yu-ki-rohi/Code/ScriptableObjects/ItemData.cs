using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemName", menuName = "ItemData/Data")]
public class ItemData : ScriptableObject
{
    public string Name;
    public Image Icon;
    public float LifeTime = 10.0f;
    public int NumberOfPossessions = 0;

}
