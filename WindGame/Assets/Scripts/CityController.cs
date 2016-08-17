﻿using UnityEngine;
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
        GameObject building = terrain.BuildObject(buildings[0].prefab, Quaternion.Euler(-90, 0, 0), Vector3.one * buildings[0].scale, centerTile, startRadius, true, false, true);
        building.transform.SetParent(transform);
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
                    CityObject buildObject = buildings[rand.Next(0, buildings.Length)];
                    building = terrain.BuildObject(buildObject.prefab, Quaternion.Euler(-90,0,0), Vector3.one * buildObject.scale, checkGridTile, 30, false, false, true);
                    building.transform.SetParent(transform);
                }
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