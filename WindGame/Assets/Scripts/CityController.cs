using UnityEngine;
using System.Collections;
using Rand = System.Random;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CityController : MonoBehaviour {

    // THINGS SET IN UNITY
    // List containing the different types of buildings
    public CityObject[] buildings;

    [Range(0,1)]
    public float minLocX; // Minimal X value of city center start location

    [Range(0, 1)]
    public float maxLocX; // Maximal X value of city center start location

    [Range(0, 1)]
    public float minLocZ; // Minimal Z value of city center start location

    [Range(0, 1)]
    public float maxLocZ; // Maximal Z value of city center start location

    public float startRadius; 

    [Range(0, 1)]
    public float density;

    public GridTile centerTile;

    // THINGS UNITY DOES NOT TOUCH
    private float cityPointTimer;
    private float cityPoints;
    private float requiredCityPoints;
    private float maximumRadius;
    private float currentRadius;

    WorldController world;
    Rand rand = new Rand();

    // Singleton for outside use
    public static CityController city;

    // Use this for initialization
    void Start ()
    {
        city = this; // Set 'this' as the singleton

        world = WorldController.GetInstance(); // Get the world controller singleton

        TerrainController terrain = TerrainController.thisTerrainController;

        float xPos = minLocX * terrain.length + (float)rand.NextDouble() * (maxLocX - minLocX) * terrain.length;
        float zPos = minLocZ * terrain.width + (float)rand.NextDouble() * (maxLocZ - minLocZ) * terrain.width;

        // Create the city center
        Vector3 centerPos = new Vector3(xPos, 0, zPos);
        centerTile = GridTile.FindClosestGridTile(centerPos);
        cityPointTimer = 0;
        cityPoints = 0;
        requiredCityPoints = 8000;
        maximumRadius = 400;
        currentRadius = startRadius;

        BuildStartCity(); // Create an initial city
    }

    void Update()
    {
        //updateRadius();
        //Update city points every second
        if (cityPointTimer + Time.deltaTime >= 1)
        {
            cityPointTimer = 0;
            cityPoints += GameResources.getProduction();
        }
        else
        {
            cityPointTimer += Time.deltaTime;
        }

        //If city points higher than threshold, increase radius and threshold and reset points
        if (cityPoints > requiredCityPoints)
        {
            cityPoints -= requiredCityPoints;
            updateRadius();
        }
    }

    void updateRadius()
    {
        List<GridTile> gridTiles = GridTile.FindGridTilesAround(centerTile.position, currentRadius, 1).ToList();

        while (gridTiles.Count > 0)
        {
            // Grab a random tile  
            int currentTile = Random.Range(0, gridTiles.Count);
            GridTile tile = gridTiles[currentTile];

            // Grab a random building object
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            float diameter = buildObject.prefab.GetComponent<SizeController>().diameter;
            Quaternion rotation;

            // List of possible orientations
            float[] possibleOrientations = { 0, 90, 180, 270 }; // Mainly only cardinal directions

            // Get an angle from the possible orientation
            float angle = possibleOrientations[rand.Next(0, possibleOrientations.Length)];

            // Make a maximum of 10 degrees offset from the cardinal direction (purely for making it more visibly appealing)
            angle += (float)rand.NextDouble() * 10;

            // Create the rotation quaternion
            rotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true) && !world.BuildingNearby(tile.position, diameter))
            {
                world.AddOther(buildObject.prefab, tile.position, rotation, buildObject.scale, GridTileOccupant.OccupantType.City, transform);
                return;
            }

            // Remove the current tile we're inspecting because we couldn't build here; Try another.
            gridTiles.RemoveAt(currentTile);

        }
        // We couldn't build anywhere within the city radius. Expand the borders.
        if (currentRadius + 30 < maximumRadius)
        {
            currentRadius += 80;
        }
    }


    void BuildStartCity()
    {
        // Get the grid tiles to build on
        GridTile[] gridTiles = GridTile.FindGridTilesAround(centerTile.position, startRadius, 1);
        foreach (GridTile tile in gridTiles)
        {
            // Pick a type of building randomly
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            
            float diameter = buildObject.prefab.GetComponent<SizeController>().diameter;

            // Rotation quaternion for the building orientation
            Quaternion rotation;
            
            // List of possible orientations
            float[] possibleOrientations = { 0, 90, 180, 270 }; // Mainly only cardinal directions

            // Get an angle from the possible orientation
            float angle = possibleOrientations[rand.Next(0, possibleOrientations.Length)];

            // Make a maximum of 10 degrees offset from the cardinal direction (purely for making it more visibly appealing)
            angle += (float)rand.NextDouble() * 10;

            // Create the rotation quaternion
            rotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (!world.BuildingNearby(tile.position, diameter) && world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true))
            {
                // Place the building
                world.AddOther(buildObject.prefab, tile.position, rotation, buildObject.scale, GridTileOccupant.OccupantType.City, transform);
            }

        }
    }

    [System.Serializable]
    public struct CityObject
    {
        public GameObject prefab;
        public float scale;
    }
}
