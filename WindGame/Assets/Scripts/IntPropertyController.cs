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
        inputField.onValueChanged.AddListener(delegate { InputFieldChange(inputField.text); });
        slider.onValueChanged.AddListener(delegate { SliderChange(Mathf.RoundToInt(slider.value)); });
        nameText.text = intProperty.propertyName;
        unitText.text = intProperty.unit;
        slider.minValue = intProperty.minValue;
        slider.maxValue = intProperty.maxValue;
        int savedValue;
        if (SavedTurbineProperties.GetSavedValue(intProperty.propertyName, out savedValue))
            slider.value = savedValue;
        else
            slider.value = intProperty.propertyValue;
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.text = slider.value.ToString();

        InputFieldChange(inputField.text);
    }
	
	void InputFieldChange(string value)
    {
        if (value != string.Empty)
        {
            slider.value = int.Parse(value);
            //intProperty.property = int.Parse(value);
            //if (intProperty.graphicsFunction != null)
            //    intProperty.graphicsFunction.Invoke(intProperty.callObject, new object[] { Mathf.RoundToInt(slider.value) });
        }
    }

    void SliderChange(int value)
    {
        inputField.text = value.ToString();
        intProperty.propertyValue = value;
        if (intProperty.graphicsFunction != null)
            intProperty.graphicsFunction.Invoke(intProperty.callObject, new object[] { Mathf.RoundToInt(slider.value) });

        SavedTurbineProperties.SaveValue(intProperty.propertyName, value);
    }
}
