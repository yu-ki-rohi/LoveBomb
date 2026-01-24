using System;
using UnityEngine.UIElements;

public class DurationField : VisualElement
{
    private readonly FloatField floatField;

    private float duration = 1f;

    public event Action OnValueChanged;

    public DurationField()
    {
        floatField = new FloatField("Duration")
        {
            value = duration,
            formatString = "F2" // 0.01•b‚ðÅ¬’PˆÊ‚ÉÝ’è‚µ‚Ä‚¢‚Ü‚·
        };

        floatField.RegisterValueChangedCallback(evt =>
        {
            OnValueChanged?.Invoke();
        });

        Add(floatField);
    }

    public float GetValue() => duration;
    public void SetValue(float value)
    {
        duration = value;

        floatField.SetValueWithoutNotify(value);
    }
}