using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UniversalProperties : MonoBehaviour {

    [Header("Height Properties")]
    // Property variables
    public string heightPropertyName = "Height";
    public string heightUnit = "m";
    public float heightStartValue = 0;
    public float heightMinValue = 0;
    public float heightMaxValue = 50;
    public float heightCostMultiplier = 3;
    public float baseStartScale = 1f;
    public FloatProperty heightProperty;

    // Property prefabs
    public GameObject turbineBasePrefab;

    [Header("Number Of Blades Properties")]
    // Property variables
    public string bladesPropertyName = "# Blades";
    public string bladesUnit = "";
    public int bladesStartValue = 3;
    public int bladesMinValue = 0;
    public int bladesMaxValue = 20;
    public IntProperty bladesProperty;

    // Property prefabs
    public GameObject persianBlade;

    [Header("Rated Speed/Cutoff Speed")]
    // Property variables
    public string RatedCutoffpropertyName = "Rated/Cuttoff Speed";
    public string ratedPropertyName = "Rated Speed";
    public string ratedCutoffUnit = "kW";
    public string cutoffPropertyName = "Cutoff Speed";
    public float ratedProperty = 5;
    public float cutOffProperty = 10;
    public float ratedCutoffMin = 1;
    public float ratedCuroffMax = 30;
    public MinMaxFloatProperty ratedCutoffProperty;

    [Header("Area Properties")]
    // Property variables
    public string areaPropertyName = "Frontal Area";
    public string areaUnit = "m^2";
    public float areaStartValue = 10;
    public float areaMinValue = 5;
    public float areaMaxValue = 25;
    public float areaCostMultiplier = 5;
    public float unitScaleArea = 7.5f;
    public FloatProperty areaProperty;

    [HideInInspector]
    public float Cp_design;

    [HideInInspector]
    public float Cp_reference;

    float height;                                               // Current height of the turbine
    List<GameObject> currentBlades = new List<GameObject>();    // The instantiated blades, if there is at least one

    public GameObject axis;                                 // Axis where the blades attach to
    public GameObject baseTurbineObject;                    // Turbine object without the base to make it heigher
    public GameObject turbineHouse;
    public GameObject turbineBase;

    void Awake()
    {
        GenerateProperties();
    }

    void GenerateProperties()
    {
        PropertiesController controller = GetComponent<PropertiesController>();

        // Frontal area
        areaProperty = new FloatProperty(areaPropertyName, areaUnit, areaStartValue, areaMinValue, areaMaxValue, null, GetType().GetMethod("ScaleByArea"), GetType().GetMethod("AreaCost"), null, this);
        controller.uniProperties.floatProperty.Add(areaProperty);

        // Height
        heightProperty = new FloatProperty(heightPropertyName, heightUnit, heightStartValue, heightMinValue, heightMaxValue, null, GetType().GetMethod("SetUpHeight"), GetType().GetMethod("HeightCost"), null, this);
        controller.uniProperties.floatProperty.Add(heightProperty);

        // Number of blades
        bladesProperty = new IntProperty(bladesPropertyName, bladesUnit, bladesStartValue, bladesMinValue, bladesMaxValue, null, GetType().GetMethod("CreateBlades"), null, null, this);
        controller.uniProperties.intProperty.Add(bladesProperty);

        // Rated/Cutoff Speed
        ratedCutoffProperty = new MinMaxFloatProperty(RatedCutoffpropertyName, ratedCutoffUnit, ratedPropertyName, cutoffPropertyName, ratedProperty, cutOffProperty, ratedCutoffMin, ratedCuroffMax, null, null, null, null, null, null, null, null, this);
        controller.uniProperties.minMaxProperty.Add(ratedCutoffProperty);

    }

    // Number of Blades //
    public void CreateBlades(int blades)
    {
        for (int i = 0; i < currentBlades.Count; i++)
            Destroy(currentBlades[i]);

        currentBlades.Clear();

        for (int i = 0; i < blades; i++)
            currentBlades.Add((GameObject)Instantiate(persianBlade, axis.transform.position, Quaternion.Euler(0, i * (360 / blades), 0), axis.transform));
    }

    public int BladesCost(int blades)
    {
        return 0;
        //return blades * costPerBlade;
    }

    // Height //
    public void SetUpHeight(float height)
    {
        float dh = height - this.height;
        this.height = height;
        transform.GetChild(0).position += Vector3.up * dh;
        if (height < 0)
            return;
        turbineBase.transform.localScale = (Vector3.right + Vector3.up) * areaProperty.propertyValue / unitScaleArea * baseStartScale + Vector3.forward * (heightProperty.propertyValue + 1);
        //turbineHouse.transform.localScale = (Vector3.right + Vector3.forward) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.up;
        transform.parent.position -= Vector3.up * dh * 0.5f;
        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (areaProperty.propertyValue / 2 + heightProperty.propertyValue) + (GetComponentInParent<Camera>().transform.forward) * 2 * (areaProperty.propertyValue / Mathf.Tan(30 * Mathf.Deg2Rad));
    }

    public int HeightCost(float height)
    {
        return Mathf.RoundToInt(heightCostMultiplier * height * height);
    }

    // Area //
    public int AreaCost(float area)
    {
        return Mathf.RoundToInt(areaCostMultiplier * area * area);
    }

    public void ScaleByArea(float area)
    {
        turbineBase.transform.localScale = (Vector3.right + Vector3.up) * areaProperty.propertyValue / unitScaleArea * baseStartScale + Vector3.forward * (heightProperty.propertyValue + 1);

        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (area / 2 + heightProperty.propertyValue) + (GetComponentInParent<Camera>().transform.forward) * 2 * (area * baseStartScale / Mathf.Tan(30 * Mathf.Deg2Rad));
        turbineHouse.transform.localScale = (Vector3.right + Vector3.forward) * areaProperty.propertyValue / unitScaleArea * baseStartScale + Vector3.up * baseStartScale;
        baseTurbineObject.transform.localScale = Vector3.one * areaProperty.propertyValue / unitScaleArea * baseStartScale;

        GetComponent<SizeController>().desiredScale = area / unitScaleArea;
    }

    // Rated Power //
    public int RatedCost(float ratedCost)
    {
        return 0;
        //return Mathf.RoundToInt(RatedCostMultiplier * ratedCost);
    }

    // Cutoff Power //
    public int CutoffCost(float cutoffCost)
    {
        return 0;
        //return Mathf.RoundToInt(CutoffCostMultiplier * cutoffCost);
    }

}
