using UnityEngine;
using System.Collections;
using UnityEditor;

public class PersianTurbineSpecificsController : MonoBehaviour {

    public GameObject persianBlade;
    public GameObject wall;
    public GameObject axis;
    public GameObject turbineBasePrefab;

    public int costPerBlade = 20;
    public float heightCostMultiplier = 3;
    public float areaCostMultiplier = 5;
    public int wallCost = 1000;
    public float unitScaleArea = 9;

    TurbineController controller;

    public static GameObject previewTurbine;

    float height;
    GameObject curTurbineBase;
    // Use this for initialization


    void Awake()
    {
        Create();
    }

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
        float optimalValue = 7;
        float spread = 4;
        controller.turbineProperties.intProperty.Add(new IntProperty(propertyName, unit, Mathf.RoundToInt(property), Mathf.RoundToInt(minValue), Mathf.RoundToInt(maxValue), Mathf.RoundToInt(optimalValue), Mathf.RoundToInt(spread), GetType().GetMethod("CreateBlades"), GetType().GetMethod("BladesCost"), null, this));

        propertyName = "Frontal Area";
        unit = "m^2";
        property = 10;
        minValue = 1;
        maxValue = 50;
        optimalValue = 50;
        spread = 35;
        controller.turbineProperties.floatProperty.Add(new FloatProperty(propertyName, unit, property, minValue, maxValue, optimalValue, spread, GetType().GetMethod("ScaleByArea"), GetType().GetMethod("AreaCost"), null, this));

        propertyName = "Turbine Height";
        unit = "m";
        property = 0;
        minValue = 0;
        maxValue = 10;
        optimalValue = 10;
        spread = 20;
        controller.turbineProperties.floatProperty.Add(new FloatProperty(propertyName, unit, property, minValue, maxValue, optimalValue, spread, GetType().GetMethod("SetUpHeight"), GetType().GetMethod("HeightCost"), null, this));

        propertyName = "Has Wall";
        bool isOn = false;
        float multiplier = 1.2f;
        controller.turbineProperties.boolProperty.Add(new BoolProperty(propertyName, isOn, multiplier, GetType().GetMethod("CreateWall"), GetType().GetMethod("WallCost"), null, this));
    }

    public void CreateBlades(int blades)
    {
        for (int i = 0; i < previewTurbine.GetComponent<TurbinePreviewController>().blades.Count; i++)
            Destroy(previewTurbine.GetComponent<TurbinePreviewController>().blades[i]);

        previewTurbine.GetComponent<TurbinePreviewController>().blades.Clear();

        for (int i = 0; i< blades; i++)
            previewTurbine.GetComponent<TurbinePreviewController>().blades.Add((GameObject)Instantiate(persianBlade, previewTurbine.transform.position + Vector3.up *height, Quaternion.Euler(-90, i * (360 / blades), 0), axis.transform));
    }

    public int BladesCost(int blades)
    {
        return blades * costPerBlade;
    }

    public void CreateWall(bool isOn)
    {
        if (isOn)
        {
            previewTurbine.GetComponent<TurbinePreviewController>().wall = (GameObject)Instantiate(wall, previewTurbine.transform.position + Vector3.up *height, Quaternion.Euler(-90, 0, 0), previewTurbine.transform);
        }
        else
        {
            if(previewTurbine.GetComponent<TurbinePreviewController>().wall != null)
                Destroy(previewTurbine.GetComponent<TurbinePreviewController>().wall);
        }
    }

    public int WallCost(bool isOn)
    {
        if (isOn)
            return wallCost;
        else
            return 0;
    }

    public void SetUpHeight(float height)
    {
        if (curTurbineBase != null)
            Destroy(curTurbineBase);

        float dh = height - this.height;
        this.height = height;
        previewTurbine.transform.GetChild(0).position += Vector3.up * dh;
        curTurbineBase = (GameObject)Instantiate(turbineBasePrefab, previewTurbine.transform.position + Vector3.up * height/2, previewTurbine.transform.rotation, previewTurbine.transform);
        curTurbineBase.transform.localScale = new Vector3(curTurbineBase.transform.localScale.x, 0, curTurbineBase.transform.localScale.z) + Vector3.up * height;
        transform.parent.position -= Vector3.up * dh *0.5f;
    }

    public int HeightCost(float height)
    {
        return Mathf.RoundToInt(heightCostMultiplier * height * height);
    }


    public int AreaCost(float area)
    {
        return Mathf.RoundToInt(areaCostMultiplier * area * area);
    }

    public void ScaleByArea(float area)
    {
        previewTurbine.GetComponent<TurbineController>().desiredScale = area/unitScaleArea;
    }



}
