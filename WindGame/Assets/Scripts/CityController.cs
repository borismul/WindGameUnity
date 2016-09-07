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
    [Range(0, 1)]
    public float density;

    TerrainController terrain;
    WorldController world;
    Rand rand = new Rand();

    public GridTile centerTile;

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

        BuildStartCity();
    }


    void BuildStartCity()
    {
        world.AddOther(buildings[0].prefab, centerTile.position , Quaternion.identity, buildings[0].scale, GridTileOccupant.OccupantType.City, 50, transform);

        GridTile[] gridTiles = GridTile.FindGridTilesAround(centerTile.position, startRadius, 1);
        foreach (GridTile tile in gridTiles)
        {
            if (rand.NextDouble() < density && world.CanBuild(tile.position, 50, true))
            {
                CityObject buildObject = buildings[rand.Next(0, buildings.Length)];
                world.AddOther(buildObject.prefab, tile.position, Quaternion.LookRotation(new Vector3(tile.position.x,0, tile.position.z) - new Vector3(centerTile.position.x, 0, centerTile.position.z)), buildObject.scale, GridTileOccupant.OccupantType.City, 50, transform);
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}

    [System.Serializable]
    public struct CityObject
    {
        public GameObject prefab;
        public float scale;
    }
}
