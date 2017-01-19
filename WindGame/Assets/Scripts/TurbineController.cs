using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

public abstract class TurbineController : MonoBehaviour {

    public float Cp_ref_min = 0;
    public float Cp_ref_max = 1;
    public float Vcutin = 1;
    public int n_aboveRated;
    public float eff;
    public bool safemode;
    public float penaltyControl;
    public float cp;

    [SerializeField]
    GameObject blades;       // Blades + Hub GameObject

    [SerializeField]
    GameObject nacelle;     // Nacelle GameObject (Contains the Blades + Hub as a child)

    float rotationSpeed;    // The current rotational speed of the blades
    float direction;        // Direction in which the turbine is pointed

    [HideInInspector]
    public float power;
    [HideInInspector]
    public float efficiency;
    [HideInInspector]
    public float health = 1;
    [HideInInspector]
    public float avgPower;
    [HideInInspector]
    public float desiredScale;
    [HideInInspector]
    public float desiredHeight;

    public string turbineName;

    float avgPowerCount = 0;

    public GridTile onGridtile;

    [HideInInspector]
    public UniversalProperties uniProperties;

    void Start()
    {
        uniProperties = GetComponent<UniversalProperties>();
        health = 1;
        onGridtile = GridTile.FindClosestGridTile(transform.position);
        startChild();
    }

    // This method starts the part of the Start cycle that is specific to the actual turbine being made.
    // This mainly prepares the specific properties.
    public abstract void startChild();

	// Update is called once per frame
	void Update ()
    {
        RotateTurbine();
    }

    public void Update(float gameDeltaTime)
    {
        if (GameResources.isPaused()) return;
        UpdatePower(gameDeltaTime);
        UpdateHealth(gameDeltaTime);
    }

    void RotateTurbine()
    {
        // Set the rotation speed of the turbine with Method
        rotationSpeed = power/8;

        // Set the directional in which the turbine is pointed in the oposite direction of the wind
        direction = -WindController.direction;

        // Set the blades rotations in the three components depending on the rotation speed (only the z rotation changes)
        float bladesRotX = blades.transform.rotation.eulerAngles.x;
        float bladesRotY = blades.transform.rotation.eulerAngles.y + rotationSpeed * Time.deltaTime;
        float bladesRotZ = blades.transform.rotation.eulerAngles.z;

        blades.transform.rotation = Quaternion.Euler(new Vector3(bladesRotX, bladesRotY, bladesRotZ));

        if (!GetComponent<BuildAttributes>().canRotateAtBuild)
        {
            // Set the nacelle rotations in the three components depending on the wind direction (only the y rotation changes)
            float nacelleRotX = nacelle.transform.rotation.eulerAngles.x;
            float nacelleRotY = direction;
            float nacelleRotZ = nacelle.transform.rotation.eulerAngles.z;

            // Give the gameobjects their right rotations

            nacelle.transform.rotation = Quaternion.Euler(nacelleRotX, nacelleRotY, nacelleRotZ);
        }
    }

    void UpdatePower(float gameDeltaTime)
    {
        cp = Cp();
        power = 0.5f * Cp() * 1.225f * uniProperties.areaProperty.propertyValue * Mathf.Pow(WindController.GetWindAtTile(onGridtile, uniProperties.heightProperty.propertyValue),3);
        //print(WindController.GetWindAtTile(onGridtile, uniProperties.heightProperty.propertyValue));
    }

    public abstract void SpecificProperties();

    public abstract ObjectProperties getSpecificProperties();

    float Cp()
    {
        int safemodeint = safemode ? 1 : 0;
        return CpDesign() * eff * Mathf.Sqrt(health) * (1 - safemodeint *  penaltyControl);
    }

    float CpReference()
    {
        float B = uniProperties.bladesProperty.propertyValue;
        float Bmin = uniProperties.bladesMinValue;
        float Bmax = uniProperties.bladesMaxValue;
        return Cp_ref_min + (Cp_ref_max - Cp_ref_min) * Mathf.Pow((B - Bmin)/(Bmax - Bmin),0.8f);
    }

    float CpDesign()
    {
        float V_rated = uniProperties.ratedCutoffProperty.minPropertyValue;
        float V_cutoff = uniProperties.ratedCutoffProperty.maxPropertyValue;
        float height = uniProperties.heightProperty.propertyValue;
        float V = WindController.GetWindAtTile(onGridtile, height);

        if (V < Vcutin) return 0;
        else if (Vcutin < V && V <= V_rated) return CpReference();
        else if (V_rated < V && V <= V_cutoff) return CpFormula(V, V_rated, V_cutoff);
        else return 0;
    }

