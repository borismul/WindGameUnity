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
    [HideInInspector]
    public GameObject preview;

    // Use this for initialization
    void Start()
    {
        nameText.text = floatProperty.propertyName;
        unitText.text = floatProperty.unit;
        slider.minValue = floatProperty.minValue;
        slider.maxValue = floatProperty.maxValue;
        slider.value = floatProperty.property;
        inputField.contentType = InputField.ContentType.DecimalNumber;
        inputField.text = slider.value.ToString();
        inputField.onValueChanged.AddListener(delegate { InputFieldChange(inputField.text); });
        slider.onValueChanged.AddListener(delegate { SliderChange(slider.value); });

    }

    void InputFieldChange(string value)
    {
        if (value != string.Empty)
        {
            slider.value = float.Parse(value);
            if(floatProperty.graphicsFunction != null)
                floatProperty.graphicsFunction.Invoke(floatProperty.callObject, new object[] { slider.value });

            floatProperty.property = float.Parse(value);
        }
    }

    void SliderChange(float value)
    {

        inputField.text = value.ToString();
        if (floatProperty.graphicsFunction != null)
            floatProperty.graphicsFunction.Invoke(floatProperty.callObject, new object[] { slider.value });

        floatProperty.property = value;

    }
}
