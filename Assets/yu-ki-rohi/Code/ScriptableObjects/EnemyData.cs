using UnityEngine;

[CreateAssetMenu(fileName = "EnemyName", menuName = "CharacterData/EnemyData")]
public class EnemyData : ScriptableObject
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
    [Min(0)]
    public int Power = 10;
    [Min(0.0f)]
    public float Agility = 5.0f;
    [Header("爆発")]
    public int ExplosionPower = 6;

    public float ExplosionScale = 1.0f;
    [Header("お化け型用")]
    public Vector2 AxietyEffectOffset = Vector2.zero;

    [Header("こうもり型用")]
    [Min(0.0f)]
    public float StandbyDistance = 8.0f;
    [Min(0.0f)]
    public float StandbyDistanceBaffar = 0.0f;
    [Min(0.0f)]
    public float PrepareRushTime = 0.5f;
    [Min(1.0f)]
    public float RushInterval = 5.0f;
    [Min(1.0f)]
    public float RushSpeedMultiplier = 2.0f;

    [Header("報酬")]
    public Enemy.Drops DropsNomal;
    public Enemy.Drops DropsExplosion;
    [Min(0)]
    public int BaseScore = 10;
    [Min(0)]
    public int Enegy = 10;
}