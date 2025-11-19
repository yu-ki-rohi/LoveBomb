using UnityEditor;
using UnityEngine;

// GeneratorBaseがあるがあちらで出現範囲の指定と出現位置の決定
// こちらで出現するオブジェクトを決定している

public abstract class Generator : MonoBehaviour
{
    [SerializeField, HideInInspector] protected GeneratorBase generator;

    void OnDestroy()
    {
        generator?.UnlinkCallback(OnGenerate);
    }

    

    protected abstract void OnGenerate();

    #region エディタ限定
#if UNITY_EDITOR
    public void ForcedGenerate()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("This Execute Only in Playing!!");
            return;
        }
        OnGenerate();
    }

    public void AttachCircle()
    {
        AttachGenerator<CircleGenerator>();
    }

    public void AttachBox()
    {
        AttachGenerator<BoxGenerator>();
    }

    private void AttachGenerator<T>() where T : GeneratorBase
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("This Execute Only in Editor!!");
            return;
        }

        float initialGenerateDelay = 5.0f, generateInterval = 3.0f, generateIntervalRandomOffset = 0.0f;
        if (generator != null)
        {
            initialGenerateDelay = generator.InitialGenerateDelay;
            generateInterval = generator.GenerateInterval;
            generateIntervalRandomOffset = generator.GenerateIntervalRandomOffset;
            Undo.DestroyObjectImmediate(generator);
        }
        generator = Undo.AddComponent<T>(gameObject);
        generator.InitialGenerateDelay = initialGenerateDelay;
        generator.GenerateInterval = generateInterval;
        generator.GenerateIntervalRandomOffset = generateIntervalRandomOffset;
    }

#endif

    #endregion

}
