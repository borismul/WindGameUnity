using UnityEngine;
using System.Collections;

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

    public static void SetOccupant(GridTile tile, GameObject occupant, float cutOffRadius)
    {
        tile.occupant = occupant;
        TerrainController terrain = TerrainController.thisTerrainController;
        int tileSize = terrain.tileSize;

        int thisX = Mathf.RoundToInt((tile.position.x - 0.5f * tileSize) / tileSize);
        int thisZ = Mathf.RoundToInt((tile.position.z - 0.5f * tileSize) / tileSize);

        int startX = thisX - Mathf.RoundToInt(cutOffRadius / tileSize)/2;
        int startZ = thisZ - Mathf.RoundToInt(cutOffRadius / tileSize)/2;

        int maxX = terrain.length / tileSize;
        int maxZ = terrain.width / tileSize;

        for (int i = 0; i<cutOffRadius/tileSize; i++)
        {
            for (int j = 0; j < cutOffRadius / tileSize; j++)
            {
                if (startX + i <= 0 || startX + i > maxX || startZ + j <= 0 || startZ + j > maxZ)
                    continue;

                GridTile checkGridTile = terrain.world[startX + i, startZ + j];
                if (Vector3.Distance(checkGridTile.position, tile.position) < cutOffRadius)
                {
                    checkGridTile.canBuild = false;
                }
            }
        }

    }
}
