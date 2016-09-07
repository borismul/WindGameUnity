using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour {

    public static float direction;          // Direction in which the wind is moving    (Uniform)
    public static float magnitude;          // Magnitude of the wind                    (Uniform)
    public static float time = 0;

    //Seasons contains the season index per month
    //Seasonvalues contains the coefficient of windspeed per season
    static int[] seasons = new int[] {0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
    static float[] seasonvalues = new float[] {1.15f, 1.02f, 1, 1 };

    //Hellman exponents used in calculations of the wind magnitude
    static float[] biomevalues = new float[] { 0.25f, 0.10f, 0.20f, 0.15f};

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
        magnitude = 6 * Mathf.Abs(Mathf.Sin(0.0023f * Mathf.PI * Time.timeSinceLevelLoad)) + 6 * Mathf.Abs(Mathf.Cos(0.0013f * Mathf.PI * Time.timeSinceLevelLoad));
        return magnitude;
    }

    //Uses the windgradient formula to calculate the wind speed at different heights using the hellman exponent
    //Formula found on the wind gradient wikipedia page
    public static float GetWindAtTile(GridTile tile)
    {
        //Calculates the base magnitude depending on the season
        float baseWind = magnitude * seasonvalues[seasons[GameResources.getDate().Month - 1]];

        //Gets the gridtiles up to five tiles around the target
        GridTile[] nearTiles = GridTile.FindGridTilesAround(tile.position, 2 * 20);
        float blockedWind = 0;
        float theta; float deltaHeight; float tileX;
        float tileY; float length; float dot; TerrainObject terrainObj;
        float testDirection = 0;

        for(int i = 0; i < nearTiles.Length; i++)
        {
            tileX = nearTiles[i].position.x - tile.position.x;
            tileY = nearTiles[i].position.z - tile.position.z;
            length = Mathf.Sqrt(Mathf.Pow(tileX, 2) + Mathf.Pow(tileY, 2));
            tileX = tileX / length;
            tileY = tileY / length;
            dot = tileX * -Mathf.Cos(direction) + tileY * -Mathf.Sin(direction);
            //theta = Mathf.Acos(dot);
            //theta = theta / (2 * Mathf.PI) * 360;

            if (dot > 0)
            {
                deltaHeight = nearTiles[i].position.y - tile.position.y;

                //Reduces the wind coefficient when the tiles around it are higher 
                //  Tiles block a maximum of 10% each and block the maximum amound when their y coordinate is 20 higher
                if (deltaHeight > 0) blockedWind += Mathf.Min(deltaHeight / 200, 0.1f);

                if (nearTiles[i].occupant != null)
                {
                    terrainObj = nearTiles[i].occupant.obj.GetComponent<TerrainObject>();
                    //Checks wether a tile contains a terrainobject and adds the wind it blocks to windBlocked
                    //INFLUENCE OF DIFFERENT TERRAINOBJECTS NOT IMPLEMENTED YET
                    if (terrainObj != null)
                    {
                        blockedWind += 0.1f;
                        testDirection++;
                    }
                }
            }
        }
        //print("Total amount of tiles evaluated: " + nearTiles.Length);
        //print("Amount of tiles with terrainobjects in the direction of the wind: " + testDirection);
        //print("Coefficient of blocked wind: " + blockedWind);

        return baseWind * (1 - blockedWind);
    }
}
