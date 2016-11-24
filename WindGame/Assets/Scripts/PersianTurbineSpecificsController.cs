using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script contains the properties for the persian turbine. The properties are 
/// created as the turbine is created and are subsequently added to the TurbineController 
/// script. The functions for each the properties, calculation of power, cost, health 
/// degeneration and a graphics funcion, are determined in this script.
/// </summary>
[RequireComponent(typeof(TurbineController))]
public class PersianTurbineSpecificsController : MonoBehaviour {

    public GameObject axis;                                 // Axis where the blades attach to
    public GameObject baseTurbineObject;                    // Turbine object without the base to make it heigher
    public GameObject turbineHouse;
    public GameObject turbineBase;

    [Header("Area Properties")]
    // Property variables
    public string areaPropertyName = "Frontal Area";
    public string areaUnit = "m^2";
    public float areaStartValue = 10;
    public float areaMinValue = 5;
    public float areaMaxValue = 50;
    public float areaCostMultiplier = 5;
    public float unitScaleArea = 9;

    [Header("Height Properties")]
    // Property variables
    public string heightPropertyName = "Height";
    public string heightUnit = "m";
    public float heightStartValue = 0;
    public float heightMinValue = 50;
    public float heightMaxValue = 50;
    public float heightCostMultiplier = 3;
    public float baseStartScale = 15;
    // Property prefabs
    public GameObject turbineBasePrefab;


    [Header("Number Of Blades Properties")]
    // Property variables
    public string bladesPropertyName = "# Blades";
    public string bladesUnit = "";
    public int bladesStartValue = 3;
    public int bladesMinValue = 0;
    public int bladesMaxValue = 20;
    public int costPerBlade = 20;

    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float a = 0.15f;
    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float b = 0.1f;
    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float c = 0.1f;
    // Property prefabs
    public GameObject persianBlade;


    [Header("Wall Properties")]
    // Property variables
    public string wallPropertyName = "Use A Wall";
    public string wallUnit = "";
    public int wallCost = 1000;
    public float wallMultiplier = 1.3f;
    public bool wallIsOn = false;

    // Property prefabs
    public GameObject wall;

    [Header("Rated Power/Cutoff Power")]
    // Property variables
    public string RatedCutoffpropertyName = "Rated/Cuttoff Power";
    public string ratedPropertyName = "Rated Power";
    public string ratedCutoffUnit = "kW";
    public float RatedCostMultiplier = 500;
    public float CutoffCostMultiplier = 500;
    public string cutoffPropertyName = "Cutoff Power";
    public float ratedProperty = 5;
    public float cutOffProperty = 10;
    public float ratedCutoffMin = 1;
    public float ratedCuroffMax = 30;

    // Properties, used for cross linking values to eachother
    FloatProperty areaProperty;
    FloatProperty heightProperty;
    IntProperty bladesProperty;
    BoolProperty wallProperty;
    MinMaxFloatProperty ratedCutoffProperty;

    PropertiesContainer controller;

    float height;                                               // Current height of the turbine
    List<GameObject> currentBlades = new List<GameObject>();    // The instantiated blades, if there is at least one
    GameObject currentWall;                                     // The instantiate wall, if there is one

    void Awake()
    {
        GenerateProperties();
    }
	
    // Generate the properties for the persian turbine
    void GenerateProperties()
    {
        controller = GetComponent<PropertiesContainer>();

        // Number of blades
        bladesProperty = new IntProperty(bladesPropertyName, bladesUnit, bladesStartValue, bladesMinValue, bladesMaxValue, GetType().GetMethod("BladesPower"), GetType().GetMethod("CreateBlades"), GetType().GetMethod("BladesCost"), null, this);
        controller.properties.intProperty.Add(bladesProperty);

        // Frontal area
        areaProperty = new FloatProperty(areaPropertyName, areaUnit, areaStartValue, areaMinValue, areaMaxValue, GetType().GetMethod("AreaPower"), GetType().GetMethod("ScaleByArea"), GetType().GetMethod("AreaCost"), null, this);
        controller.properties.floatProperty.Add(areaProperty);

        // Height
        heightProperty = new FloatProperty(heightPropertyName, heightUnit, heightStartValue, heightMinValue, heightMaxValue, GetType().GetMethod("HeightPower"), GetType().GetMethod("SetUpHeight"), GetType().GetMethod("HeightCost"), null, this);
        controller.properties.floatProperty.Add(heightProperty);

        // Wall
        wallProperty = new BoolProperty(wallPropertyName, wallIsOn, GetType().GetMethod("WallPower"), GetType().GetMethod("CreateWall"), GetType().GetMethod("WallCost"), null, this);
        controller.properties.boolProperty.Add(wallProperty);

        // Rated/Cutoff Power
        ratedCutoffProperty = new MinMaxFloatProperty(RatedCutoffpropertyName, ratedCutoffUnit, ratedPropertyName, cutoffPropertyName, ratedProperty, cutOffProperty, ratedCutoffMin, ratedCuroffMax, GetType().GetMethod("RatedPower"), GetType().GetMethod("CutoffPower"), null, null, GetType().GetMethod("RatedCost"), GetType().GetMethod("CutoffCost"), null, null, this);
        controller.properties.minMaxProperty.Add(ratedCutoffProperty);
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
        return blades * costPerBlade;
    }

