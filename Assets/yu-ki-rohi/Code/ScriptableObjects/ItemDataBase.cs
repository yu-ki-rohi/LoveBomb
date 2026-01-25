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
    public enum ItemKind
    {
        Bell,
        Pen,
        Sphere
    }

    public ItemCommonData CommonData;


    [Header("ベル")]
    public float VacuumPower = 10.0f;
    public float Interval = 3.0f;

    [Header("スフィア")]
    public float PushPower = 3.0f;
    public float KnockPower = 3.0f;
    public float BlinkTime = 3.0f;

    [Header("ペン")]
    public int ExplosionPower = 6;
    public float ExplosionScale = 5.0f;
    public int NumOfIgnit = 10;



    public List<ItemData> Items;
}
