using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
	The turbine manager SINGLETON is the object keeping track of all turbines.
	Contains functions for adding and removing turbines as well as
	functions for other classes to request specific information
**/
public class TurbineManager : MonoBehaviour{
	private List<GameObject> turbines = new List<GameObject>();

    private static TurbineManager instance;

    // Use this for initialization
    void Awake()
    {
        CreateSingleton();
    }

    // Create the singletone for the TurbineManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("TurbineManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Get the singleton instance
    public static TurbineManager GetInstance()
    {
        return instance;
    }
    
    void Update()
    {

    }

    public void Update(float idt)
	{
		foreach(GameObject turbine in turbines)
		{
			turbine.GetComponent<TurbineController>().Update(idt);
		}
	}

	public void AddTurbine(GameObject turb)
	{
		turbines.Add(turb);
	}

    public void RemoveTurbine(GameObject turb)
    {
        turbines.Remove(turb);
        WorldController.GetInstance().RemoveTurbine(turb.GetComponent<TurbineController>());
    }

	// Returns the number of turbines in the world
    public int GetTurbineCount() 
    {
        return turbines.Count;
    }

    // In case some class, like the UI, wants to know the total production
    public float GetTotalProduction()
    {
        float sum = 0;
        foreach(GameObject turbine in turbines)
        {
            sum += turbine.GetComponent<TurbineController>().power;
        }
        return sum;
    }

    // The UI would like to know the total maintenance cost
    //public float GetTotalMaintenanceCosts()
    //{
    //    float sum = 0;
    //    foreach(GameObject turbine in turbines)
    //    {
    //        sum += turbine.GetComponent<TurbineController>().costOfMaintenance;
    //    }
    //    return sum;
    //}
}
