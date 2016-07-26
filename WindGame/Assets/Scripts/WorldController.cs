using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour
{
    public string missionName;
    public System.DateTime date;
    public float capital;
    public float totalPower;
    public float costOfElectricity;
    public float interestRate;
    public float publicAcceptance;
    public float operationalCosts;
    public float gameSpeed;
    public System.Collections.Generic.List<GameObject> turbines;

    // Use this for initialization
    void Start()
    {
        GameObject[] turbinesObj = GameObject.FindGameObjectsWithTag("turbine");
        foreach (GameObject turbineObj in turbinesObj)
        {
            turbines.Add(turbineObj);
        }
        date = new System.DateTime(800, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        //If paused don't update
        if (gameSpeed == 0)
            return;

        capital += gameSpeed;
        //Calculate gameDeltaTime in hours
        //gameDeltaTime is amount of hours
        float gameDeltaTime = Time.deltaTime * gameSpeed;
        date = date.AddHours(gameDeltaTime);

        GameObject[] turbines = GameObject.FindGameObjectsWithTag("turbine");
        foreach (GameObject turbineObj in turbines)
        {
            TurbineController turbine = turbineObj.GetComponent<TurbineController>();
            turbine.Update(gameDeltaTime);
        }

        this.Update(gameDeltaTime);
    }

    void Update(float gameDeltaTime)
    {
        this.updatePower();
        this.updateOperationalCosts();
        this.updateCapital(gameDeltaTime);
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
