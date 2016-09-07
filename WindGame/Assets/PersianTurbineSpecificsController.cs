using UnityEngine;
using System.Collections;
using UnityEditor;

public class PersianTurbineSpecificsController : MonoBehaviour {

    public GameObject persianBlade;
    public GameObject wall;

    TurbineController controller;

    public static GameObject previewTurbine;
    // Use this for initialization

    public void Create () {
        controller = GetComponent<TurbineController>();
        PersianTurbine();
	}
	
    // Persian Turbine Section
    void PersianTurbine()
    {
        string propertyName = "# Blades";
        string unit = "";
        float property = 3;
        float minValue = 0;
        float maxValue = 20;
        controller.turbineProperties.intProperty.Add(new IntProperty(propertyName, unit, Mathf.RoundToInt(property), Mathf.RoundToInt(minValue), Mathf.RoundToInt(maxValue), GetType().GetMethod("CreateBlades"), this));

        propertyName = "Frontal Area";
        unit = "m^2";
        property = 10;
        minValue = 1;
        maxValue = 50;
        controller.turbineProperties.floatProperty.Add(new FloatProperty(propertyName, unit, property, minValue, maxValue));

        propertyName = "Turbine Height";
        unit = "m";
        property = 0;
        minValue = 0;
        maxValue = 10;
        controller.turbineProperties.floatProperty.Add(new FloatProperty(propertyName, unit, property, minValue, maxValue));

        propertyName = "Has Wall";
        unit = "m";
        bool isOn = false;

        controller.turbineProperties.boolProperty.Add(new BoolProperty(propertyName, isOn, GetType().GetMethod("CreateWall"), this));
    }

    public void CreateBlades(int blades)
    {
        for (int i = 0; i < previewTurbine.GetComponent<TurbinePreviewController>().blades.Count; i++)
            Destroy(previewTurbine.GetComponent<TurbinePreviewController>().blades[i]);

        previewTurbine.GetComponent<TurbinePreviewController>().blades.Clear();

        for (int i = 0; i< blades; i++)
            previewTurbine.GetComponent<TurbinePreviewController>().blades.Add((GameObject)Instantiate(persianBlade, previewTurbine.transform.position, Quaternion.Euler(-90, i * (360 / blades), 0), previewTurbine.transform));
    }

    public void CreateWall(bool isOn)
    {
        if (isOn)
        {
            previewTurbine.GetComponent<TurbinePreviewController>().wall = (GameObject)Instantiate(wall, previewTurbine.transform.position, Quaternion.Euler(-90, 0, 0), previewTurbine.transform);
        }
        else
        {
            if(previewTurbine.GetComponent<TurbinePreviewController>().wall != null)
                Destroy(previewTurbine.GetComponent<TurbinePreviewController>().wall);
        }
    }
}
