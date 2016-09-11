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

    [Header("Area Properties")]
    // Property variables
    public string areaPropertyName = "Frontal Area";
    public string areaUnit = "m^2";
    public float areaStartValue = 10;
    public float areaMinValue = 5;
    public float areaMaxValue = 50;
    public float areaOptimalValue = 50;
    public float areaSpread = 35;
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
    public float heightOptimalValue = 50;
    public float heightSpread = 20;
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
    public int bladesOptimalValue = 7;
    public int bladesSpread = 4;
    // Property prefabs
    public GameObject persianBlade;


    [Header("Wall Properties")]
    // Property variables
    public string wallPropertyName = "Use A Wall";
    public string wallUnit = "";
    public int wallCost = 1000;
    public float wallPlusCurvedMultiplier = 1f;
    public float wallMinCurvedMultiplier = .8f;
    public bool wallIsOn = false;

    // Property prefabs
    public GameObject wall;

    [Header("Curved Properties")]
    // Property variables
    public string curvedPropertyName = "Use Curved Blades";
    public string curvedUnit = "m";
    public int curvedCost = 500;
    public float curvedMinWallMultiplier = 0.6f;
    public float noCurvedNoWallMultiplier = 0f;
    public bool curvedIsOn = false;

    // Property prefabs
    public GameObject curvedBladePrefab;


    // Properties, used for cross linking values to eachother
    FloatProperty areaProperty;
    FloatProperty heightProperty;
    IntProperty bladesProperty;
    BoolProperty wallProperty;
    BoolProperty curvedProperty; 
    
    TurbineController controller;

    float height;                                               // Current height of the turbine
    GameObject curTurbineBase;                                  // The instantiated base, if there is one
    List<GameObject> currentBlades = new List<GameObject>();    // The instantiated blades, if there is at least one
    GameObject currentWall;                                     // The instantiate wall, if there is one

    void Awake()
    {
        GenerateProperties();
    }
	
    // Generate the properties for the persian turbine
    void GenerateProperties()
    {
        controller = GetComponent<TurbineController>();

        // Number of blades
        bladesProperty = new IntProperty(bladesPropertyName, bladesUnit, bladesStartValue, bladesMinValue, bladesMaxValue, GetType().GetMethod("BladesPower"), GetType().GetMethod("CreateBlades"), GetType().GetMethod("BladesCost"), null, this);
        controller.turbineProperties.intProperty.Add(bladesProperty);

        // Frontal area
        areaProperty = new FloatProperty(areaPropertyName, areaUnit, areaStartValue, areaMinValue, areaMaxValue, GetType().GetMethod("AreaPower"), GetType().GetMethod("ScaleByArea"), GetType().GetMethod("AreaCost"), null, this);
        controller.turbineProperties.floatProperty.Add(areaProperty);

        // Height
        heightProperty = new FloatProperty(heightPropertyName, heightUnit, heightStartValue, heightMinValue, heightMaxValue, GetType().GetMethod("HeightPower"), GetType().GetMethod("SetUpHeight"), GetType().GetMethod("HeightCost"), null, this);
        controller.turbineProperties.floatProperty.Add(heightProperty);

        // Wall
        wallProperty = new BoolProperty(wallPropertyName, wallIsOn, GetType().GetMethod("WallPower"), GetType().GetMethod("CreateWall"), GetType().GetMethod("WallCost"), null, this);
        controller.turbineProperties.boolProperty.Add(wallProperty);

        // Curved Blades
        curvedProperty = new BoolProperty(curvedPropertyName, curvedIsOn, GetType().GetMethod("CurvedPower"), GetType().GetMethod("CreateCurvedBlades"), GetType().GetMethod("CurvedCost"), null, this);
        controller.turbineProperties.boolProperty.Add(curvedProperty);
    }

    // Number of Blades //
    public void CreateBlades(int blades)
    {
        if (curvedProperty.property)
            CreateCurvedBlades(true);
        else
        {
            for (int i = 0; i < currentBlades.Count; i++)
                Destroy(currentBlades[i]);

            currentBlades.Clear();

            for (int i = 0; i < blades; i++)
                currentBlades.Add((GameObject)Instantiate(persianBlade, transform.position + Vector3.up * height, Quaternion.Euler(-90, i * (360 / blades), 0), axis.transform));
        }
    }

    public int BladesCost(int blades)
    {
        return blades * costPerBlade;
    }

    public float BladesPower(int blades)
    {
        return OptimumCalculator(bladesOptimalValue, bladesSpread, blades);
    }

    // Wall //
    public void CreateWall(bool isOn)
    {
        if (isOn)
        {
            currentWall = (GameObject)Instantiate(wall, transform.position + Vector3.up *height, Quaternion.Euler(-90, 0, 0), baseTurbineObject.transform);
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
            return wallCost;
        else
            return 0;
    }

    public float WallPower(bool isOn)
    {
        if (curvedProperty.property && isOn)
            return wallPlusCurvedMultiplier;

        else if (!curvedProperty.property && isOn)
            return wallMinCurvedMultiplier;

        else if (curvedProperty.property && !isOn)
            return curvedMinWallMultiplier;

        else
            return noCurvedNoWallMultiplier;
    }

    // Height //
    public void SetUpHeight(float height)
    {
        if (curTurbineBase != null)
            Destroy(curTurbineBase);


        float dh = height - this.height;
        this.height = height;
        transform.GetChild(0).position += Vector3.up * dh;
        if (height <= 0)
            return;

        curTurbineBase = (GameObject)Instantiate(turbineBasePrefab, transform.position + Vector3.up * height/2, transform.rotation, transform);
        curTurbineBase.transform.localScale = (Vector3.right + Vector3.forward) * areaProperty.property / unitScaleArea * baseStartScale + Vector3.up * heightProperty.property;
        transform.parent.position -= Vector3.up * dh *0.5f;
        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (areaProperty.property / 2 + heightProperty.property) + (GetComponentInParent<Camera>().transform.forward) * 2 * (areaProperty.property / Mathf.Tan(30 * Mathf.Deg2Rad));


    }

    public int HeightCost(float height)
    {
        return Mathf.RoundToInt(heightCostMultiplier * height * height);
    }

    public float HeightPower(float height)
    {
        return OptimumCalculator(heightOptimalValue, heightSpread, height);
    }

    // Area //
    public int AreaCost(float area)
    {
        return Mathf.RoundToInt(areaCostMultiplier * area * area);
    }

    public void ScaleByArea(float area)
    {
        baseTurbineObject.transform.localScale = Vector3.one *area/unitScaleArea;

        if (curTurbineBase != null)
            curTurbineBase.transform.localScale = (Vector3.right + Vector3.forward) * area/unitScaleArea * baseStartScale + Vector3.up * heightProperty.property;

        transform.parent.position = GetComponentInParent<Camera>().transform.position - Vector3.up * (area/2 + heightProperty.property) + (GetComponentInParent<Camera>().transform.forward) * 2 * (area / Mathf.Tan(30 * Mathf.Deg2Rad));
        GetComponent<TurbineController>().desiredScale = area/unitScaleArea;
    }

    public float AreaPower(float area)
    {
        return OptimumCalculator(areaOptimalValue, areaSpread, area);
    }

    // Curved //
    public void CreateCurvedBlades(bool isOn)
    {
        int blades = bladesProperty.property;
        
        if (!isOn)
            CreateBlades(blades);

        else
        {
            for (int i = 0; i < currentBlades.Count; i++)
                Destroy(currentBlades[i]);

            currentBlades.Clear();

            for (int i = 0; i < blades; i++)
                currentBlades.Add((GameObject)Instantiate(curvedBladePrefab, transform.position + Vector3.up * height, Quaternion.Euler(-90, i * (360 / blades), 0), axis.transform));
        }

    }

    public int CurvedCost(bool isOn)
    {
        if (isOn)
            return curvedCost;
        else
            return 0;
    }

    public float CurvedPower(bool isOn)
    {
        return 1;
    }

    // Function calculates a value between 0 and 1, when at == optimum it gives 1. Else it will 
    // give a value lower than 1, depending on the spread and how far at is from the optimum.
    float OptimumCalculator(float optimum, float spread, float at)
    {
        float value = Mathf.Exp(-Mathf.Pow(at - optimum, 2f) / (2f * spread * spread));
        return value;
    }

}
