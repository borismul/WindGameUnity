using UnityEngine;
using System.Collections;
using Rand = System.Random;
using UnityEngine.UI;

public class CityController : MonoBehaviour {

    public GameObject[] buildings;
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
    Rand rand = new Rand();

    public GridTile centerTile;

    public static CityController city;

    // Use this for initialization
    void Start ()
    {
        city = this;
        terrain = TerrainController.thisTerrainController;
        float xPos = minLocX * terrain.length + (float)rand.NextDouble() * (maxLocX - minLocX) * terrain.length;
        float zPos = minLocZ * terrain.width + (float)rand.NextDouble() * (maxLocZ - minLocZ) * terrain.width;

        Vector3 centerPos = new Vector3(xPos, 0, zPos);
        centerTile = GridTile.FindClosestGridTile(centerPos);

        BuildStartCity();
    }


    void BuildStartCity()
    {
        terrain.BuildObject(buildings[0], centerTile.position, startRadius, true, false, true);

        int tileSize = terrain.tileSize;

        int thisX = Mathf.RoundToInt((centerTile.position.x - 0.5f * tileSize) / tileSize);
        int thisZ = Mathf.RoundToInt((centerTile.position.z - 0.5f * tileSize) / tileSize);

        int startX = thisX - Mathf.RoundToInt(startRadius / tileSize);
        int startZ = thisZ - Mathf.RoundToInt(startRadius / tileSize);

        int maxX = terrain.length / tileSize;
        int maxZ = terrain.width / tileSize;

        for (int i = 0; i < startRadius / tileSize * 2; i++)
        {
            for (int j = 0; j < startRadius / tileSize * 2; j++)
            {
                if (startX + i < 0 || startX + i > maxX || startZ + j < 0 || startZ + j > maxZ)
                    continue;
                GridTile checkGridTile = terrain.world[startX + i, startZ + j];
                if (Vector3.Distance(checkGridTile.position, centerTile.position) < startRadius && rand.NextDouble() < density && checkGridTile.canBuild)
                {
                    terrain.BuildObject(buildings[rand.Next(0, buildings.Length-1)], checkGridTile, 30, false, false, true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}
}
