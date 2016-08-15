using UnityEngine;
using System.Collections;
using System;

/**
	The purpose of this class is to keep track of the game resources e.g. wealth,
	production, population acceptance. Also the date is kept track of here since 
	time may be a (limited) resource available to the player.
	The world should inform this class of the production of all turbines. This class
	in turn will calculate the total other currencies to the player.
	This class contains functions to inform the state of the world that it needs to calculate all
	resources.
**/

public class GameResources : MonoBehaviour
{
	const float startingWealth = 10000.0f;
	const float initialAcceptance = 50;
	const float nuisanceModifier = 0.5f;

	static float wealth;			// Keep track of the player spendable resources
	static float productionRate;	// Keep track of the production rate
	static float production;		// Keep track of the total production
	static float publicAcceptance; // Keep track of public acceptance
	static DateTime date;

	float costOfElectricity = 0.002f;

	void Start () {
		// Set starting game resources
		wealth = startingWealth;
		production = 0;
		publicAcceptance = initialAcceptance;
		date = new DateTime(800, 10, 10);
	}
	
	// Update is called once per frame
	void Update () {
		updateResources();	
	}

	// Resource calculators
	void updateResources()
	{
		TurbineManager turbManager = TurbineManager.GetInstance();
		// **Update production
		production = turbManager.GetTotalProduction(); // * dt; // IMPLEMENT WHEN DT KNOWN

		// **Update wealth
		wealth += production * costOfElectricity; // Add 'sold' electricity to our wealth
		wealth -= turbManager.GetTotalMaintenanceCosts(); // Deduct the price of turbine maintenance

		// **Update public acceptance
		// The public gets more negative with more turbines built
		float buildingNuisance = turbManager.GetTurbineCount() * nuisanceModifier;
		publicAcceptance = production - buildingNuisance; // Public acceptance increases with power produced
		publicAcceptance /= 100; // Scaling factor
		publicAcceptance = 1.0f / (1.0f + (float)Math.Exp(-(publicAcceptance - 5.0f)));
		publicAcceptance *= 100; // Convert to %

		// Update time
		date = date.AddDays(1);
		
		// wealth += 0.8f * production;
	}

	public static bool BuyTurbine(float cost)
	{
		if(wealth < cost)
		{
			return false;
		}
		else
		{
			wealth -= cost;
			return true;
		}
	}

	// Getters for the private functions;
	public static float getWealth()
	{
		return wealth;
	}

	public static float getProduction()
	{
		return production;
	}

	public static float getPublicAcceptance()
	{
		return publicAcceptance;
	}

	public static DateTime getDate()
	{
		return date;
	}
}
