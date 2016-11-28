using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WindController : MonoBehaviour {

    public static float direction;          // Direction in which the wind is moving    (Uniform)
    public static float magnitude;          // Magnitude of the wind                    (Uniform)
    public static float time = 0;

    //Variables used for balancing
    public static float heightInfluence = 1f/200f;
    public static float turbineHeight = 10;

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
        direction = 180 * Mathf.Sin(0.041f * Mathf.PI * time) + 180 * Mathf.Cos(0.193f * Mathf.PI * time);
        return direction;
    }

    // Method that determine the wind magnitude
    // SHOULD BE UPDATED!
    float WindMagnitude()
    {
        magnitude = 6 * Mathf.Abs(Mathf.Sin(0.2313f * Mathf.PI * Time.timeSinceLevelLoad)) + 6 * Mathf.Abs(Mathf.Cos(0.132f * Mathf.PI * Time.timeSinceLevelLoad));
        return magnitude;
    }

    //Uses the windgradient formula to calculate the wind speed at different heights using the hellman exponent
    //Formula found on the wind gradient wikipedia page
    public static float GetWindAtTile(GridTile tile, float height)
    {

        //Calculates the base magnitude depending on the season
        float baseWind = magnitude * seasonvalues[seasons[GameResources.getDate().Month - 1]];

        //Gets the gridtiles up to 3 tiles around the target
        GridTile[] nearTiles = GridTile.FindGridTilesAround(tile.position, 80);
        float blockedWind = 0;
        float deltaHeight; float dot; TerrainObject terrainObj;
        float blockedTile =  0;
        for(int i = 0; i < nearTiles.Length; i++)
        {
            Vector3 diff = nearTiles[i].position - tile.position;
            diff.y = 0;
            dot = Vector3.Dot(Vector3.Normalize(diff), Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * direction), 0, Mathf.Cos(Mathf.Deg2Rad * direction))));
            if (dot > 0)
            {
                blockedTile =  0;
                deltaHeight = nearTiles[i].position.y - tile.position.y;

                //Reduces the wind coefficient when the tiles around it are higher 
                //  Tiles block a maximum of 100% divided by their distance to the desired point each
                if (deltaHeight > 5) blockedTile += Mathf.Min(deltaHeight * heightInfluence, 1/diff.magnitude);

                for(int j = 0; j< nearTiles[i].occupants.Count; j++)
                if (nearTiles[i].occupants[j] != null)
                {
                    terrainObj = nearTiles[i].occupants[j].terrainObject;
                    //Checks wether a tile contains a terrainobject and adds the wind it blocks to windBlocked if the object is heigh enough
                    if (terrainObj != null)
                    {
                        WindEffectController controller = nearTiles[i].occupants[j].windEffectController;
                        if (deltaHeight + controller.objectHeight > height) blockedTile += controller.objectDensity;
                    }
                }
                blockedWind += blockedTile * dot;
            }
        }
        //print("Total amount of tiles evaluated: " + nearTiles.Length);
        //print("Amount of tiles with terrainobjects in the direction of the wind: " + testDirection);
        //print("Coefficient of blocked wind: " + blockedWind);
        blockedWind = Mathf.Min(blockedWind, 0.9f);

        return GetMaximumWind() * (1f - blockedWind);
    }

    public static float GetMaximumWind()
    {
        return magnitude;
    }
}
