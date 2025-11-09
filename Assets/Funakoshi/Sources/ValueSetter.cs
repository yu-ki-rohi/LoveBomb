using System.Collections.Generic;
using UnityEngine;

public class ValueSetter : MonoBehaviour
{
    [SerializeField] private NumberTextComponent killsCount;

    void Start()
    {
        // ƒV[ƒ“‚ğŒ×‚¢‚Å‚«‚½’l‚ğ‚±‚±‚Å‘ã“ü‚µ‚Ü‚·

        killsCount.InitalSetValue(ScoreManager.Instance.GetKillNum());
    }
}