    float CpFormula(float V, float V_rated, float V_cutoff)
    {
        float Cp_ref = CpReference();
        float BaseEq = Mathf.Sqrt(Mathf.Sin(Mathf.PI / 2 + (Mathf.PI / 4) * ((V - V_rated) / (V_cutoff - V_rated)))) * Mathf.Pow(V_rated / V, n_aboveRated);
        return Cp_ref * BaseEq;
    }

    void UpdateHealth(float gameDeltaTime)
    {
        //This assumes a turbine lifespan of 5 years
        //43800 being the amount of hours in 5 years
        float decay = (1 / 43800) * gameDeltaTime;
        if (health - decay < 0) return;
        health -= decay;
    }

    // Method that determines the rotation speed, based on the Incomming flow speed (Uinfinity), the tip speed ration (TSR) and the Radius of the acuator disk (R)
    float RotationSpeed(float Uinfinity, float R)
    {
        float omega = Uinfinity * 80 / R;
        return omega;
    }
}

public class ObjectProperties
{
    public List<FloatProperty> floatProperty;
    public List<IntProperty> intProperty;
    public List<BoolProperty> boolProperty;
    public List<MinMaxFloatProperty> minMaxProperty;

    public ObjectProperties()
    {
        floatProperty = new List<FloatProperty>();
        intProperty = new List<IntProperty>();
        boolProperty = new List<BoolProperty>();
        minMaxProperty = new List<MinMaxFloatProperty>();
    }
}

public class FloatProperty
{
    public string propertyName;
    public string unit;
    public float propertyValue;
    public float minValue;
    public float maxValue;
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;
    public float lastSetting;

    public object callObject;

    public FloatProperty(string propertyName, string unit, float propertyValue, float minValue, float maxValue, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.propertyValue = propertyValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.powerFunction = powerFunction; 
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;
        this.lastSetting = propertyValue;
    }
}

public class MinMaxFloatProperty
{
    public string propertyName;
    public string unit;
    public string minPropertyName;
    public string maxPropertyName;
    public float minPropertyValue;
    public float maxPropertyValue;
    public float minValue;
    public float maxValue;
    public MethodInfo minPowerFunction;
    public MethodInfo minGraphicsFunction;
    public MethodInfo minCostFunction;
    public MethodInfo minDegenFunction;
    public MethodInfo maxPowerFunction;
    public MethodInfo maxGraphicsFunction;
    public MethodInfo maxCostFunction;
    public MethodInfo maxDegenFunction;
    public float minLastSetting;
    public float maxLastSetting;

    public object callObject;

    public MinMaxFloatProperty(string propertyName, string unit, string minPropertyName, string maxPropertyName, float minPropertyValue, float maxPropertyValue, float minValue, float maxValue, MethodInfo minPowerFunction, MethodInfo maxPowerFunction, MethodInfo minGraphicsFunction, MethodInfo maxGraphicsFunction, MethodInfo minCostFunction, MethodInfo maxCostFunction, MethodInfo minDegenFunction, MethodInfo maxDegenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.minPropertyName = minPropertyName;
        this.maxPropertyName = maxPropertyName;
        this.minPropertyValue = minPropertyValue;
        this.maxPropertyValue = maxPropertyValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.minPowerFunction = minPowerFunction;
        this.minGraphicsFunction = minGraphicsFunction;
        this.minCostFunction = minCostFunction;
        this.minDegenFunction = minDegenFunction;
        this.maxPowerFunction = maxPowerFunction;
        this.maxGraphicsFunction = maxGraphicsFunction;
        this.maxCostFunction = maxCostFunction;
        this.maxDegenFunction = maxDegenFunction;
        this.callObject = callObject;
        this.minLastSetting = minPropertyValue;
        this.maxLastSetting = maxPropertyValue;
    }
}

public class IntProperty
{
    public string propertyName;
    public string unit;
    public int propertyValue;
    public int minValue;
    public int maxValue;
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;
    public object callObject;
    public int lastSetting;

    public IntProperty(string propertyName, string unit, int propertyValue, int minValue, int maxValue, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.propertyValue = propertyValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.powerFunction = powerFunction;
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;
        this.lastSetting = propertyValue;
    }
}

public class BoolProperty
{
    public string propertyName;
    public bool propertyValue;
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;
    public object callObject;
    public bool lastSetting;

    public BoolProperty(string propertyName, bool propertyValue, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.propertyValue = propertyValue;
        this.powerFunction = powerFunction;
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;
        this.lastSetting = propertyValue;

    }

}
