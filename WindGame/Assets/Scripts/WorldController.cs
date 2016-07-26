using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {
    public string missionName;
    public System.DateTime date;
    public float capital;
    public float totalPower;
    public float costOfElectricity;
    public float interestRate;
    public float publicAcceptance;
    public float operationalCosts;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        //Calculate deltaTime and call Update(float deltaTime) here
    }

	void Update (float deltaTime) {
        this.updatePower();
        this.updateOperationalCosts();
        this.updateCapital(deltaTime);
        this.updatePublicAcceptance();
	}

    void updatePower()
    {
        GameObject[] turbines = GameObject.FindGameObjectsWithTag("turbine");
        float pow = 0;
        foreach (GameObject turbineObj in turbines)
        {
            TurbineController turbine = turbineObj.GetComponent<TurbineController>();
            pow += turbine.baseOutput;
        }
        totalPower = pow;
    }

    void updateOperationalCosts()
    {
        GameObject[] turbines = GameObject.FindGameObjectsWithTag("turbine");
        float costs = 0;
        foreach (GameObject turbineObj in turbines)
        {
            TurbineController turbine = turbineObj.GetComponent<TurbineController>();
            costs += turbine.operationalCosts;
        }
        operationalCosts = costs;
    }

    void updateCapital(float deltaTime)
    {
        capital += totalPower * this.costOfElectricity / 1000000;
        double hourlyInterestRate = System.Math.Pow((1 + this.interestRate), 1 / 365 * 24);
        double variation = System.Math.Pow(hourlyInterestRate, deltaTime) - 1;
        double costOfCapital = System.Math.Min(this.capital * variation, 0);
        costOfCapital -= operationalCosts;
        capital += (float)costOfCapital * deltaTime;
    }

    void updatePublicAcceptance()
    {
        GameObject[] turbines = GameObject.FindGameObjectsWithTag("turbine");
        float negPublic = 0;
        foreach (GameObject turbineObj in turbines) 
        {
            negPublic += 0.5f;
        }
        publicAcceptance = totalPower / 2.1f + negPublic * 1;
    }
}
