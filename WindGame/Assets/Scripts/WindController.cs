using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindController : MonoBehaviour {

    public static float direction;          // Direction in which the wind is moving    (Uniform)
    public static float magnitude;          // Magnitude of the wind                    (Uniform)

    private float time = 0;

    // Seasons contains the season index per month
    // Seasonvalues contains the coefficient of windspeed per season
    static int[] seasons = new int[] {0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
    static float[] seasonvalues = new float[] {1.15f, 1.02f, 1, 1 };

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
        time += Time.deltaTime * gameSpeed / 200.0f;

        direction = 180 * Mathf.Sin(0.021f * Mathf.PI * time) + 180 * Mathf.Cos(0.0133f * Mathf.PI * time);

        return direction;
    }

    // Method that determine the wind magnitude
    // SHOULD BE UPDATED!
    float WindMagnitude()
    {
        magnitude = 6 * Mathf.Abs(Mathf.Sin(0.02313f * Mathf.PI * Time.timeSinceLevelLoad)) + 6 * Mathf.Abs(Mathf.Cos(0.0132f * Mathf.PI * Time.timeSinceLevelLoad));

        return magnitude;
    }

    // Uses the windgradient formula to calculate the wind speed at different heights using the hellman exponent
    // Formula found on the wind gradient wikipedia page
    public static float GetWindAtTile(GridTile tile, float height)
    {
        //Calculates the base magnitude depending on the season
        float baseWind = magnitude * seasonvalues[seasons[GameResources.getDate().Month - 1]];

        //Gets the gridtiles up to 3 tiles around the target
        GridTile[] nearTiles = GridTile.FindGridTilesAround(tile.position, 80);
        float blockedWind = 0;

        // Iterate through the nearby tiles
        for(int i = 0; i < nearTiles.Length; i++)
        {
            // Calculate the distance between the target and the nearby tile currently under inspection
            Vector3 diff = nearTiles[i].position - tile.position;

            diff.y = 0; // Set the vertical component of the difference to 0

            // Calculate the dot product between the normalized difference vector and normalized wind direction vector
            float dot = Vector3.Dot(Vector3.Normalize(diff), Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * direction), 0, Mathf.Cos(Mathf.Deg2Rad * direction))));

            // If the dot product is larger than 0, we are blocking the wind.
            // dot < 0 implies we are BEHIND the target w.r.t. the wind
            // dot == 0 implies we are exactly 90 degrees next to the target w.r.t. to wind vector
            if (dot > 0)
            {
                float blockedTile =  1;
                float deltaHeight = nearTiles[i].position.y - tile.position.y - height;

                // Reduces the wind coefficient when the tiles around it are higher 
                // Tiles block a maximum of 100% divided by their distance to the desired point each
                if (deltaHeight > 10) blockedTile *=  (1-Mathf.Min(1/Mathf.Pow(diff.magnitude,0.7f) * heightInfluence *  Mathf.Pow(deltaHeight,1.1f) * dot, 1) );

                // Loop through the occupants of the tile we're currently investigating
                for (int j = 0; j < nearTiles[i].occupants.Count; j++)
                {
                    TerrainObject terrainObj = nearTiles[i].occupants[j].terrainObject;
                    //Checks wether a tile contains a terrainobject and adds the wind it blocks to windBlocked if the object is heigh enough
                    if (terrainObj != null)
                    {
                        WindEffectController controller = nearTiles[i].occupants[j].windEffectController;
                        if (deltaHeight + controller.objectHeight > height) blockedTile *= (1-controller.objectDensity);
                    }
                }
                    
                blockedWind += (1-blockedTile) * dot;
            }
        }

        blockedWind = Mathf.Min(0.99f, blockedWind);
        //print("Total amount of tiles evaluated: " + nearTiles.Length);
        //print("Amount of tiles with terrainobjects in the direction of the wind: " + testDirection);
        //print("Coefficient of blocked wind: " + blockedWind);

        return GetMaximumWind() * (1 - blockedWind);
    }

    public static float GetMaximumWind()
    {
        return magnitude;
    }
}
