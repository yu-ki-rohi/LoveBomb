using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Light2D playerLight;
    [SerializeField] private Light2D grobalLight;
    private Light2D heartCoreLight;
    private LightInfomation lightInfomation;

    public Light2D HeartCoreLight { set { heartCoreLight = value; } }
    public LightInfomation LightInfomation { set {  lightInfomation = value; } }

    public void ChangeLight(float fillAmount)
    {
        float ratio = Mathf.Clamp01(fillAmount);

        heartCoreLight.pointLightInnerRadius = (lightInfomation.maxLightRadius - lightInfomation.minLightRadius) * ratio + lightInfomation.minLightRadius;
        heartCoreLight.pointLightOuterRadius = heartCoreLight.pointLightInnerRadius * lightInfomation.lightOuterRadiusMaltiplier;

        if(fillAmount > 0.5f)
        {
            float t = (ratio - 0.5f) * 2.0f;
            Color minColor = lightInfomation.minGrobalLight;
            Color maxColor = lightInfomation.maxGrobalLight;
            grobalLight.color = new Color(maxColor.r * t + minColor.r * (1.0f - t), maxColor.g * t + minColor.r * (1.0f - t), maxColor.r * t + minColor.b * (1.0f - t));
        }
        else
        {
            grobalLight.color = lightInfomation.minGrobalLight;
        }

        // HACK: ボーダーのパラメーターを
        if(fillAmount > lightInfomation.playerLightBorder)
        {
            playerLight.enabled = false;
        }
        else
        {
            playerLight.enabled = true;
        }
    }

    public void LightOut(float time)
    {
        StartCoroutine(LightOutCoroutine(time));
    }

    private IEnumerator LightOutCoroutine(float time)
    {
        float diff = heartCoreLight.pointLightOuterRadius - heartCoreLight.pointLightInnerRadius;
        float rate = diff / time;
        while(heartCoreLight.pointLightOuterRadius - heartCoreLight.pointLightInnerRadius > 0)
        {
            yield return null;
            heartCoreLight.pointLightOuterRadius -= rate * Time.deltaTime;
        }

    }



}
