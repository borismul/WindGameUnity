using UnityEngine;
using System.Collections;

public class TurbineController : MonoBehaviour {

    [SerializeField]
    GameObject blades;       // Blades + Hub GameObject

    [SerializeField]
    GameObject nacelle;     // Nacelle GameObject (Contains the Blades + Hub as a child)

    float rotationSpeed;    // The current rotational speed of the blades
    float direction;        // Direction in which the turbine is pointed
    float TSR = 8;          // Tip speed ratio of the turbine
    float R = 50;           // Radius of the actuator disk of the turbine

    //Parameters we came up with for a turbine
    public int buildProgress;
    public float height;
    public int nrBlades;

    //All parameters of a turbine that Carlos has in his code
    //Really vague, must discuss at meeting
    public float repairCosts;
    public int modeOfOperation = 0;
    public float revenue1;
    public float revenue2;
    public float randTerm;
    public float maximumHealth;
    public float totalCosts;
    public float health;
    public float power;
    public float powerDelivered;
    public float costOfMaintenance;
    
    //Initialize parameters use in the functions of the object, used if no input provided
    public float parameterRatedPower = 2.3f;
    public float parameterCapacityFactor = 0.25f;
    //Power lost if in safe mode of operation
    public float parameterPowerFractionsSafeModeOperation = 0.8f;
    //Power lost due to inefficiency
    public float parameterPowerFractionLoss = 0.2f;
    public float parameterCostWEU = 1.8f;
    //hal health in xx years - last number
    public float parameterHealth = (float)(1 * System.Math.Log(0.5) / (24 * 30 * 12 * 30));
    public float parameterHealthModeOperation = 0.5f;
    public float parameterLosses = 0.95f;
    public float parameterMaintenanceWEU1 = 0.25f;
    public float parameterMaintenanceRepairWEU = 0.5f;
    public float parameterValueEnergy1 = 0;
    public float parameterValueEnergy2 = 0;
    public float parameterRevenue2 = 0;
    public float publicAcceptance = -0.1f;
    public float parameterAcceptanceWEU1 = 0;
    public float parameterAcceptanceWEU2 = 0;


	// Update is called once per frame
	void Update ()
    {
        // Set the rotation speed of the turbine with Method
        rotationSpeed = RotationSpeed(WindController.magnitude, TSR, R); 
        // Set the directional in which the turbine is pointed in the oposite direction of the wind
        direction = -WindController.direction;

        // Set the blades rotations in the three components depending on the rotation speed (only the z rotation changes)
        float bladesRotX = blades.transform.rotation.eulerAngles.x;
        float bladesRotY = blades.transform.rotation.eulerAngles.y;
        float bladesRotZ = blades.transform.rotation.eulerAngles.z + rotationSpeed * Time.deltaTime;

        // Set the nacelle rotations in the three components depending on the wind direction (only the y rotation changes)
        float nacelleRotX = nacelle.transform.rotation.eulerAngles.x;
        float nacelleRotY = direction;
        float nacelleRotZ = nacelle.transform.rotation.eulerAngles.z;

        // Give the gameobjects their right rotations
        blades.transform.rotation = Quaternion.Euler(new Vector3(bladesRotX, bladesRotY, bladesRotZ));
        nacelle.transform.rotation = Quaternion.Euler(nacelleRotX, nacelleRotY, nacelleRotZ);
    }

    void repairWEU()
    {
        repairCosts = (float)(-1 * System.Math.Sqrt(1 - health) * parameterMaintenanceRepairWEU);
        float newHealth = (float)(maximumHealth * 0.75 + 0.25 * health);
        maximumHealth = newHealth;
        health = newHealth;
    }

    void updateHealth(float deltaGameTime)
    {
        health = (float)(health * System.Math.Exp(parameterHealth * deltaGameTime * (1 - modeOfOperation * parameterHealthModeOperation)));
    }

    void updatePower()
    {
        power = (float)(parameterCapacityFactor * parameterRatedPower * (0.9 + 0.2 * randTerm) * System.Math.Sqrt(health) * (1 - modeOfOperation * (1 - parameterPowerFractionsSafeModeOperation))) * 100;
        powerDelivered = power * (1 - parameterPowerFractionLoss);
    }

    void updateCostOfMaintenance(float gameDeltaTime)
    {
        costOfMaintenance = (float)((gameDeltaTime / (12 * 24 * 20)) * (-1 * System.Math.Sqrt(1 - health) * parameterMaintenanceWEU1));
    }

    public void Update(float gameDeltaTime)
    {
        updateHealth(gameDeltaTime);
        updatePower();
        updateCostOfMaintenance(gameDeltaTime);
        totalCosts = costOfMaintenance + repairCosts;
        repairCosts = 0;
    }

    // Method that determines the rotation speed, based on the Incomming flow speed (Uinfinity), the tip speed ration (TSR) and the Radius of the acuator disk (R)
    float RotationSpeed(float Uinfinity, float TSR, float R)
    {
        float omega = Uinfinity * TSR / R;
        return omega;
    }

}
