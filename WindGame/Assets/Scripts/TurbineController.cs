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

    // Method that determines the rotation speed, based on the Incomming flow speed (Uinfinity), the tip speed ration (TSR) and the Radius of the acuator disk (R)
    float RotationSpeed(float Uinfinity, float TSR, float R)
    {
        float omega = Uinfinity * TSR / R;
        return omega;
    }

}
