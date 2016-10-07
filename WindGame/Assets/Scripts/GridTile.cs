using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridTile{

    public Vector3 position;
    public int biome;
    public GridTileOccupant occupant;
    public bool underWater;
    public bool isOutsideBorder;
    public List<Vector3> vert;
    public List<int> vertIndex;
    public Chunk chunk;
    public GridTileOccupant.OccupantType type;

    public GridTile(Vector3 position, Chunk chunk, List<Vector3> vert, List<int> vertIndex, int biome, bool isUnderWater, bool isOutsideBorder, GridTileOccupant.OccupantType OccupantType, GridTileOccupant occupant)
    {
        this.position = position;
        this.biome = biome;
        this.type = OccupantType;
        this.occupant = occupant;
        this.vert = vert;
        this.underWater = isUnderWater;
        this.chunk = chunk;
        this.vertIndex = vertIndex;
        this.isOutsideBorder = isOutsideBorder;
    }

    public static GridTile FindClosestGridTile(Vector3 point)
    {
        
        GridTile[,] world = TerrainController.thisTerrainController.world;
        int tileSize = TerrainController.thisTerrainController.tileSize;

        int x = Mathf.FloorToInt((point.x +tileSize/2) / tileSize);
        int z = Mathf.FloorToInt((point.z + tileSize / 2) / tileSize);

        if (x >= world.GetLength(0) || x < 0)
            return null;

        if (z >= world.GetLength(1) || z < 0)
            return null;

        return world[x,z];
    }

    // Find all GridTiles in a radius around point
    public static GridTile[] FindGridTilesAround(Vector3 point, float circleRadius)
    {
        List<GridTile> gridTiles = new List<GridTile>();
        GridTile middleTile = FindClosestGridTile(point - new Vector3(TerrainController.thisTerrainController.tileSize, 0, TerrainController.thisTerrainController.tileSize));
        TerrainController terrain = TerrainController.thisTerrainController;

        float startTile = -circleRadius;
        float endTile = circleRadius;

        for (float i = startTile; i < endTile; i += terrain.tileSize)
        {
            for (float j = startTile; j < endTile; j += terrain.tileSize)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i, 0, middleTile.position.z + j));
                if (tile == null)
                    continue;

                if (Vector3.Distance(new Vector3(tile.position.x, 0, tile.position.z), new Vector3(point.x, 0, point.z)) < circleRadius)
                {
                    gridTiles.Add(FindClosestGridTile(tile.position));
                }
            }
        }

        return gridTiles.ToArray();
    }

    // Find all GridTiles in a radius around point with an added option to skip tiles in between tiles that are returned
    public static GridTile[] FindGridTilesAround(Vector3 point, float circleRadius, int ammountSkipTile)
    {
        List<GridTile> gridTiles = new List<GridTile>();
        GridTile middleTile = FindClosestGridTile(point);
        TerrainController terrain = TerrainController.thisTerrainController;

        float startTile = -circleRadius;
        float endTile = circleRadius;

        for (float i = startTile; i < endTile; i += terrain.tileSize * ammountSkipTile)
        {
            for (float j = startTile; j < endTile; j += terrain.tileSize * ammountSkipTile)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i, 0, middleTile.position.z + j));
                if (Vector3.Distance(new Vector3(tile.position.x, 0, tile.position.z), new Vector3(point.x, 0, point.z)) < circleRadius)
                {
                    gridTiles.Add(FindClosestGridTile(tile.position));
                }
            }
        }

        return gridTiles.ToArray();
    }


}
