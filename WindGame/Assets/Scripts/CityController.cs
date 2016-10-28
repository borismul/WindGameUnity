using UnityEngine;
using System.Collections;
using Rand = System.Random;
using UnityEngine.UI;

public class CityController : MonoBehaviour {

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
        world = WorldController.GetInstance();

        float xPos = minLocX * terrain.length + (float)rand.NextDouble() * (maxLocX - minLocX) * terrain.length;
        float zPos = minLocZ * terrain.width + (float)rand.NextDouble() * (maxLocZ - minLocZ) * terrain.width;
        Vector3 centerPos = new Vector3(xPos, 0, zPos);
        centerTile = GridTile.FindClosestGridTile(centerPos);
        cityPointTimer = 0;
        cityPoints = 0;
        requiredCityPoints = 1000;
        maximumRadius = 500;

        BuildStartCity();
    }

    void Update()
    {
        //Update city points every second
        if(cityPointTimer + Time.deltaTime >= 1)
        {
            cityPointTimer = 0;
            cityPoints += GameResources.getProduction();
            print(cityPoints);
        } else
        {
            cityPointTimer += Time.deltaTime;
        }

        //If city points higher than threshold, increase radius and threshold and reset points
        if (cityPoints > requiredCityPoints)
        {
            density += 0.05f;
            cityPoints = 0;
            updateRadius();
        }
    }

    void updateRadius()
    {
        GridTile[] gridTiles = GridTile.FindGridTilesAround(centerTile.position, currentRadius, 1);
        foreach (GridTile tile in gridTiles)
        {
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            if (rand.NextDouble() < density && world.CanBuild(tile.position, buildObject.prefab.GetComponent<SizeController>().diameter, true))
            {
                world.AddOther(buildObject.prefab, tile.position, Quaternion.LookRotation(new Vector3(tile.position.x, 0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z)), buildObject.scale, GridTileOccupant.OccupantType.City, transform);
                return;
            }
        }
        if (!(currentRadius + 30 > maximumRadius))
        {
            currentRadius += 30;
        }
    }


    void BuildStartCity()
    {
        currentRadius = startRadius;
        world.AddOther(buildings[0].prefab, centerTile.position , Quaternion.identity, buildings[0].scale, GridTileOccupant.OccupantType.City, transform);

        GridTile[] gridTiles = GridTile.FindGridTilesAround(centerTile.position, startRadius, 1);
        foreach (GridTile tile in gridTiles)
        {
            CityObject buildObject = buildings[rand.Next(0, buildings.Length)];

            if (rand.NextDouble() < density && world.CanBuild(tile.position, buildObject.prefab.GetComponent<SizeController>().diameter, true))
            {
                world.AddOther(buildObject.prefab, tile.position, Quaternion.LookRotation(new Vector3(tile.position.x,0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z)), buildObject.scale, GridTileOccupant.OccupantType.City, transform);
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
