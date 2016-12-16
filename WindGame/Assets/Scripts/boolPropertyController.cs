using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoolPropertyController : MonoBehaviour {

    public Toggle toggle;
    public Text nameText;

    [HideInInspector]
    public BoolProperty boolProperty;

    // Use this for initialization
    void Start()
    {
        toggle.onValueChanged.AddListener(delegate { ToggleChange(toggle.isOn); });

        nameText.text = boolProperty.propertyName;
        bool value;
        if (SavedTurbineProperties.GetSavedValue(boolProperty.propertyName, out value))
            toggle.isOn = value;
        else
            toggle.isOn = boolProperty.propertyValue;
    }

    void ToggleChange(bool isOn)
    {
        boolProperty.propertyValue = isOn;

        if (boolProperty.graphicsFunction != null)
            boolProperty.graphicsFunction.Invoke(boolProperty.callObject, new object[] { toggle.isOn });

        SavedTurbineProperties.SaveValue(boolProperty.propertyName, isOn);

    }


}
