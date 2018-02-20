using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WindController : MonoBehaviour {

    public static float direction;          // Direction in which the wind is moving    (Uniform)
    public static Vector3 windVector;
    public static float magnitude;          // Magnitude of the wind                    (Uniform)
    public static float time = 0;

    //Variables used for balancing
    public static float heightInfluence = 1/100f;
    public static float turbineHeight = 10;

    //Seasons contains the season index per month
    //Seasonvalues contains the coefficient of windspeed per season
    static int[] seasons = new int[] {0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
    static float[] seasonvalues = new float[] {1.15f, 1.02f, 1, 1 };

    //Hellman exponents used in calculations of the wind magnitude
    static float[] biomevalues = new float[] { 0.25f, 0.10f, 0.20f, 0.15f};

    public AudioSource windSound;

    public AudioClip[] windSounds;

    void Start()
    {
        //windSound = Camera.main.transform.Find("WindSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameResources.isPaused()) return;
            WindDirection(GameResources.getGameSpeed());
        magnitude = WindMagnitudeAtTime(time);
        //WindSound();


    }

    void WindSound()
    {
        float camHeight = Camera.main.GetComponentInParent<CameraController>().camHeight/ Camera.main.GetComponentInParent<CameraController>().maxHeight;
        windSound.volume = magnitude / 12 * 0.3f * camHeight*camHeight;

        windSound.pitch = 0.8f + (Mathf.PerlinNoise(time / 1.5f, time / 1.5f)) * 0.4f;

        if (windSound.isPlaying)
            return;

        windSound.clip = windSounds[Random.Range(0, windSounds.Length)];
        windSound.Play();
    }

    // Method that determines the wind direction
    // SHOULD BE UPDATED!
    static void WindDirection(float gameSpeed)
    {
        time += Time.deltaTime;
        direction = GetWinDirAtTime(time);
        windVector = GetWindAtTime(time);
    }

    static Vector3 GetWindAtTime(float time)
    {
        float direction = GetWinDirAtTime(time);
        Vector3 windVector = Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * direction), 0, Mathf.Cos(Mathf.Deg2Rad * direction)));

        return windVector;
    }

    static float GetWinDirAtTime(float time)
    {
        float direction = 180 * Mathf.Sin(0.021f * Mathf.PI * time) + 180 * Mathf.Cos(0.0133f * Mathf.PI * time);
        return direction;
    }
    // Method that determine the wind magnitude
    // SHOULD BE UPDATED!
    public static float WindMagnitudeAtTime(float time)
    {
        magnitude = 6 * Mathf.Abs(Mathf.Sin(0.02313f * Mathf.PI * time)) + 6 * Mathf.Abs(Mathf.Cos(0.0132f * Mathf.PI * time));
        return magnitude;
    }

    //Uses the windgradient formula to calculate the wind speed at different heights using the hellman exponent
    //Formula found on the wind gradient wikipedia page
    public static float GetWindAtTile(GridTile tile, float height, float time = -1)
    {
        //Calculates the base magnitude depending on the season
        float baseWind = magnitude * seasonvalues[seasons[GameResources.getDate().Month - 1]];

        //Gets the gridtiles up to 3 tiles around the target
        List<GridTile> nearTiles = tile.windEffectTiles;
        float blockedWind = 1;
        float deltaHeight; float dot; TerrainObject terrainObj;

        if (time != -1)
            windVector = GetWindAtTime(time);
        else
            windVector = GetWindAtTime(WindController.time);

        for(int i = 0; i < nearTiles.Count; i++)
        {
            Vector3 diff = nearTiles[i].position - tile.position;
            diff.y = 0;
            dot = Vector3.Dot(Vector3.Normalize(diff), windVector);
            if (dot > 0)
            {
                deltaHeight = nearTiles[i].position.y - tile.position.y - height;

                //Reduces the wind coefficient when the tiles around it are higher 
                //  Tiles block a maximum of 100% divided by their distance to the desired point each
                if (deltaHeight > 10)
                    blockedWind *= (1 - Mathf.Min(heightInfluence * Mathf.Pow(deltaHeight, 1.5f) * dot / (diff.magnitude * 10 + 1), 1));

                for (int j = 0; j < nearTiles[i].occupants.Count; j++)
                {
                    if (nearTiles[i].occupants[j] != null)
                    {
                        terrainObj = nearTiles[i].occupants[j].terrainObject;
                        //Checks wether a tile contains a terrainobject and adds the wind it blocks to windBlocked if the object is high enough
                        if (terrainObj != null)
                        {
                            WindEffectController controller = nearTiles[i].occupants[j].windEffectController;
                            if (deltaHeight + controller.objectHeight - height > 0)
                                blockedWind *= 1 - (controller.objectDensity / (diff.magnitude / 10 + 1)) * dot;
                            else
                            {
                                blockedWind *= 1 - (controller.objectDensity / ((diff.magnitude * -(deltaHeight + controller.objectHeight - height)) / 10 + 1)) * dot;
                            }

                        }
                    }
                }

            }
        }

        blockedWind = Mathf.Min(0.99f, blockedWind);

        return GetMaximumWind() * (blockedWind);
    }

    public static float GetMaximumWind()
    {
        return magnitude;
    }
}
