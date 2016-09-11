using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

public class TurbineController : MonoBehaviour {
    [SerializeField]
    GameObject blades;       // Blades + Hub GameObject

    [SerializeField]
    GameObject nacelle;     // Nacelle GameObject (Contains the Blades + Hub as a child)

    float rotationSpeed;    // The current rotational speed of the blades
    float direction;        // Direction in which the turbine is pointed

    public TurbineProperties turbineProperties = new TurbineProperties();

    [HideInInspector]
    public float power;
    [HideInInspector]
    public float efficiency;
    [HideInInspector]
    public double health = 1;
    [HideInInspector]
    public float avgPower;
    [HideInInspector]
    public float desiredScale;
    [HideInInspector]
    public float desiredHeight;

    public string turbineName;
    public float diameter;
    public float price;

    public bool canRotateAtBuild;

    float avgPowerCount = 0;

    void Start()
    {
        health = 1;
    }
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
        rotationSpeed = RotationSpeed(WindController.magnitude, 50);

        // Set the directional in which the turbine is pointed in the oposite direction of the wind
        direction = -WindController.direction;
        

        // Set the blades rotations in the three components depending on the rotation speed (only the z rotation changes)
        float bladesRotX = blades.transform.rotation.eulerAngles.x;
        float bladesRotY = blades.transform.rotation.eulerAngles.y + rotationSpeed * Time.deltaTime;
        float bladesRotZ = blades.transform.rotation.eulerAngles.z;

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

    void UpdatePower(float gameDeltaTime)
    {
        power = 1;
        foreach (FloatProperty prop in turbineProperties.floatProperty)
        {
            power *= (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property });
            //print(prop.propertyName + ": " + (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property }));
        }

        foreach (IntProperty prop in turbineProperties.intProperty)
        {
            power *= (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property });
            //print(prop.propertyName + ": " + (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property }));

        }

        foreach (BoolProperty prop in turbineProperties.boolProperty)
        {
            power *= (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property });
            //print(prop.propertyName + ": " + (float)prop.powerFunction.Invoke(prop.callObject, new object[] { prop.property }));

        }

        power *= WindController.magnitude * WindController.magnitude * WindController.magnitude;

        efficiency = power / (WindController.magnitude * WindController.magnitude * WindController.magnitude);

        avgPower = (avgPower * avgPowerCount + power) / (avgPowerCount + 1);
        avgPowerCount++;
    }

    void UpdateHealth(float gameDeltaTime)
    {
        //This assumes a turbine lifespan of 5 years
        //43800 being the amount of hours in 5 years
        double decay = ((double)1 / (double)43800) * (double)gameDeltaTime;
        if (health - decay < 0) return;
        health -= decay;
    }

    // Method that determines the rotation speed, based on the Incomming flow speed (Uinfinity), the tip speed ration (TSR) and the Radius of the acuator disk (R)
    float RotationSpeed(float Uinfinity, float R)
    {
        float omega = Uinfinity * 80 / R;
        return omega;

    }

    public float CalculateCost()
    {
        float cost = 0;

        foreach (FloatProperty prop in turbineProperties.floatProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }
        foreach (IntProperty prop in turbineProperties.intProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }
        foreach (BoolProperty prop in turbineProperties.boolProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }

        return cost;
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
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;

    public object callObject;

    public FloatProperty(string propertyName, string unit, float property, float minValue, float maxValue, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.powerFunction = powerFunction; 
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;

    }
}

public class IntProperty
{
    public string propertyName;
    public string unit;
    public int property;
    public int minValue;
    public int maxValue;
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;
    public object callObject;

    public IntProperty(string propertyName, string unit, int property, int minValue, int maxValue, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.unit = unit;
        this.property = property;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.powerFunction = powerFunction;
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;
    }
}

public class BoolProperty
{
    public string propertyName;
    public bool property;
    public MethodInfo powerFunction;
    public MethodInfo graphicsFunction;
    public MethodInfo costFunction;
    public MethodInfo degenFunction;
    public object callObject;

    public BoolProperty(string propertyName, bool property, MethodInfo powerFunction, MethodInfo graphicsFunction, MethodInfo costFunction, MethodInfo degenFunction, object callObject)
    {
        this.propertyName = propertyName;
        this.property = property;
        this.powerFunction = powerFunction;
        this.graphicsFunction = graphicsFunction;
        this.costFunction = costFunction;
        this.degenFunction = degenFunction;
        this.callObject = callObject;
    }

}
