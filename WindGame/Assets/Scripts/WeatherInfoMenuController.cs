using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeatherInfoMenuController : MonoBehaviour {

    public Text windSpeed;
    public Text windDirection;

    public GameObject windRose;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        windSpeed.text = WindController.GetMaximumWind().ToString();
        windDirection.text = WindDirection();
	}

    string WindDirection()
    {
        float direction = WindController.direction;
        if (direction > -22.5f && direction < 22.5f)
            return "S";

        if (direction > 337.5f || direction < -337.5f)
            return "S";

        if (direction > 22.5f && direction < 67.5f)
            return "SW";

        if (direction > -337.5f && direction < -292.5f)
            return "SW";

        if (direction > 67.5f && direction < 112.5f)
            return "W";

        if (direction > -292.5f && direction < -247.5f)
            return "W";

        if (direction > 112.5f && direction < 157.5f)
            return "NW";

        if (direction > -247.5f && direction < -202.5f)
            return "NW";

        if (direction > 157.5f && direction < 202.5f)
            return "N";

        if (direction > -202.5f && direction < -157.5f)
            return "N";

        if (direction > 202.5f && direction < 247.5f)
            return "NE";

        if (direction > -157.5f && direction < -112.5f)
            return "NE";

        if (direction > 247.5f && direction < 292.5f)
            return "E";

        if (direction > -112.5f && direction < -67.5f)
            return "E";

        if (direction > 292.5f && direction < 337.5f)
            return "SE";

        if (direction > -67.5f && direction < 22.5f)
            return "SE";

        else
            return "wtf?";
    }
}
