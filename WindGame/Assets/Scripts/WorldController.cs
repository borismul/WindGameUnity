using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/**
    The goal of this class is to keep track of all objects in the scene. 
    It should contain functions for adding and removing new objects as well as
    maintaining a list of all relevant objects.
**/

public class WorldController : MonoBehaviour
{
    private static WorldController instance;

    [Header("Prefabs")]
    public GameObject weatherManagerPrefab;
    public GameObject terrainManagerPrefab;
    public GameObject turbineManagerPrefab;
    public GameObject buildingsManagerPrefab;
    public GameObject worldInteractionManagerPrefab;

    // Use this for initialization
    void Awake()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
    }

    void Start()
    {
        if(GameResources.currentMission == 1)
        {
            transform.GetComponentInChildren<TerrainController>().seed = 1885953640;
        } else if(GameResources.currentMission == 2)
        {
            transform.GetComponentInChildren<TerrainController>().seed = 1123992140;
        }
    }


    void Update()
    {
     
    }

    // Create the singletone for the WorldManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("WorldManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the WorldManager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(terrainManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(weatherManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(turbineManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(buildingsManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(worldInteractionManagerPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public static WorldController GetInstance()
    {
        return instance;
    }

    // Builder function, some class wants the world to add an object
    public void AddTurbine(GameObject t, Vector3 pos, Quaternion rotation, float scale, GridTileOccupant.OccupantType type, Transform parent)
    {
        t.transform.position = pos;
        t.transform.rotation = rotation;
        t.transform.SetParent(parent);
        float diameter = t.GetComponent<SizeController>().diameter;
        AddToGridTiles(t, pos, (diameter * scale) + 2 * TerrainController.thisTerrainController.tileSize, type);
        EqualTerrain(pos, (diameter * scale));
        TurbineManager turbManager = TurbineManager.GetInstance();
        turbManager.AddTurbine(t); 
    }

    // Set the terrain height around a position to the tile closest to the entered position
    void EqualTerrain(Vector3 pos, float circleRadius)
    {
        GridTile middleTile = GridTile.FindClosestGridTile(pos);
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, circleRadius);

        List<Chunk> updateChunks = new List<Chunk>();
                      
        foreach(GridTile tile in gridtiles)
        {
            for (int i = 0; i < tile.vert.Count; i++)
            {
                Vector3 newPos = Vector3.zero;
                Vector3 vertex = tile.vert[i];
                Chunk[] chunks = Chunk.FindChunksWithVertex(vertex);

                foreach (Chunk chunk in chunks)
                {
                    List<int[]> vertexIndices = Chunk.FindClosestVertices(vertex, chunk);

                    foreach (int[] index in vertexIndices)
                    {
                        newPos = new Vector3(chunk.map[index[0], index[1]].x, middleTile.position.y, chunk.map[index[0], index[1]].z);
                        chunk.map[index[0], index[1]] = newPos;
                    }
                    if (!updateChunks.Contains(chunk))
                        updateChunks.Add(chunk);
                }
                tile.vert[i] = new Vector3(tile.vert[i].x, middleTile.position.y, tile.vert[i].z);

            }
            tile.position = tile.vert[0];
        }
        foreach (Chunk chunk in updateChunks)
            chunk.GenerateTerrainMesh(true);
    }

    public void RemoveTurbine(TurbineController turbineController)
    {
        RemoveFromGridTiles(turbineController.gameObject.transform.position, turbineController.GetComponent<SizeController>().diameter + TerrainController.thisTerrainController.tileSize * 3);
        Destroy(turbineController.gameObject);
    }

    public void AddOther(GameObject something, Vector3 pos, Quaternion rotation, float scale, GridTileOccupant.OccupantType type, Transform parent)
    {
        GameObject t = (GameObject)Instantiate(something, pos, rotation, parent);
        t.transform.localScale = Vector3.one * scale;
        float diameter = t.GetComponent<SizeController>().diameter;
        AddToGridTiles(something, pos, diameter * scale + 2 * TerrainController.thisTerrainController.tileSize, type);
        EqualTerrain(pos, diameter * scale);
    }

    // Function that determines if a tile has an object on it and return true if there is no objects on all the tiles in a circle with size as diameter.
    public bool CanBuild(Vector3 pos, float size, bool neglectTerrainObjects)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, size);
        GridTile thisTile = GridTile.FindClosestGridTile(pos);

        if (thisTile.underWater)
            return false;

        foreach (GridTile tile in gridtiles)
        {
            if (tile.isOutsideBorder)
                return false;
            else if (neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City))
                return false;
            else if (!neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City || tile.type == GridTileOccupant.OccupantType.TerrainGenerated))
                return false;
        }
        return true;
    }

    // Function that adds on object to all gridtiles in a certian circle radius around a tile with position point.
    void AddToGridTiles(GameObject something, Vector3 point, float circleRadius, GridTileOccupant.OccupantType type)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(point, circleRadius);

        foreach (GridTile tile in gridtiles)
        {
            if (tile.type == GridTileOccupant.OccupantType.TerrainGenerated)
            {
                TerrainController.thisTerrainController.RemoveTerrainTileOccupant(tile);
            }

            tile.occupant = new GridTileOccupant(something);
            tile.type = type;
        }
    }

    void RemoveFromGridTiles(Vector3 point, float circleRadius)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(point, circleRadius);

        foreach (GridTile tile in gridtiles)
        {
            tile.occupant = null;
            tile.type = GridTileOccupant.OccupantType.Empty;
        }
    }
    
    public static void SetBorders(Vector3 mapMiddle, int width, int length)
    {
        foreach (Chunk chunk in TerrainController.thisTerrainController.chunks)
        {
            for (int i = 0; i < chunk.vert.Count; i++)
            {
                if (Mathf.Abs(chunk.vert[i].x + chunk.transform.position.x - mapMiddle.x) > width * TerrainController.thisTerrainController.tileSize ||
                    Mathf.Abs(chunk.vert[i].z + chunk.transform.position.z - mapMiddle.z) > length * TerrainController.thisTerrainController.tileSize)
                    chunk.uv[i] = new Vector2(chunk.uv[i].x, 3f / 8f);
                else
                    chunk.uv[i] = new Vector2(chunk.uv[i].x, 1f / 8f);
            }

            chunk.SetMesh(TerrainController.thisTerrainController.isFlatShaded);
        }

        foreach (GridTile tile in TerrainController.thisTerrainController.world)
        {

            if (Mathf.Abs(tile.position.x - mapMiddle.x) > width * TerrainController.thisTerrainController.tileSize ||
                Mathf.Abs(tile.position.z - mapMiddle.z) > length * TerrainController.thisTerrainController.tileSize)
                tile.isOutsideBorder = true;
            else
                tile.isOutsideBorder = false;
        }

    }
}