    public float BladesPower(int blades, TurbineController controller)
    {
        float curPower = controller.power;
        float power = curPower * (a + Mathf.Pow((blades / bladesMaxValue), b) * c);
        return power;
    }

    // Wall //
    public void CreateWall(bool isOn)
    {
        if (isOn)
        {
            currentWall = (GameObject)Instantiate(wall, baseTurbineObject.transform.position, Quaternion.Euler(0, baseTurbineObject.transform.rotation.eulerAngles.y, 0), baseTurbineObject.transform);
        }
        else
        {
            if(currentWall != null)
                Destroy(currentWall);
        }
    }

    public int WallCost(bool isOn)
    {
        if (isOn)
            return Mathf.RoundToInt(wallCost * areaProperty.property);
        else
            return 0;
    }

    public float WallPower(bool isOn, TurbineController controller)
    {
        float curPower = controller.power;
        float power;
        if (isOn)
            power = curPower * wallMultiplier;
        else
            power = curPower;

        return power;
    }

    // Height //
    public void SetUpHeight(float height)
    {
        float dh = height - this.height;
        this.height = height;
        transform.GetChild(0).position += Vector3.up * dh;
        if (height < 0)
            return;
        turbineBase.transform.localScale = (Vector3.right + Vector3.up) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.forward * (heightProperty.property + 1);
        //turbineHouse.transform.localScale = (Vector3.right + Vector3.forward) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.up;
        transform.parent.position -= Vector3.up * dh * 0.5f;
        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (areaProperty.property / 2 + heightProperty.property) + (GetComponentInParent<Camera>().transform.forward) * 2 * (areaProperty.property / Mathf.Tan(30 * Mathf.Deg2Rad));
    }

    public int HeightCost(float height)
    {
        return Mathf.RoundToInt(heightCostMultiplier * height * height);
    }

    public float HeightPower(float height, TurbineController controller)
    {
        float windSpeed = WindController.GetWindAtTile(controller.onGridtile, height);
        return controller.power * windSpeed * windSpeed * windSpeed;
    }

    // Area //
    public int AreaCost(float area)
    {
        return Mathf.RoundToInt(areaCostMultiplier * area * area);
    }

    public void ScaleByArea(float area)
    {
        turbineBase.transform.localScale = (Vector3.right + Vector3.up) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.forward * (heightProperty.property + 1);

        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (area/2 + heightProperty.property) + (GetComponentInParent<Camera>().transform.forward) * 2 * (area *baseStartScale / Mathf.Tan(30 * Mathf.Deg2Rad));
        turbineHouse.transform.localScale = (Vector3.right + Vector3.forward) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.up * baseStartScale;
        baseTurbineObject.transform.localScale = Vector3.one * areaProperty.property / unitScaleArea * baseStartScale;

        GetComponent<SizeController>().desiredScale = area/unitScaleArea;
    }

    public float AreaPower(float area, TurbineController controller)
    {
        float curPower = controller.power;
        float power = curPower * area;
        return power;
    }

    // Rated Power //
    public int RatedCost(float ratedCost)
    {
        return Mathf.RoundToInt(RatedCostMultiplier * ratedCost);
    }

    public float RatedPower(float rated, TurbineController controller)
    {
        float curPower = controller.power;
        if (curPower > rated*1000)
            return rated* 1000;
        else
            return curPower;
    }

    // Cutoff Power //
    public int CutoffCost(float cutoffCost)
    {
        return Mathf.RoundToInt(CutoffCostMultiplier * cutoffCost);
    }

    public float CutoffPower(float cutoff, TurbineController controller)
    {
        float curPower = controller.power;

        if (curPower > cutoff*1000)
            return 0;
        else
            return curPower;
    }


    // Function calculates a value between 0 and 1, when at == optimum it gives 1. Else it will 
    // give a value lower than 1, depending on the spread and how far at is from the optimum.
    float OptimumCalculator(float optimum, float spread, float at)
    {
        float value = Mathf.Exp(-Mathf.Pow(at - optimum, 2f) / (2f * spread * spread));
        return value;
    }

}
