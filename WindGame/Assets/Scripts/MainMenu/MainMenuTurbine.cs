using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTurbine : MonoBehaviour {

    public Image rotor;
    public float rotationSpeed;

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update () {
        rotor.transform.localRotation = Quaternion.Euler(rotor.transform.localRotation.eulerAngles + new Vector3(0, 0, rotationSpeed)*Time.deltaTime);
	}
}
