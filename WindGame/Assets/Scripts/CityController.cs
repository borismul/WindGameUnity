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
    public int startRadius;
    public int currentRadius;
    [Range(0, 1)]
    public float density;

    TerrainController terrain;
    public WorldController world;
    Rand rand = new Rand();

    public GridTile centerTile;

    private float cityPointTimer;
    private float cityPoints;
    private float requiredCityPoints;
    private float maximumRadius;

    public static CityController city;

    int currentMaxHouses = 30;
    int currentPlacedHouses = 0;
    int counter;
    // Use this for initialization
    void Start ()
    {
        city = this;

        terrain = TerrainController.thisTerrainController;

        float xPos = minLocX * terrain.length + (float)rand.NextDouble() * (maxLocX - minLocX) * terrain.length;
        float zPos = minLocZ * terrain.width + (float)rand.NextDouble() * (maxLocZ - minLocZ) * terrain.width;
        Vector3 centerPos = new Vector3(xPos, 0, zPos);
        centerTile = GridTile.FindClosestGridTile(centerPos);
        cityPointTimer = 0;
        cityPoints = 0;
        requiredCityPoints = 8000;
        maximumRadius = 400;
        currentRadius = startRadius;
        StartCoroutine(BuildStartCity());
    }

    void Update()
    {



    }

    private void FixedUpdate()
    {
        currentMaxHouses++;
    }

    void UpdateCity()
    {
        if (currentMaxHouses >= currentPlacedHouses)
            return;
    }


    IEnumerator BuildStartCity()
    {
        GridTile[] gridTiles = GridTile.FindAnnulusAround(centerTile.position, currentRadius, 3);
        float timer = Time.realtimeSinceStartup;
        while (true)
        {
            while (currentMaxHouses > currentPlacedHouses)
            {
                // Get the grid tiles to build on
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
                    if (world.CanBuild(tile.position, diameter, buildObject.prefab, buildObject.scale, rotation, true))
                    {
                        // Place the building
                        world.AddOther(buildObject.prefab, tile.position, rotation, buildObject.scale, GridTileOccupant.OccupantType.City, transform);
                        currentPlacedHouses++;
                    }

                    if (currentPlacedHouses >= currentMaxHouses)
                        break;

                    if (Time.realtimeSinceStartup - timer > 1 / 60f)
                    {
                        yield return null;
                        timer = Time.realtimeSinceStartup;
                    }
                }


                if (currentPlacedHouses >= currentMaxHouses)
                    break;

                yield return null;

                currentRadius += 3;
                gridTiles = GridTile.FindAnnulusAround(centerTile.position, currentRadius, 3);

            }
            yield return null;
        }

    }

    [System.Serializable]
    public struct CityObject
    {
        public GameObject prefab;
        public float scale;
    }
}
