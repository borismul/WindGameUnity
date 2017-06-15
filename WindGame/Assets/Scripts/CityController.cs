using UnityEngine;
using System.Collections;
using Rand = System.Random;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CityController : MonoBehaviour {

    // List containing the different types of buildings
    public CityObject[] buildings;
    [Range(0,1)]
    public float minLocX;
    [Range(0, 1)]
    public float maxLocX;
    [Range(0, 1)]
    public float minLocZ;
    [Range(0, 1)]
    public float maxLocZ;
    public float startRadius;
    public float currentRadius;
    [Range(0, 1)]
    public float density;

    TerrainController terrain;
    WorldController world;
    Rand rand = new Rand();

    public GridTile centerTile;

    private float cityPointTimer;
    private float cityPoints;
    private float requiredCityPoints;
    private float maximumRadius;

    public static CityController city;

    // Use this for initialization
    void Start ()
    {
        city = this;

        terrain = TerrainController.thisTerrainController;
        world = WorldController.Instance;

        float xPos = minLocX * terrain.length + (float)rand.NextDouble() * (maxLocX - minLocX) * terrain.length;
        float zPos = minLocZ * terrain.width + (float)rand.NextDouble() * (maxLocZ - minLocZ) * terrain.width;
        Vector3 centerPos = new Vector3(xPos, 0, zPos);
        centerTile = GridTile.FindClosestGridTile(centerPos);
        cityPointTimer = 0;
        cityPoints = 0;
        requiredCityPoints = 8000;
        maximumRadius = 400;

        BuildStartCity();
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

        while (true)
        {
            int currentTile = Random.Range(0, gridTiles.Count);
            GridTile tile = gridTiles[currentTile];
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            float diameter = buildObject.prefab.GetComponent<SizeController>().diameter;
            Quaternion rotation;
            if (new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z) == Vector3.zero)
                rotation = Quaternion.identity;
            else
             rotation = Quaternion.LookRotation(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z));

            if (world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true) && !world.BuildingNearby(tile.position, diameter))
            {
                world.AddOther(buildObject.prefab, tile.position, Quaternion.LookRotation(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z)), buildObject.scale, GridTileOccupant.OccupantType.City, transform);
                return;
            }

            gridTiles.RemoveAt(currentTile);

            if (gridTiles.Count == 0)
            {
                gridTiles = GridTile.FindGridTilesAround(centerTile.position, currentRadius, 1).ToList();

                while (true)
                {
                    currentTile = Random.Range(0, gridTiles.Count);
                    tile = gridTiles[currentTile];
                    buildObject = buildings[rand.Next(0, buildings.Length)];

                    diameter = buildObject.prefab.GetComponent<SizeController>().diameter;
                    if(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z) == Vector3.zero)
                        rotation = Quaternion.identity;
                    else
                        rotation = Quaternion.LookRotation(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z));
                    if (world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true))
                    {
                        world.AddOther(buildObject.prefab, tile.position, Quaternion.LookRotation(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z)), buildObject.scale, GridTileOccupant.OccupantType.City, transform);
                        return;
                    }
                    gridTiles.RemoveAt(currentTile);

                    if (gridTiles.Count == 0)
                        break;
                }
                break;
            }

        }
        if (!(currentRadius + 30 > maximumRadius))
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

        // Second time we do this foreach loop for 'reasons'
        foreach (GridTile tile in gridTiles)
        {
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            float diameter = buildObject.prefab.GetComponent<SizeController>().diameter;
            Quaternion rotation;
            if (new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z) == Vector3.zero)
                rotation = Quaternion.identity;
            else
            {
                // List of possible orientations
                float[] possibleOrientations = { 0, 90, 180, 270 }; // Mainly only cardinal directions

                // Get an angle from the possible orientation
                float angle = possibleOrientations[rand.Next(0, possibleOrientations.Length)];

                // Make a maximum of 10 degrees offset from the cardinal direction (purely for making it more visibly appealing)
                angle += (float)rand.NextDouble() * 10;

                // Create the rotation quaternion
                rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }

            if (world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true))
            {
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
