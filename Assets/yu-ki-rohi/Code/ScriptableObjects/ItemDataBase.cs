using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemCommonData
{
    [Min(0.0f)]
    public float Speed = 10.0f;

    [Min(0.0f), Header("点滅：第一フェーズ")]
    public float StartBlinkRemainingTimeFirstPhase = 5.0f;
    [Range(0.0f, 1.0f)]
    public float VisibleTimeFirstPhase = 0.4f;
    [Range(0.0f, 0.5f)]
    public float InvisibleTimeFirstPhase = 0.1f;
    [Min(0.0f), Header("点滅：第二フェーズ")]
    public float StartBlinkRemainingTimeSecondPhase = 2.0f;
    [Range(0.0f, 1.0f)]
    public float VisibleTimeSecondPhase = 0.2f;
    [Range(0.0f, 0.5f)]
    public float InvisibleTimeSecondPhase = 0.05f;
}

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "ItemData/DataBase")]
public class ItemDataBase : ScriptableObject
{

    public ItemCommonData CommonData;

    public List<ItemData> Items;
}
