using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCommonData", menuName = "CharacterData/EnemyCommonData")]
public class EnemyCommonData : ScriptableObject
{
    [Range(0.0f, 2.0f)]
    public float DelayToDisapeear = 1.0f;

    [Range(0.0f, 1.0f)]
    public float DelayToExplosion = 0.5f;

    [Min(0.0f)]
    public float AnxietyPropagateSpeed = 10.0f;
}
