using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridTile{

    public Vector3 position;
    public int biome;
    public List<GridTileOccupant> occupants;
    public bool underWater;
    public bool isOutsideBorder;
    public bool canSeeWind;
    public List<GridNode> gridNodes;
    public Chunk chunk;

    public GridTile(Vector3 position, Chunk chunk, List<GridNode> gridNodes, int biome, bool isUnderWater, bool isOutsideBorder, List<GridTileOccupant> occupants)
    {
        this.position = position;
        this.biome = biome;
        this.occupants = occupants;
        this.gridNodes = gridNodes;
        this.underWater = isUnderWater;
        this.chunk = chunk;
        this.isOutsideBorder = isOutsideBorder;
    }

    public void AddOccupant(GridTileOccupant occupant)
    {
        occupants.Add(occupant);
    }
    
    public void RemoveOccupant(GridTileOccupant occupant)
    {
        occupants.Remove(occupant);
    }

    public static GridTile FindClosestGridTile(Vector3 point)
    {
        
        GridTile[,] world = TerrainController.thisTerrainController.world;
        int tileSize = TerrainController.thisTerrainController.tileSize;

        int x = Mathf.FloorToInt((point.x + tileSize / 2) / tileSize);
        int z = Mathf.FloorToInt((point.z + tileSize / 2) / tileSize);
        if (world == null)
            return null;

        if (x >= world.GetLength(0) || x < 0)
            return null;

        if (z >= world.GetLength(1) || z < 0)
            return null;

        return world[x,z];
    }

    public static GridTile FindClosestGridTile(ThreadVector3 point)
    {

        GridTile[,] world = TerrainController.thisTerrainController.world;
        int tileSize = TerrainController.thisTerrainController.tileSize;

        int x = Mathf.FloorToInt((point.x + tileSize / 2) / tileSize);
        int z = Mathf.FloorToInt((point.z + tileSize / 2) / tileSize);

        if (x >= world.GetLength(0) || x < 0)
            return null;

        if (z >= world.GetLength(1) || z < 0)
            return null;

        return world[x, z];
    }

    // Find all GridTiles in a radius around point
    public static GridTile[] FindGridTilesAround(Vector3 point, float circleRadius)
    {
        // Create the container that we'll be returning when this function is called
        List<GridTile> gridTiles = new List<GridTile>();

        // Find the GridTile object closest to the center point
        GridTile middleTile = FindClosestGridTile(point);

        // If there is no middle Tile, return our empty array
        if (middleTile == null)
            return gridTiles.ToArray();

        // Grab the terrain controller singleton
        TerrainController terrain = TerrainController.thisTerrainController;

        float startTile = -circleRadius;
        float endTile = circleRadius;

        // Finds all the tiles within a SQUARE BOX of the center location
        for (float i = startTile-1; i < endTile; i += terrain.tileSize)
        {
            for (float j = startTile-1; j < endTile; j += terrain.tileSize)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i, 0, middleTile.position.z + j));
                if (tile == null)
                    continue;

                // Only adds a grid tile to the list of tiles when it is within a CIRCULAR RADIUS
                if (Vector3.Distance(new Vector3(tile.position.x, 0, tile.position.z), new Vector3(point.x, 0, point.z)) < circleRadius)
                {
                    gridTiles.Add(FindClosestGridTile(tile.position));
                }
            }
        }

        return gridTiles.ToArray();
    }

    // Find all GridTiles in a radius around point
    public static GridTile[] FindGridTilesAround(ThreadVector3 point, float circleRadius)
    {
        List<GridTile> gridTiles = new List<GridTile>();
        GridTile middleTile = FindClosestGridTile(point - new ThreadVector3(TerrainController.thisTerrainController.tileSize, 0, TerrainController.thisTerrainController.tileSize));
        TerrainController terrain = TerrainController.thisTerrainController;

        float startTile = -circleRadius;
        float endTile = circleRadius;

        if (middleTile == null)
            return gridTiles.ToArray();

        for (float i = startTile - 1; i < endTile; i += terrain.tileSize)
        {
            for (float j = startTile - 1; j < endTile; j += terrain.tileSize)
            {
                GridTile tile = FindClosestGridTile(new Vector3(middleTile.position.x + i, 0, middleTile.position.z + j));
                if (tile == null)
                    continue;

                if (ThreadVector3.Distance(new ThreadVector3(tile.position.x, 0, tile.position.z), new ThreadVector3(point.x, 0, point.z)) < circleRadius)
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
