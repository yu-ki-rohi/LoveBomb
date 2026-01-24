using UnityEngine;

[CreateAssetMenu(fileName = "FadeSetting", menuName = "Scene Transition/FadeSetting")]
public class FadeSetting : ScriptableObject
{
    [Header("フェードにかかる時間 (秒)")]
    public float FadeDuration = 1f;

    [Header("透明(0)～暗転(1) の間をどう変化させるか")]
    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
}
