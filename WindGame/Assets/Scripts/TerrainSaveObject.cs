using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class TerrainSaveObject
{
    public MyVector3[,] map;
    public int[,] biomeMap;
    public MyVector3 chunkLoc;

    public TerrainSaveObject(Vector3[,] map, int[,] biomeMap, Vector3 chunkLoc)
    {
        MyVector3[,] myVec = new MyVector3[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                myVec[i,j] = new MyVector3(map[i, j]);
            }
                    
        }
        this.map = myVec;
        this.biomeMap = biomeMap;
        this.chunkLoc = new MyVector3(chunkLoc);
    }

    public Vector3[,] GetVec3Map()
    {
        Vector3[,] mapVec3 = new Vector3[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                mapVec3[i, j] = map[i, j].GetVec3();
            }
        }

        return mapVec3;
    }
}

[System.Serializable]
public class MyVector3
{
    float x;
    float y;
    float z;

    public MyVector3(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 GetVec3()
    {
        return new Vector3(x, y, z);
    }

}

[System.Serializable]
public class TerrainSaver
{
    public List<TerrainSaveObject> terrainSaveList;
    public int length;
    public int width;
    public int maxHeight;
    public int seed;

    public int terrainOctaves;
    public float terrainPersistance;
    public float terrainFrequency;
    public int biomeOctaves;
    public float biomePersistance;
    public float biomeFrequency;

    public int chunkSize;
    public int tileSize;
    public int tileSlope;

    public int waterChunkSize;
    public int waterTileSize;
    public int waterOctaves;
    public float waterPersistance;
    public float waterFrequency;
    public float waterLevel;
    public float maxWaveHeight;


    public TerrainSaver(List<TerrainSaveObject> list, int length, int width, int maxHeight, int seed, int terrainOctaves, float terrainPersistance, float terrainFrequency, int biomeOctaves, float biomePersistance, float biomeFrequency, int chunkSize, int tileSize, int tileSlope, int waterChunkSize, int waterTileSize, int waterOctaves, float waterPersistance, float waterFrequency, float waterLevel, float maxWaveHeight)
    {
        terrainSaveList = list;
        this.length = length;
        this.width = width;
        this.maxHeight = maxHeight;
        this.seed = seed;
        this.terrainOctaves = terrainOctaves;
        this.terrainPersistance = terrainPersistance;
        this.terrainFrequency = terrainFrequency;
        this.biomeFrequency = biomeFrequency;
        this.biomeOctaves = biomeOctaves;
        this.biomePersistance = biomePersistance;
        this.chunkSize = chunkSize;
        this.tileSize = tileSize;
        this.tileSlope = tileSlope;
        this.waterChunkSize = waterChunkSize;
        this.waterTileSize = waterTileSize;
        this.waterOctaves = waterOctaves;
        this.waterPersistance = waterPersistance;
        this.waterFrequency = waterFrequency;
        this.waterLevel = waterLevel;
        this.maxWaveHeight = maxWaveHeight;
    }
}
