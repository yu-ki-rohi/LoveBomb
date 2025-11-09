using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    [Header("基本情報")]
    public string Name;
    public Sprite Image;
    public RuntimeAnimatorController Controller;
    public Enemy.Type Type;
    public float Scale = 1.0f;
    [Header("パラメーター")]
    [Min(1)]
    public int MaxHitPoint = 3;
    [Min(0)]
    public int Strength = 10;
    [Min(0.0f)]
    public float Agility = 5.0f;
    [Header("爆発")]
    public int ExplosionPower = 6;

    public float ExplosionScale = 1.0f;
    [Header("攻撃エフェクト")]
    public Vector2 AxietyEffectOffset = Vector2.zero;
    [Min(0.0f)]
    public float AnxietyPropagateInterval = 5.0f;
    [Header("報酬")]
    [Min(0)]
    public int BaseScore = 10;
    [Min(0)]
    public int Enegy = 10;
}

[CreateAssetMenu(fileName = "EnemyDataList", menuName = "CharacterData/EnemyDataList")]
public class EnemyDataList : ScriptableObject
{
    public List<EnemyData> EnemyList;
}
