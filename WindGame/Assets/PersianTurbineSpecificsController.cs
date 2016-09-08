using UnityEngine;
using System.Collections;
using UnityEditor;

public class PersianTurbineSpecificsController : MonoBehaviour {

    public GameObject persianBlade;
    public GameObject wall;
    public GameObject axis;
    public GameObject baseTurbineObject;
    public GameObject turbineBasePrefab;
    public GameObject curvedBladePrefab;

    [Header("Area Properties")]
    public float areaOptimalValue = 50;
    public float areaSpread = 35;
    public float areaCostMultiplier = 5;
    public float unitScaleArea = 9;

    [Header("Height Properties")]
    public float heightCostMultiplier = 3;
    public float heightOptimalValue = 10;
    public float heightSpread = 20;

    [Header("Number Of Blades Properties")]
    public int costPerBlade = 20;
    public int bladesOptimalValue = 7;
    public int bladesSpread = 4;

    [Header("Wall Properties")]
    public int wallCost = 1000;
    public float wallPlusCurvedMultiplier = 1.2f;
    public float wallMinCurvedMultiplier = 1;

    [Header("Curved Properties")]
    public int curvedCost = 500;
    public float curvedMinWallMultiplier = 0.8f;
    public float noCurvedNoWallMultiplier = 0f;

    FloatProperty areaProperty;
    FloatProperty heightProperty;
    IntProperty bladesProperty;
    BoolProperty wallProperty;
    BoolProperty curvedProperty; 

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

        bladesProperty = new IntProperty(propertyName, unit, Mathf.RoundToInt(property), Mathf.RoundToInt(minValue), Mathf.RoundToInt(maxValue), GetType().GetMethod("BladesPower"), GetType().GetMethod("CreateBlades"), GetType().GetMethod("BladesCost"), null, this);
        controller.turbineProperties.intProperty.Add(bladesProperty);

        propertyName = "Frontal Area";
        unit = "m^2";
        property = 10;
        minValue = 1;
        maxValue = 50;

        areaProperty = new FloatProperty(propertyName, unit, property, minValue, maxValue, GetType().GetMethod("AreaPower"), GetType().GetMethod("ScaleByArea"), GetType().GetMethod("AreaCost"), null, this);
        controller.turbineProperties.floatProperty.Add(areaProperty);

        propertyName = "Turbine Height";
        unit = "m";
        property = 0;
        minValue = 0;
        maxValue = 10;

        heightProperty = new FloatProperty(propertyName, unit, property, minValue, maxValue, GetType().GetMethod("HeightPower"), GetType().GetMethod("SetUpHeight"), GetType().GetMethod("HeightCost"), null, this);
        controller.turbineProperties.floatProperty.Add(heightProperty);

        propertyName = "Has Wall";
        bool isOn = false;

        wallProperty = new BoolProperty(propertyName, isOn, GetType().GetMethod("WallPower"), GetType().GetMethod("CreateWall"), GetType().GetMethod("WallCost"), null, this);
        controller.turbineProperties.boolProperty.Add(wallProperty);

        propertyName = "Curved Blades";
        isOn = false;

        curvedProperty = new BoolProperty(propertyName, isOn, GetType().GetMethod("CurvedPower"), GetType().GetMethod("CreateCurvedBlades"), GetType().GetMethod("CurvedCost"), null, this);
        controller.turbineProperties.boolProperty.Add(curvedProperty);
    }

    // Number of Blades //
    public void CreateBlades(int blades)
    {
        if (curvedProperty.property)
            CreateCurvedBlades(true);
        else
        {
            for (int i = 0; i < previewTurbine.GetComponent<TurbinePreviewController>().blades.Count; i++)
                Destroy(previewTurbine.GetComponent<TurbinePreviewController>().blades[i]);

            previewTurbine.GetComponent<TurbinePreviewController>().blades.Clear();

            for (int i = 0; i < blades; i++)
                previewTurbine.GetComponent<TurbinePreviewController>().blades.Add((GameObject)Instantiate(persianBlade, previewTurbine.transform.position + Vector3.up * height, Quaternion.Euler(-90, i * (360 / blades), 0), axis.transform));
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
            previewTurbine.GetComponent<TurbinePreviewController>().wall = (GameObject)Instantiate(wall, previewTurbine.transform.position + Vector3.up *height, Quaternion.Euler(-90, 0, 0), baseTurbineObject.transform);
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
        previewTurbine.transform.GetChild(0).position += Vector3.up * dh;
        if (height <= 0)
            return;

        curTurbineBase = (GameObject)Instantiate(turbineBasePrefab, previewTurbine.transform.position + Vector3.up * height/2, previewTurbine.transform.rotation, previewTurbine.transform);
        curTurbineBase.transform.localScale = new Vector3(curTurbineBase.transform.localScale.x, 0, curTurbineBase.transform.localScale.z) + Vector3.up * height;
        transform.parent.position -= Vector3.up * dh *0.5f;
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
        previewTurbine.GetComponent<TurbineController>().desiredScale = area/unitScaleArea;
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
            for (int i = 0; i < previewTurbine.GetComponent<TurbinePreviewController>().blades.Count; i++)
                Destroy(previewTurbine.GetComponent<TurbinePreviewController>().blades[i]);

            previewTurbine.GetComponent<TurbinePreviewController>().blades.Clear();

            for (int i = 0; i < blades; i++)
                previewTurbine.GetComponent<TurbinePreviewController>().blades.Add((GameObject)Instantiate(curvedBladePrefab, previewTurbine.transform.position + Vector3.up * height, Quaternion.Euler(-90, i * (360 / blades), 0), axis.transform));
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


    float OptimumCalculator(float optimum, float spread, float at)
    {
        float value = Mathf.Exp(-Mathf.Pow(at - optimum, 2f) / (2f * spread * spread));
        return value;
    }

}
