using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridTile{

    public Vector3 position;
    public int biome;
    public GridTileOccupant occupant;
    public List<Vector3> vert;
    public List<int> vertIndices;

    // 0 is empty tile
    // 1 holds terrain generated object
    // 2 is self build. (Maybe more types)
    public int type;
    public bool canBuild;            

    public GridTile(Vector3 position, List<Vector3> vert, List<int> vertIndices, int biome, int type, bool canBuild, GridTileOccupant occupant)
    {
        this.position = position;
        this.biome = biome;
        this.canBuild = canBuild;
        this.type = type;
        this.occupant = occupant;
        this.vertIndices = vertIndices;
        this.vert = vert;
    }

    public static GridTile FindClosestGridTile(Vector3 point)
    {
        GridTile[,] world = TerrainController.thisTerrainController.world;
        int tileSize = TerrainController.thisTerrainController.tileSize;

        int x = Mathf.RoundToInt((point.x - 0.5f * tileSize) / tileSize);
        int z = Mathf.RoundToInt((point.z - 0.5f * tileSize) / tileSize);


        return world[x,z];
    }
}
