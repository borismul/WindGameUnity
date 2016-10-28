using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntPropertyController : MonoBehaviour {

    public Slider slider;
    public Text nameText;
    public InputField inputField;
    public Text unitText;

    [HideInInspector]
    public IntProperty intProperty;

	// Use this for initialization
	void Start ()
    {
        nameText.text = intProperty.propertyName;
        unitText.text = intProperty.unit;
        slider.minValue = intProperty.minValue;
        slider.maxValue = intProperty.maxValue;
        slider.value = intProperty.lastSetting;
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.text = slider.value.ToString();
        inputField.onValueChanged.AddListener(delegate { InputFieldChange(inputField.text); });
        slider.onValueChanged.AddListener(delegate { SliderChange(Mathf.RoundToInt(slider.value)); });
        InputFieldChange(inputField.text);
    }
	
	void InputFieldChange(string value)
    {
        if (value != string.Empty)
        {
            slider.value = int.Parse(value);
            intProperty.property = int.Parse(value);
            if(intProperty.graphicsFunction != null)
                intProperty.graphicsFunction.Invoke(intProperty.callObject, new object[] { Mathf.RoundToInt(slider.value) });
        }

        intProperty.lastSetting = int.Parse(value);
    }

    void SliderChange(int value)
    {
        inputField.text = value.ToString();
        intProperty.property = value;
        if (intProperty.graphicsFunction != null)
            intProperty.graphicsFunction.Invoke(intProperty.callObject, new object[] { Mathf.RoundToInt(slider.value) });

        intProperty.lastSetting = value;

    }
}
