using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour {

    public static float direction;          // Direction in which the wind is moving    (Uniform)
    public static float magnitude;          // Magnitude of the wind                    (Uniform)
    public static float time = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameResources.isPaused()) return;
        direction = WindDirection(GameResources.getGameSpeed());
        magnitude = WindMagnitude();
    }

    // Method that determines the wind direction
    // SHOULD BE UPDATED!
    float WindDirection(float gameSpeed)
    {
        time += Time.deltaTime * gameSpeed/200;
        direction = 180 * Mathf.Sin(0.01f * Mathf.PI * time) + 180 * Mathf.Cos(0.012f * Mathf.PI * time);
        return direction;
    }

    // Method that determine the wind magnitude
    // SHOULD BE UPDATED!
    float WindMagnitude()
    {
        magnitude = 1000 * Mathf.Abs(Mathf.Sin(0.0023f * Mathf.PI * Time.timeSinceLevelLoad)) + 1000 * Mathf.Abs(Mathf.Cos(0.0013f * Mathf.PI * Time.timeSinceLevelLoad));
        return magnitude;
    }
}
