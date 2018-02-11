using UnityEngine;

public class TurbineObject : BuildableObject
{
    [Header("Turbine model parts")]
    public GameObject turbineBase;
    public GameObject turbineBlade;
    public GameObject turbineTop;

    [Header("Turbine condition specifics")]
    public float health = 1;    // possible to be damaged (over time / events)
    public float efficiency = 1;
    [HideInInspector]
    public float currentPower;

    [Header("Turbine building specifics")]
    public float height; // height of the model, should be changed to model scale of different elements.
    public int blades;
    public float bladeDiameter;

    // Make variables that are adjustable from the build menu a separate type
    // this way, the buildMenu can instantiate the turbineObject from a button,
    // and check which paramters should show up in the build menu parameter sliders
    // then it can accordingly couple sliders and booleans and integer boxes to those quantities

    // Public method section
    void ProducePower()
    {
        currentPower = efficiency * (1-health) * 1f;
    }

    private void Update()
    {
        ProducePower();
    }

}
