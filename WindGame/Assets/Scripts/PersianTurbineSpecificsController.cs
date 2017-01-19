using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script contains the properties for the persian turbine. The properties are 
/// created as the turbine is created and are subsequently added to the TurbineController 
/// script. The functions for each the properties, calculation of power, cost, health 
/// degeneration and a graphics funcion, are determined in this script.
/// </summary>
[RequireComponent(typeof(PersianTurbineController))]
public class PersianTurbineSpecificsController : MonoBehaviour {

    public ObjectProperties properties;

    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float a = 0.15f;
    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float b = 0.1f;
    [Tooltip("Power Function: a + (nBlades/maxBlades)^b * c")]
    public float c = 0.1f;



    [Header("Wall Properties")]
    // Property variables
    public string wallPropertyName = "Use A Wall";
    public string wallUnit = "";
    public int wallCost = 1000;
    public float wallMultiplier = 1.3f;
    public bool wallIsOn = false;

    // Property prefabs
    public GameObject wall;

    // Properties, used for cross linking values to eachother
    BoolProperty wallProperty;

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
        properties = new ObjectProperties();

        // Wall
        wallProperty = new BoolProperty(wallPropertyName, wallIsOn, GetType().GetMethod("WallPower"), GetType().GetMethod("CreateWall"), GetType().GetMethod("WallCost"), null, this);
        properties.boolProperty.Add(wallProperty);
    }

    // Wall //
    public void CreateWall(bool isOn)
    {
        GameObject baseTurbineObject = GetComponent<UniversalProperties>().baseTurbineObject;
        if (isOn)
        {
            currentWall = (GameObject)Instantiate(wall, baseTurbineObject.transform.position, Quaternion.Euler(0, baseTurbineObject.transform.rotation.eulerAngles.y, 0), baseTurbineObject.transform);
        }
        else
        {
            if (currentWall != null)
                Destroy(currentWall);
        }
    }

    public int WallCost(bool isOn)
    {
        return 0;
        //if (isOn)
        //    return Mathf.RoundToInt(wallCost * areaProperty.propertyValue);
        //else
        //    return 0;
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


    // Function calculates a value between 0 and 1, when at == optimum it gives 1. Else it will 
    // give a value lower than 1, depending on the spread and how far at is from the optimum.
    float OptimumCalculator(float optimum, float spread, float at)
    {
        float value = Mathf.Exp(-Mathf.Pow(at - optimum, 2f) / (2f * spread * spread));
        return value;
    }

}
