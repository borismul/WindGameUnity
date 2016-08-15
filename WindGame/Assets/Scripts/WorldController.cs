using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
    The goal of this class is to keep track of all objects in the scene. 
    It should contain functions for adding and removing new objects as well as
    maintaining a list of all relevant objects.
**/

public class WorldController : MonoBehaviour
{
    public string missionName;
    public float gameSpeed;

    TurbineManager turbManager; // Holder of turbines

    // Use this for initialization
    void Start()
    {
        turbManager = TurbineManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
                // If paused don't update
        if (gameSpeed == 0)
            return;
        
        // Calculate the time passed in seconds since last frame
        float dt = Time.deltaTime * gameSpeed;

        Update(dt);
    }

    void Update(float idt)
    {
        // Update the turbine manager
        turbManager.Update(idt);
    }


    // Builder function, some class wants the world to add an object
    public static void Add(GameObject something, Vector3 pos)
    {
        GameObject t = Instantiate(something);
        t.transform.position = pos;
        t.tag = "turbine";
        
        TurbineManager turbManager = TurbineManager.GetInstance();
        turbManager.AddTurbine(t);
    }
}
