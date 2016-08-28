using UnityEngine;
using System.Collections;

public class TurbineController : MonoBehaviour {
    [SerializeField]
    GameObject blades;       // Blades + Hub GameObject

    [SerializeField]
    GameObject nacelle;     // Nacelle GameObject (Contains the Blades + Hub as a child)

    float rotationSpeed;    // The current rotational speed of the blades
    float direction;        // Direction in which the turbine is pointed
    float weight = 1;

    public string turbineName;
    public float TSR = 8;          // Tip speed ratio of the turbine
    public float power;
    public double health = 1;
    public float avgPower;
    public float bladePitch = 5;

    public float diameter = 50;
    public float price = 5000;

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
        rotationSpeed = RotationSpeed(WindController.magnitude, TSR, 50);
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

    void UpdatePower(float gameDeltaTime)
    {
        float TSRInducedPower = OptimumCalculator(8, 3f, TSR);
        float pitchInducePower = OptimumCalculator(5, 15f, bladePitch);
        power = (float)health * TSRInducedPower * pitchInducePower * rotationSpeed;
        avgPower = (avgPower * weight + power) / (weight + 1);
        weight = weight + 1;
    }

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
    float RotationSpeed(float Uinfinity, float TSR, float R)
    {
        float omega = Uinfinity * 8 / R;
        return omega;
    }

}
