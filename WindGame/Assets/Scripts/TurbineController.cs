using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class TurbineController : MonoBehaviour {
    [SerializeField]
    GameObject blades;       // Blades + Hub GameObject

    [SerializeField]
    GameObject nacelle;     // Nacelle GameObject (Contains the Blades + Hub as a child)

    float rotationSpeed;    // The current rotational speed of the blades
    float direction;        // Direction in which the turbine is pointed

    public TurbineProperties turbineProperties = new TurbineProperties();

    [HideInInspector]
    public string turbineName;
    //[HideInInspector]
    //public int numberOfBlades;
    //[HideInInspector]
    //public float FrontalArea;
    //[HideInInspector]
    //public float heightAboveGround;
    //[HideInInspector]
    //public bool hasWall;
    [HideInInspector]
    public float power;
    [HideInInspector]
    public double health = 1;
    [HideInInspector]
    public float avgPower;

    public float diameter;
    public float price;

    public bool canRotateAtBuild;

	// Update is called once per frame
	void Update ()
    {
        RotateTurbine();
        
    }

    public void Update(float gameDeltaTime)
    {
        if (GameResources.isPaused()) return;
        //UpdatePower(gameDeltaTime);
        UpdateHealth(gameDeltaTime);
    }

    void RotateTurbine()
    {
        // Set the rotation speed of the turbine with Method
        rotationSpeed = RotationSpeed(WindController.magnitude, 50);

        // Set the directional in which the turbine is pointed in the oposite direction of the wind
        direction = -WindController.direction;
        

        // Set the blades rotations in the three components depending on the rotation speed (only the z rotation changes)
        float bladesRotX = blades.transform.rotation.eulerAngles.x;
        float bladesRotY = blades.transform.rotation.eulerAngles.y;
        float bladesRotZ = blades.transform.rotation.eulerAngles.z + rotationSpeed * Time.deltaTime;

        blades.transform.rotation = Quaternion.Euler(new Vector3(bladesRotX, bladesRotY, bladesRotZ));

        if (!canRotateAtBuild)
        {
            // Set the nacelle rotations in the three components depending on the wind direction (only the y rotation changes)
            float nacelleRotX = nacelle.transform.rotation.eulerAngles.x;
            float nacelleRotY = direction;
            float nacelleRotZ = nacelle.transform.rotation.eulerAngles.z;

            // Give the gameobjects their right rotations

            nacelle.transform.rotation = Quaternion.Euler(nacelleRotX, nacelleRotY, nacelleRotZ);
        }
    }

    //void UpdatePower(float gameDeltaTime)
    //{
    //    float TSRInducedPower = OptimumCalculator(8, 3f, TSR);
    //    float pitchInducePower = OptimumCalculator(5, 15f, bladePitch);
    //    power = (float)health * TSRInducedPower * pitchInducePower * rotationSpeed;
    //    avgPower = (avgPower * weight + power) / (weight + 1);
    //    weight = weight + 1;
    //}

    void UpdateHealth(float gameDeltaTime)
    {
        //This assumes a turbine lifespan of 5 years
        //43800 being the amount of hours in 5 years
        double decay = ((double)1 / (double)43800) * (double)gameDeltaTime;
        if (health - decay < 0) return;
        health -= decay;
    }

    float OptimumCalculator(float optimum, float spread, float at)
    {
        float value = Mathf.Exp(-Mathf.Pow(at - optimum, 2f) / (2f * spread * spread));
        return value;
    }


    // Method that determines the rotation speed, based on the Incomming flow speed (Uinfinity), the tip speed ration (TSR) and the Radius of the acuator disk (R)
    float RotationSpeed(float Uinfinity, float R)
    {
        float omega = Uinfinity * 8 / R;
        return omega;

    }


}

public class TurbineProperties
{
    public List<FloatProperty> floatProperty;
    public List<IntProperty> intProperty;
    public List<BoolProperty> boolProperty;

    public TurbineProperties()
    {
        floatProperty = new List<FloatProperty>();
        intProperty = new List<IntProperty>();
        boolProperty = new List<BoolProperty>();
    }

}

public class FloatProperty
{
    public string propertyName;
    public string unit;
    public float property;
    public float minValue;
    public float maxValue;
    public System.Reflection.MethodInfo graphicsFunction;
    public object callObject;

    public FloatProperty(string propertyName, string unit, float property, float minValue, float maxValue, System.Reflection.MethodInfo graphicsFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.graphicsFunction = graphicsFunction;
        this.callObject = callObject;

    }
    public FloatProperty(string propertyName, string unit, float property, float minValue, float maxValue)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

public class IntProperty
{
    public string propertyName;
    public string unit;
    public int property;
    public int minValue;
    public int maxValue;
    public System.Reflection.MethodInfo graphicsFunction;
    public object callObject;

    public IntProperty(string propertyName, string unit, int property, int minValue, int maxValue, System.Reflection.MethodInfo graphicsFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.graphicsFunction = graphicsFunction;
        this.callObject = callObject;
    }
    public IntProperty(string propertyName, string unit, int property, int minValue, int maxValue)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

public class BoolProperty
{
    public string propertyName;
    public bool property;
    public System.Reflection.MethodInfo graphicsFunction;
    public object callObject;

    public BoolProperty(string propertyName, bool property, System.Reflection.MethodInfo graphicsFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.property = property;
        this.graphicsFunction = graphicsFunction;
        this.callObject = callObject;
    }

    public BoolProperty(string propertyName, bool property)
    {
        this.propertyName = propertyName;
        this.property = property;
    }

}
