using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class FadeControllerBase : MonoBehaviour
{
    public abstract UniTask FadeOutAsync(FadeSettings settings);
    public abstract UniTask FadeInAsync(FadeSettings settings);
}
