using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SliderNumberController : MonoBehaviour {

    public Slider slider;

    void Start()
    {
        GetComponent<Text>().text = slider.value.ToString();
        slider.onValueChanged.AddListener(delegate { SetNumber(slider.value); });
    }

    void SetNumber(float number)
    {
        GetComponent<Text>().text = number.ToString();
    }
}
