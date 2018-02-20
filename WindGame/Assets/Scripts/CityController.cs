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

    List<CityBuildingManager> curBuildings = new List<CityBuildingManager>();

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
        currentMaxHouses += 1;
    }

    void UpdateCity()
    {
        if (currentMaxHouses >= currentPlacedHouses)
            return;
    }


    IEnumerator BuildStartCity()
    {
        List<GridTile> gridTiles = GridTile.FindAnnulusAround(centerTile.position, currentRadius, 3);
         // Mainly only cardinal directions

        while (true)
        {
            while (currentMaxHouses > currentPlacedHouses)
            {
                // Get the grid tiles to build on
                foreach (GridTile tile in gridTiles)
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                        BuildBuilding(tile);
                    else
                        UpgradeBuilding();
                    if (currentPlacedHouses >= currentMaxHouses)
                        break;
                    yield return null;
                }

                yield return null;

                if (currentRadius < 100)
                {
                    currentRadius += 3;
                    gridTiles = GridTile.FindAnnulusAround(centerTile.position, currentRadius, 3);
                }
                else
                {
                    yield break;
                }



            }
            yield return null;
        }

    }

    void BuildBuilding(GridTile tile)
    {
        // Pick a type of building randomly
        CityObject buildObject = buildings[rand.Next(0, buildings.Length)];


        float diameter = buildObject.prefabs[0].GetComponent<SizeController>().diameter;

        // Rotation quaternion for the building orientation
        Quaternion rotation;

        // List of possible orientations
        float[] possibleOrientations = { 0, 90, 180, 270 };

        // Get an angle from the possible orientation
        float angle = possibleOrientations[rand.Next(0, possibleOrientations.Length)];

        // Make a maximum of 10 degrees offset from the cardinal direction (purely for making it more visibly appealing)
        //angle += (float)rand.NextDouble() * 10;

        // Create the rotation quaternion
        rotation = Quaternion.AngleAxis(angle, Vector3.up);
        if (world.CanBuild(tile.position, diameter, buildObject.prefabs[0], buildObject.scale, rotation, true))
        {
            GameObject parentObj = new GameObject();
            parentObj.transform.parent = transform;
            CityBuildingManager parentController = parentObj.AddComponent<CityBuildingManager>();
            parentController.cityObject = buildObject;
            // Place the building
            GameObject cityObj = world.AddOther(buildObject.prefabs[0], tile.position + Vector3.down*0.1f, rotation, buildObject.scale, GridTileOccupant.OccupantType.City, parentObj.transform);
            parentController.curObject = cityObj;

            curBuildings.Add(parentController);

            currentPlacedHouses++;
        }


    }

    void UpgradeBuilding()
    {
        if (curBuildings.Count == 0)
            return;

        int index = RouletteSelection();
        CityBuildingManager upgBuilding = curBuildings[index];
        curBuildings.RemoveAt(index);
        upgBuilding.Upgrade();
        curBuildings.Add(upgBuilding);
    }

    int RouletteSelection()
    {
        int n = curBuildings.Count;
        int index = Mathf.FloorToInt(Random.Range(0,1f) * (n * (n + 1) / 2));
        int temp = 0;


        for (int i = n-1; i >= 0; i--)
        {
            temp = temp + (n - (i - 1));


            if (index < temp)
                return i;
            
        }

        return 0;

    }


}

[System.Serializable]
public struct CityObject
{
    public GameObject[] prefabs;
    public float scale;
}
