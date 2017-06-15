using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
    The goal of this class is to keep track of all objects in the scene. 
    It should contain functions for adding and removing new objects as well as
    maintaining a list of all relevant objects.

    ToDo: Should this be deleted upon changing back to main menu? Probably.
**/

public class WorldController : MonoBehaviour
{
    private static WorldController instance;

    [Header("Prefabs")]
    public GameObject weatherManagerPrefab;
    public GameObject[] terrainManagerPrefab;
    public GameObject turbineManagerPrefab;             // Not used yet
    public GameObject buildingsManagerPrefab;           // Not used yet
    public GameObject worldInteractionManagerPrefab;    // Not used yet

    [Header("Collision Info")]
    public LayerMask notTerrain;
    public GameObject debugThing;

    List<Chunk> tempChunks = new List<Chunk>();

    public static WorldController Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
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
        // Make sure the correct terrainManager is loaded for the level.
        // And make all instantiated GameObjects children of the worldcontroller.
        GameObject obj = Instantiate(terrainManagerPrefab[SceneManager.GetActiveScene().buildIndex - 1]);
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

    // Builder function, some class wants the world to add an object
    public void AddTurbine(GameObject t, Vector3 pos, Quaternion rotation, float scale, GridTileOccupant.OccupantType type, Transform parent)
    {
        float diameter = t.GetComponent<SizeController>().diameter;

        pos.y = BuildingHeight(pos, diameter * scale);
        t.transform.position = pos;
        t.transform.rotation = rotation;
        t.transform.SetParent(parent);
        AddToGridTiles(t, pos, diameter * scale, type);
        EqualTerrain(pos, (diameter * scale), false);
        TurbineManager turbManager = TurbineManager.GetInstance();
        turbManager.AddTurbine(t);

        if (TileInfomationMenu.instance != null && TileInfomationMenu.instance.toggle.isOn)
        {
            WindVisualizer.instance.UpdateChunks();
        }
    }

    public void ResetTempChunks()
    {
        foreach (Chunk chunk in tempChunks)
            chunk.GenerateTerrainMesh(true, true);

        tempChunks.Clear();
    }

    public Vector3 TempEqualTerrain(Vector3 pos, float diameter, float scale)
    {
        pos.y = BuildingHeight(pos, diameter * scale);
        EqualTerrain(pos, (diameter * scale), true);

        return pos;
    }

    // Set the terrain height around a position to the tile closest to the entered position
    void EqualTerrain(Vector3 pos, float circleRadius, bool isTemp)
    {
        GridTile middleTile = GridTile.FindClosestGridTile(pos);
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, circleRadius);

