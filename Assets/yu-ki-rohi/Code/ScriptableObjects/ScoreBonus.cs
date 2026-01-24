using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreBonus", menuName = "Data/ScoreBonus")]
public class ScoreBonus : ScriptableObject
{
    [Min(0)]
    public List<int> BonusBorder;
    [Min(1.0f)]
    public List<float> Bonus;

    public int Length { get => Mathf.Min(BonusBorder.Count, Bonus.Count); }
}
