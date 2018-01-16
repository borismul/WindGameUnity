using UnityEngine;

public class WeatherstationObject : BuildableObject {

    [Header("Weatherstation model parts")]
    public GameObject weatherStation;

    private float windDirection;
    private float windMagnitude;

    private void MeasureWind()
    {
        windDirection += 0.1f;
        windMagnitude = 1;
    }

    private void Update()
    {
        MeasureWind();
    }

}
