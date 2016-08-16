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

    TurbineManager turbManager; // Holder of turbines

    private static WorldController instance;

    [Header("Prefabs")]
    public GameObject weatherManagerPrefab;
    public GameObject terrainManagerPrefab;
    public GameObject turbineManagerPrefab;
    public GameObject buildingsManagerPrefab;
    public GameObject worldInteractionManagerPrefab;

    // Use this for initialization
    void Awake()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
    }

    void Start()
    {
        turbManager = TurbineManager.GetInstance();
    }


    void Update()
    {
        float dt = Time.deltaTime * GameResources.getGameSpeed();
        turbManager.Update(dt);
    }

    // Create the singletone for the WorldManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("WorldManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the WorldManager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(terrainManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(weatherManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(turbineManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(buildingsManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(worldInteractionManagerPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public static WorldController GetInstance()
    {
        return instance;
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
