using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridTile {

    public Vector3 position;
    public int biome;
    public GridTileOccupant occupant;

    // 0 is empty tile
    // 1 holds terrain generated object
    // 2 is self build. (Maybe more types)
    public int type;
    public bool canBuild;            

    public GridTile(Vector3 position, int biome, int type, bool canBuild, GridTileOccupant occupant)
    {
        this.position = position;
        this.biome = biome;
        this.canBuild = canBuild;
        this.type = type;
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
