using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemName", menuName = "ItemData/Data")]
public class ItemData : ScriptableObject
{
    public enum Type
    { 
        Attract,
        Landmines,
        Barrier
    }

    public string Name;
    public int id;
    public int MaxNum = 9;

    [Header("ドロップ時")]
    public Sprite Icon;
    public float LifeTime = 10.0f;

    [Header("使用時")]
    public RuntimeAnimatorController Controller;
    public Type ItemType = Type.Attract;
    public float Duration = 10.0f;
    public float Radius = 10.0f;

    [Space(10)]
    [Header("インゲーム中に変更するもの")]
    public int NumberOfPossessions = 0;

}
