using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class boolPropertyController : MonoBehaviour {

    public Toggle toggle;
    public Text nameText;

    [HideInInspector]
    public BoolProperty boolProperty;

    // Use this for initialization
    void Start()
    {
        nameText.text = boolProperty.propertyName;
        toggle.isOn = boolProperty.property;
        toggle.onValueChanged.AddListener(delegate { ToggleChange(toggle.isOn); });
    }

    void ToggleChange(bool isOn)
    {
        boolProperty.property = isOn;

        if (boolProperty.graphicsFunction != null)
            boolProperty.graphicsFunction.Invoke(boolProperty.callObject, new object[] { toggle.isOn });

    }


}
