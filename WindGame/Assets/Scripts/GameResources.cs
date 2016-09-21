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
    public static int currentMission = 1;

	const float startingWealth = 500000;
	const float initialAcceptance = 50;
	const float nuisanceModifier = 0.5f;

	static float wealth;			// Keep track of the player spendable resources
	static float productionRate;	// Keep track of the production rate
	static float production;		// Keep track of the total production
	static float publicAcceptance; // Keep track of public acceptance
	static DateTime date;
    static float gameSpeed;
    static bool paused;
    static string objectiveText;

	static float costOfElectricity = 0.002f;

	void Start () {
		// Set starting game resources
		wealth = startingWealth;
		production = 0;
		publicAcceptance = initialAcceptance;
		date = new DateTime(800, 10, 10);
        gameSpeed = 200;
        paused = true;
	}
	
    void Update()
    {
        // If paused don't update
        if (paused)
            return;

        // Calculate the time passed in seconds since last frame
        float dt = Time.deltaTime * gameSpeed;

        Update(dt);
    }

	// Update is called once per frame
	void Update (float gameDeltaTime) {

		updateResources(gameDeltaTime);
        
    }

	// Resource calculators
	static void updateResources(float gameDeltaTime)
	{
		TurbineManager turbManager = TurbineManager.GetInstance();
        turbManager.Update(gameDeltaTime);
		// **Update production
		production = turbManager.GetTotalProduction();

		// **Update wealth
		wealth += production * costOfElectricity * gameDeltaTime; // Add 'sold' electricity to our wealth
		//wealth -= turbManager.GetTotalMaintenanceCosts() * gameDeltaTime; // Deduct the price of turbine maintenance

		// **Update public acceptance
		// The public gets more negative with more turbines built
		float buildingNuisance = turbManager.GetTurbineCount() * nuisanceModifier;
		publicAcceptance = production - buildingNuisance; // Public acceptance increases with power produced
		publicAcceptance /= 100; // Scaling factor
		publicAcceptance = 1.0f / (1.0f + (float)Math.Exp(-(publicAcceptance - 5.0f)));
		publicAcceptance *= 100; // Convert to %

		// Update time
		date = date.AddHours(gameDeltaTime);
		
		// wealth += 0.8f * production;
	}

    public static bool CanIBuy(float cost)
    {
        if (wealth < cost)
        {
            return false;
        }
        else
        {
            return true;
        }
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

    public static bool removeWealth(float ammount)
    {
        if (wealth < ammount)
        {
            return false;
        }
        else
        {
            wealth -= ammount;
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

    public static float getGameSpeed()
    {
        return gameSpeed;
    }

    public static void setGameSpeed(float speed)
    {
        gameSpeed = speed;
    }

    public static void pause()
    {
        paused = true;
    }

    public static void unPause()
    {
        paused = false;
    }

    public static bool isPaused()
    {
        return paused;
    }

    public static void setObjectiveText(string text)
    {
        objectiveText = text;
    }

    public static string getObjectiveText()
    {
        return objectiveText;
    }
}
