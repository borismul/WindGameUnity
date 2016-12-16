using UnityEngine;
using System.Collections;

public class WeatherStationSpecificsController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GenerateProperties();
	}
	
    void GenerateProperties()
    {
        // Range
        //FloatProperty floatProperty = new FloatProperty("Range", "m",100, 20, 500, null, null, GetType().GetMethod("RangeCost"), null, this);
        //GetComponent<PropertiesController>().properties.floatProperty.Add(floatProperty);
    }

    public int RangeCost(float range)
    {
        return Mathf.RoundToInt(range * range);
    }
}
