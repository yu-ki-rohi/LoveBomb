using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGenerator", menuName = "CharacterData/EnemyGeneratorData")]
public class EnemyGeneratorData : ScriptableObject
{
    [Header("基本情報")]
    public string Name;
    public Sprite Image;
    [Header("パラメーター")]
    [Min(1)]
    public int MaxHitPoint = 10;

    [Header("爆発")]
    public int ExplosionPower = 6;
    public float ExplosionScale = 1.0f;

    [Header("報酬")]
    [Min(0)]
    public int BaseScore = 10;
}