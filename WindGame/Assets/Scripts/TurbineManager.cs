using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
	The turbine manager SINGLETON is the object keeping track of all turbines.
	Contains functions for adding and removing turbines as well as
	functions for other classes to request specific information
**/
public sealed class TurbineManager {
	private List<GameObject> turbines = new List<GameObject>();

	private static readonly TurbineManager instance = new TurbineManager();
	
	private TurbineManager()
	{
		
	}

	public static TurbineManager GetInstance()
	{
		return instance;
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
    public float GetTotalMaintenanceCosts()
    {
        float sum = 0;
        foreach(GameObject turbine in turbines)
        {
            sum += turbine.GetComponent<TurbineController>().costOfMaintenance;
        }
        return sum;
    }
}
