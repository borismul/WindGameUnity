using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridTile {

    public Vector3 position;
    public int biome;
    public bool canBuild;
    public GameObject occupant;
    public GameObject gridPointDebug;

	public GridTile(Vector3 position, int biome, bool canBuild, GameObject occupant, GameObject gridPointDebug)
    {
        this.position = position;
        this.biome = biome;
        this.canBuild = canBuild;
        this.occupant = occupant;
        this.gridPointDebug = gridPointDebug;
    }

    public GridTile(Vector3 position, int biome, bool canBuild, GameObject occupant)
    {
        this.position = position;
        this.biome = biome;
        this.canBuild = canBuild;
        this.occupant = occupant;
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
