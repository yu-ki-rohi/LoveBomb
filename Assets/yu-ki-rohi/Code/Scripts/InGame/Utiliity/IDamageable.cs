using System;
using UnityEngine;

[Serializable]
public enum DamageType
{
    Piercing,
    Explosion,
    Scaring
}

public interface IDamageable
{
    public void TakeDamage(int attack, DamageType damageType, float bonus);
}
