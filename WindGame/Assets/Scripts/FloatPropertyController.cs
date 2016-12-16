using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public class FloatPropertyController : MonoBehaviour
{
    public Slider slider;
    public Text nameText;
    public InputField inputField;
    public Text unitText;

    [HideInInspector]
    public FloatProperty floatProperty;

    // Use this for initialization
    void Start()
    {
        inputField.onValueChanged.AddListener(delegate { InputFieldChange(inputField.text); });
        slider.onValueChanged.AddListener(delegate { SliderChange(slider.value); });

        float savedValue;
        if (SavedTurbineProperties.GetSavedValue(floatProperty.propertyName, out savedValue))
        {
            slider.value = savedValue;
            SliderChange(savedValue);
        }
        else
            slider.value = floatProperty.propertyValue;

        nameText.text = floatProperty.propertyName;
        unitText.text = floatProperty.unit;
        slider.minValue = floatProperty.minValue;
        slider.maxValue = floatProperty.maxValue;


        inputField.contentType = InputField.ContentType.DecimalNumber;
        inputField.text = slider.value.ToString();

        InputFieldChange(inputField.text);
    }

    void InputFieldChange(string value)
    {
        if (value != string.Empty)
            slider.value = float.Parse(value);
    }

    void SliderChange(float value)
    {
        inputField.text = value.ToString();
        floatProperty.propertyValue = value;

        if (floatProperty.graphicsFunction != null)
            floatProperty.graphicsFunction.Invoke(floatProperty.callObject, new object[] { slider.value });


        SavedTurbineProperties.SaveValue(floatProperty.propertyName, value);
    }


}