        List<Chunk> updateChunks = new List<Chunk>();
        foreach(GridTile tile in gridtiles)
        {
            for (int i = 0; i < tile.gridNodes.Count; i++)
            {
                if (tile.gridNodes[i].heighIsSet)
                    continue;

                Vector3 newPos = Vector3.zero;
                Vector3 vertex = tile.gridNodes[i].position;
                Chunk[] chunks = Chunk.FindChunksWithVertex(vertex);
                foreach (Chunk chunk in chunks)
                {
                    int[] index = Chunk.FindClosestVertices(vertex, chunk);
                    
                    float diffx = chunk.gameObject.transform.position.x + chunk.map[index[0], index[1]].x - middleTile.position.x;
                    float diffz = chunk.gameObject.transform.position.z + chunk.map[index[0], index[1]].z- middleTile.position.z;

                    newPos = new Vector3(chunk.map[index[0], index[1]].x, pos.y, chunk.map[index[0], index[1]].z);

                    if(!isTemp)
                        chunk.map[index[0], index[1]] = newPos;
                    else
                        chunk.tempMap[index[0], index[1]] = newPos;

                    //chunk.AddVertsAndUVAndNorm(chunk.map.GetLength(0), true);

                    if (!updateChunks.Contains(chunk))
                        updateChunks.Add(chunk);
                }

                if (!isTemp)
                {
                    tile.gridNodes[i].position = new Vector3(tile.gridNodes[i].position.x, pos.y, tile.gridNodes[i].position.z);
                    tile.gridNodes[i].heighIsSet = true;
                }

            }
            if (!isTemp)
                tile.position = tile.gridNodes[0].position;

        }
        foreach (Chunk chunk in updateChunks)
        {
            chunk.GenerateTerrainMesh(true, isTemp);
            tempChunks.Add(chunk);
        }
    }

    public void RemoveTurbine(TurbineController turbineController)
    {
        RemoveFromGridTiles(turbineController.gameObject.transform.position, turbineController.GetComponent<SizeController>().diameter + TerrainController.thisTerrainController.tileSize * 3);
        Destroy(turbineController.gameObject);
    }

    public void AddOther(GameObject something, Vector3 pos, Quaternion rotation, float scale, GridTileOccupant.OccupantType type, Transform parent)
    {
        float diameter = something.GetComponent<SizeController>().diameter;
        pos.y = BuildingHeight(pos, diameter * scale);
        GameObject t = (GameObject)Instantiate(something,pos,rotation,transform);
        t.transform.localScale = Vector3.one * scale;

        AddToGridTiles(something, pos, diameter * scale, type);
        EqualTerrain(pos, diameter * scale, false);

        if (TileInfomationMenu.instance != null && TileInfomationMenu.instance.toggle.isOn)
        {
            WindVisualizer.instance.UpdateChunks();
        }
    }

    public bool BuildingNearby(Vector3 pos, float diameter)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, diameter);

        foreach (GridTile tile in gridtiles)
        {
            for (int i = 0; i < tile.gridNodes.Count; i++)
            {
                if (tile.gridNodes[i].heighIsSet)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public float BuildingHeight(Vector3 pos, float diameter)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, diameter);
        int count = 0;
        float curHeight = -1;
        foreach (GridTile tile in gridtiles)
        {
            for (int i = 0; i < tile.gridNodes.Count; i++)
            {
                if (tile.gridNodes[i].heighIsSet)
                {
                    count++;
                    if (count > 1 && Mathf.Abs(tile.gridNodes[i].position.y - curHeight) > 0.5f )
                        return -1;

                    curHeight = tile.gridNodes[i].position.y;
                }
            }
        }

        if (count == 0)
            return pos.y;

        else
            return curHeight;
    }

    // Function that determines if a tile has an object on it and return true if there is no objects on all the tiles in a circle with size as diameter.
    public bool CanBuild(Vector3 pos, float size, GameObject buildObj, float scale, Quaternion rotation, bool neglectTerrainObjects)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(pos, size);
        GridTile thisTile = GridTile.FindClosestGridTile(pos);
        Collider[] colliders = Physics.OverlapBox(buildObj.GetComponentInChildren<BoxCollider>().center * scale + pos, buildObj.GetComponentInChildren<BoxCollider>().size / 2 * scale, rotation, notTerrain);
        //print(colliders[0].transform.name);
        //Vector3 Start1 = buildObj.GetComponentInChildren<BoxCollider>().center * scale - buildObj.GetComponentInChildren<BoxCollider>().size / 2 * scale + pos;
        //Vector3 End1 = pos + buildObj.GetComponentInChildren<BoxCollider>().center * scale;

        //Vector3 Start2 = buildObj.GetComponentInChildren<BoxCollider>().center * scale + buildObj.GetComponentInChildren<BoxCollider>().size / 2 * scale + pos;
        //Vector3 End2 = pos + buildObj.GetComponentInChildren<BoxCollider>().center * scale;

        //Debug.DrawLine(Start1, End1, Color.red, Mathf.Infinity);
        //Debug.DrawLine(Start2, End2, Color.green, Mathf.Infinity);

        if (colliders.Length > 1)
            return false;
        else if (colliders.Length == 1 && colliders[0].gameObject.GetInstanceID() != buildObj.GetInstanceID())
            return false;

        if (pos.y <= TerrainController.thisTerrainController.waterLevel)
            return false;

        foreach (GridTile tile in gridtiles)
        {
            if (tile.isOutsideBorder)
                return false;
            //else if (neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City))
            //    return false;
            //else if (!neglectTerrainObjects && tile.occupant != null && (tile.type == GridTileOccupant.OccupantType.Turbine || tile.type == GridTileOccupant.OccupantType.City || tile.type == GridTileOccupant.OccupantType.TerrainGenerated))
            //    return false;
        }

        if (BuildingHeight(pos, size * scale) == -1)
            return false;

        return true;
    }

    // Function that adds on object to all gridtiles in a certian circle radius around a tile with position point.
    void AddToGridTiles(GameObject something, Vector3 point, float circleRadius, GridTileOccupant.OccupantType type)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(point, circleRadius + 2*TerrainController.thisTerrainController.tileSize);
        foreach (GridTile tile in gridtiles)
        {
            TerrainController.thisTerrainController.RemoveTerrainTileOccupant(tile);
            //tile.occupants.Add(new GridTileOccupant(something));

        }
        //tile.type = type;
    }

    void RemoveFromGridTiles(Vector3 point, float circleRadius)
    {
        GridTile[] gridtiles = GridTile.FindGridTilesAround(point, circleRadius);

        foreach (GridTile tile in gridtiles)
        {
            tile.occupants.Clear();
            //tile.type = GridTileOccupant.OccupantType.Empty;
        }
    }
    
    public static void SetBorders(Vector3 mapMiddle, int width, int length, int camWidth, int camLength, bool makeBorderLine)
    {

        if (makeBorderLine)
        {
            foreach (Chunk chunk in TerrainController.thisTerrainController.chunks)
            {
                for (int i = 0; i < chunk.vert.Count; i++)
                {
                    if (Mathf.Abs(chunk.vert[i].x + chunk.transform.position.x - mapMiddle.x) > width * TerrainController.thisTerrainController.tileSize ||
                        Mathf.Abs(chunk.vert[i].z + chunk.transform.position.z - mapMiddle.z) > length * TerrainController.thisTerrainController.tileSize)
                        chunk.uv[i] = new Vector2(chunk.uv[i].x, chunk.uv[i].y + 0.25f);
                    else
                        chunk.uv[i] = new Vector2(chunk.uv[i].x, chunk.uv[i].y);
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

        Camera.main.GetComponent<CameraController>().maxX = mapMiddle.x + (camWidth * TerrainController.thisTerrainController.tileSize);
        Camera.main.GetComponent<CameraController>().minX = mapMiddle.x - (camWidth * TerrainController.thisTerrainController.tileSize);

        Camera.main.GetComponent<CameraController>().maxZ = mapMiddle.z + (camLength * TerrainController.thisTerrainController.tileSize);
        Camera.main.GetComponent<CameraController>().minZ = mapMiddle.z - (camLength * TerrainController.thisTerrainController.tileSize);
    }
}
