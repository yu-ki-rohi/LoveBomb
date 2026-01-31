using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataBase", menuName = "Data/StageDataBase")]
public class StageDataBase : ScriptableObject
{
    public List<StageData> Stages;
}
