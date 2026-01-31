using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataList", menuName = "CharacterData/EnemyDataList")]
public class EnemyDataList : ScriptableObject
{
    public List<EnemyData> EnemyList;
}
