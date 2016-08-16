using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rand = System.Random;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class TerrainController : MonoBehaviour {

    [Header("Terrain Details")]
    [HideInInspector]
    public int length;
    [HideInInspector]
    public int width;
    [HideInInspector]
    public int maxHeight;
    public Biome[] biomes;
    [HideInInspector]
    public int seed;

    [Header("Perline Noise Attributes")]
    [HideInInspector]
    public int terrainOctaves;
    [HideInInspector]
    public float terrainPersistance;
    [HideInInspector]
    public float terrainFrequency;
    [HideInInspector]
    public int biomeOctaves;
    [HideInInspector]
    public float biomePersistance;
    [HideInInspector]
    public float biomeFrequency;

    [Header("Chunk Details")]
    public GameObject chunkPrefab;
    [HideInInspector]
    public int chunkSize;
    [HideInInspector]
    public int tileSize;
    [HideInInspector]
    public int tileSlope;

    [Header("City Details")]
    public GameObject city;

    [Header("Water Details")]
    public GameObject waterChunkPrefab;
    [HideInInspector]
    public int waterChunkSize;
    [HideInInspector]
    public int waterTileSize;
    [HideInInspector]
    public int waterOctaves;
    [HideInInspector]
    public float waterPersistance;
    [HideInInspector]
    public float waterFrequency;
    [HideInInspector]
    public float waterLevel;
    [HideInInspector]
    public float maxWaveHeight;

    [Header("Camera")]
    public GameObject mainCamera;

    // Check with this if level is done loading
    [HideInInspector]
    public bool levelLoaded = false;

    // The world, containing all gridtiles in matrix
    public GridTile[,] world;

    // List of chunks and waterChunks to be able to destroy them
    [HideInInspector]
    public List<Chunk> chunks = new List<Chunk>();
    [HideInInspector]
    public List<WaterChunk> waterChunks = new List<WaterChunk>();

    // The city that is being spawned (To be able to destroy it)
    GameObject curCity;

    // The terrain generated objects in the world
    public List<List<List<GameObject>>> worldObjects = new List<List<List<GameObject>>>();

    // List that contains all meshes for objects on terrain
    public List<BiomeMesh> biomeMeshes;

    // this current terrainController so it can be obtained easy
    public static TerrainController thisTerrainController;

    // Another random number generator
    Rand rand;

    void Awake()
    {
        // Set this to the terraincontroller
        thisTerrainController = this;
    }

    void Start()
    {
        // In the main menu load the scene of mission 1
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            StartCoroutine(Load("Mission1", false));
        }
    }

    // This method is used if the build button in the map editor is run
    public void BuildButton()
    {
        DestroyAll();
        Initialize();
        StartCoroutine(BuildTerrain());
    }

    // Set all static variables to variables set in the editor
    void Initialize()
    {
        // Create a new world
        world = new GridTile[length / tileSize, width / tileSize];
        // Set the seed to a random value if set seed is 0, else keep it
        if (seed == 0)
            seed = Random.Range(0, int.MaxValue);

        // Creating new rand instance
        rand = new Rand();

        // Create biome object meshes
        biomeMeshes = BuildMeshes();
    }

    // Method creates meshes of all objects per biome
    List<BiomeMesh> BuildMeshes()
    {
        // Create a new list of all meshes on the terrain
        List<BiomeMesh> biomeMeshes = new List<BiomeMesh>();

        // Loop through all biomes
        for (int i = 0; i < biomes.Length; i++)
        {
            // For each new biome, add a new 2 dimensional list of gameobjects (for each terrainobject their exists a list of gameobject holding meshes of that object)
            worldObjects.Add(new List<List<GameObject>>());

            // Create a new biomemesh which holds the meshes and other details for the current biome
            BiomeMesh biomeMesh = new BiomeMesh();

            // Loop through each of the objects added to the biome
            for (int j = 0; j < biomes[i].biomeObjects.Length; j++)
            {
                // Get all parameters that were set on that specific terrain object
                GameObject obj = biomes[i].biomeObjects[j].prefab;
                float occurance = biomes[i].biomeObjects[j].occurance;
                float minScale = biomes[i].biomeObjects[j].minScale;
                float maxScale = biomes[i].biomeObjects[j].maxScale;
                float minRot = biomes[i].biomeObjects[j].minRotation;
                float maxRot = biomes[i].biomeObjects[j].maxRotation;

                // Add a mesh to the current biome adding also its details
                biomeMesh.AddMesh(GetSubmeshes(obj), occurance, minScale, maxScale, minRot, maxRot);
                
                // Add to the worldObjects list a new list of GameObjects that holds on the first place the prefab mesh. (Is used to instantiate new ones from)
                worldObjects[i].Add(new List<GameObject>());
                worldObjects[i][j].Add(biomes[i].biomeObjects[j].emptyPrefab);
            }

            // Add the biomeMesh to the list of all biomeMeshes
            biomeMeshes.Add(biomeMesh);
        }
        
        return biomeMeshes;
    }

    // Create chunks by instantiating the prefabs at the desired locations
    IEnumerator BuildTerrain()
    {
        // Loop thourgh all chunks
        for (int i = 0; i < length / chunkSize; i++)
        {
            for (int j = 0; j < width / chunkSize; j++)
            {
                // Instantiate one at the right position
                GameObject chunk = (GameObject)Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity);
                chunk.transform.parent = this.transform;
            }

            yield return null;
        }

        // Build the water chunks
        BuildWater();

        // Generate all the biome attributes
        StartCoroutine(GenerateBiomeAttributes());
    }

    // Create the water chunks by instantiating them at the desired positions
    void BuildWater()
    {
        // Loop though all water chunks that need to be instantiated
        for (int i = 0; i < length / waterChunkSize; i++)
        {
            for (int j = 0; j < width / waterChunkSize; j++)
            {
                // Instantiate them at the right positions
                GameObject chunk = (GameObject)Instantiate(waterChunkPrefab, new Vector3(i * waterChunkSize, 0, j * waterChunkSize), Quaternion.identity);
                chunk.transform.parent = this.transform;
            }
        }
    }

    // Method that generates each of the Objects on the terrain
    IEnumerator GenerateBiomeAttributes()
    {
        // Index to keep track of how much time it takes
        float timeSinceUpdate = Time.realtimeSinceStartup;

        // Loop through each grid tile in the world
        for (int i = 0; i < world.GetLength(0); i++)
        {
            for (int k = 0; k < world.GetLength(1); k++)
            {
                // If the grid tile is under the water level, don't do anything.
                if (world[i, k].position.y < waterLevel)
                    continue;

                // Get all Meshes with paramters of the biome
                BiomeMesh curBiomeMesh = biomeMeshes[world[i,k].biome];
                List<float> occurances = curBiomeMesh.occurance;
                List<float> minScale = curBiomeMesh.minScale;
                List<float> maxScale = curBiomeMesh.maxScale;
                List<float> minRot = curBiomeMesh.minRot;
                List<float> maxRot = curBiomeMesh.maxRot;

                // create a random variable between 0 and 1
                float choice = (float)rand.NextDouble();

                // Create a lower and upper limit
                float lower = 0;
                float upper = 0;

                for (int j = 0; j < occurances.Count; j++)
                {
                    // Add the occurance of the current object to the upper limit
                    upper += occurances[j];

                    // If the random variable is between upper and lower limit generate that object
                    if (choice < upper && choice > lower)
                    {

                        // Determine a random scale and rotation, based on the min and max set by the user
                        float scale = minScale[j] + (float)rand.NextDouble() * (maxScale[j] - minScale[j]);
                        float rot = minRot[j] + (float)rand.NextDouble() * (maxRot[j] - minRot[j]);
                        
                        // Generate the object
                        GenerateObject((world[i,k].position), Quaternion.Euler(0, rot, 0), Vector3.one * scale, world[i,k].biome, j, world[i,k]);

                        // Break the loop for this grid tile
                        break;
                    }

                    // Set the current upper to the lower limit
                    lower = upper;
                }


                // index is bigger than the set value wait for one frame update
                if (Time.realtimeSinceStartup - timeSinceUpdate > 1f/10)
                {
                    yield return null;
                    timeSinceUpdate = Time.realtimeSinceStartup;
                }

            }
        }

        // When done, be sure the check if all meshes have been updated
        BuildMeshesAll();
    }

    // Method that checks if all biome object meshes have been updated
    void BuildMeshesAll()
    {
        // Loop trhough all object lists that are in the world
        foreach (List<List<GameObject>> biomeList in worldObjects)
        {
            foreach (List<GameObject> objList in biomeList)
            {
                int index = 0;
                foreach (GameObject curObject in objList)
                {
                    // if the mesh has not reloaded and it is not the first one in the list (prefab), reload it
                    if (!curObject.GetComponent<TerrainObject>().hasReloaded && index != 0)
                    {
                        curObject.GetComponent<TerrainObject>().Reload();
                    }

                    index++;
                }
            }
        }

        // Initiate the city
        InitiateCity();
    }

    // Initiate the city
    void InitiateCity()
    {
        // instantiate the city
        curCity = Instantiate(city);

        // When that is done the level loading is complete
        levelLoaded = true;
    }

    // Method that generates an object on the terrain based on the inputs
    void GenerateObject(Vector3 position, Quaternion rotation, Vector3 scale, int biome, int objIndex, GridTile gridTile)
    {
        // create a TerrainObject
        TerrainObject curterrainObject;
        
        // Get the gameobjectList of this terrain object
        List<GameObject> curGameObjectList = worldObjects[biome][objIndex];

        // Determine the count
        int count = curGameObjectList.Count;

        // if the count is lower than 2 or the current gameobject is full add a new gameobject to the list
        if (curGameObjectList.Count < 2 || curGameObjectList[count - 1].GetComponent<TerrainObject>().isFull)
        {
            // Add another instance of the prefab (on index 0) to the list
            curGameObjectList.Add(Instantiate(curGameObjectList[0]));

            // recalculate the count
            count = curGameObjectList.Count;

            // Set the parent of the gameobject to this gameobject
            curGameObjectList[count - 1].transform.parent = transform;

            // Get the terrainobject component
            curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();

            // Add the biome and object index to this
            curterrainObject.biome = biome;
            curterrainObject.objectNR = objIndex;
        }

        // Get the terrainobject component of the last gameobject in the list
        curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();

        // Get the submeshes of the object that needs to be placed
        Mesh[] subMeshes = biomeMeshes[biome].mesh[objIndex];

        // Loop trough the submeshes
        for (int i = 0; i < subMeshes.Length; i++)
        {
            // Add the submesh, moved to the right spot, scale and rotation, to the newcomponents list
            curterrainObject.newComponents[i].Add(MoveMesh(subMeshes[i], position, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale));

            // Determine the vertex count of the total mesh
            curterrainObject.verticesNow += subMeshes[i].vertexCount;
        }

        // If the gameobject has more vertices than the maximum allowed reload the mesh and set it to full
        if (curterrainObject.verticesNow > curterrainObject.vertexMax)
        {
            curterrainObject.isFull = true;
            curterrainObject.hasReloaded = true;
            curterrainObject.Reload();
        }

        // Set the occupant on the gridtile to this generated object
        GridTileOccupant gridOccupant = new GridTileOccupant(curterrainObject.gameObject, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale);
        gridTile.occupant = gridOccupant;

        // set the gridtile type to 1 which means there is a terrain object on it
        gridTile.type = 1;

    }

    // Method that moves, rotates and scales a mesh
    Mesh MoveMesh(Mesh mesh, Vector3 trans, Quaternion rotate, Vector3 scale)
    {
        // create a new mesh
        Mesh result = new Mesh();

        // make a new list of vector3 which will hold the vertices
        Vector3[] vertNew = new Vector3[mesh.vertexCount];

        // Keep track of the index in the foreach loop
        int index = 0;

        // loop through all vertices in the inputted mesh
        foreach (Vector3 vert in mesh.vertices)
        {
            // scale, rotate and move the vertex according to inputs
            vertNew[index] = new Vector3(vert.x * scale.x, vert.y* scale.y, vert.z* scale.z);
            vertNew[index] = rotate * vertNew[index];
            vertNew[index] = vertNew[index] + trans;

            index++;
        }
        
        // set the vertices and the submeshcount in the new mesh
        result.vertices = vertNew;
        result.subMeshCount = mesh.subMeshCount;

        // loop thourgh all submeshes in the inputted mesh
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            // set the triangles in the submesh to the new mesh
            result.SetTriangles(mesh.GetTriangles(i), i);
        }
        // set the uv and normals of the old mesh
        result.uv = mesh.uv;
        result.normals = mesh.normals;

        return result;
    }

    // Get submeshes of a prefab
    Mesh[] GetSubmeshes(GameObject obj)
    {
        // Get the mesh of the gameobject
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

        // Generate a new mesh array which will contain all the submeshes
        Mesh[] allMeshes = new Mesh[mesh.subMeshCount];

        // Loop though all submeshes of the object mesh
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            // generate a new mesh
            Mesh newMesh = new Mesh();

            // create a list of vector3 which will contain the vertices of the submesh
            List<Vector3> vert = new List<Vector3>();

            // create a list of int which will contain the triangles of the submesh
            List<int> finTriangles = new List<int>();

            // create lists for all uvs in the sub mesh
            List<Vector2> uvs, uvs2, uvs3, uvs4;
            uvs = new List<Vector2>();
            uvs2 = new List<Vector2>();
            uvs3 = new List<Vector2>();
            uvs4 = new List<Vector2>();

            // Get all vertices in the mesh of the object
            Vector3[] allVert = mesh.vertices;

            // get all triangles that are in the submesh
            int[] tri = mesh.GetTriangles(i);

            // loop trough each of these triangles
            for (int j = 0; j < tri.Length; j++)
            {
                // get the index of the vertex of which the current triangle points to
                int curVertIndex = tri[j];

                // If the new list of vertices does not contain that vertex 
                if (!vert.Contains(allVert[curVertIndex]))
                {
                    // add this vertex
                    vert.Add(allVert[curVertIndex]);

                    // also add the traingle that points to this vertex
                    finTriangles.Add(vert.Count - 1);

                    // Also add the uvs of that vertex
                    if (mesh.uv != null && mesh.uv.Length > curVertIndex)
                        uvs.Add(mesh.uv[curVertIndex]);
                    if (mesh.uv2 != null && mesh.uv2.Length > curVertIndex)
                        uvs2.Add(mesh.uv2[curVertIndex]);
                    if (mesh.uv3 != null && mesh.uv3.Length > curVertIndex)
                        uvs3.Add(mesh.uv3[curVertIndex]);
                    if (mesh.uv4 != null && mesh.uv4.Length > curVertIndex)
                        uvs4.Add(mesh.uv4[curVertIndex]);
                }
                // Else if it already contains the vertex
                else
                {
                    // Add only the triangle and point it to the right vertex
                    Vector3 tryThis = allVert[curVertIndex];
                    finTriangles.Add(vert.IndexOf(tryThis));
                }
            }
            
            // Buildup the new mesh by adding vertices, triangles, uvs to it
            newMesh.vertices = vert.ToArray();
            newMesh.triangles = finTriangles.ToArray();
            if (uvs.Count > 0)
                newMesh.uv = uvs.ToArray();
            if (uvs2.Count > 0)
                newMesh.uv2 = uvs2.ToArray();
            if (uvs3.Count > 0)
                newMesh.uv3 = uvs3.ToArray();
            if (uvs4.Count > 0)
                newMesh.uv4 = uvs4.ToArray();

            // also calculate the normals bounds and optimize it
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            newMesh.Optimize();

            // add it to the list of submeshes
            allMeshes[i] = newMesh;
        }

        // return the array of submeshes
        return allMeshes;
    }

    // Method that sets the occupant of a tile and check in a certain radius where objects should be removed
    public IEnumerator SetOccupant(GridTile tile, GridTileOccupant occupant, float cutOffRadius, bool removeOccupants, bool removeSelfBuild, bool setNotBuild)
    {
        // Remove the current occupant on this tile
        RemoveOccupant(tile, false);

        // set the new tile occupant
        tile.occupant = occupant;

        // set the tile type to 2 which means that it is a self build object
        tile.type = 2;
        
        // get the i and k index of the current grid tile in world
        int thisIndexI = Mathf.RoundToInt((tile.position.x) / tileSize);
        int thisIndexK = Mathf.RoundToInt((tile.position.z) / tileSize);

        // Determine the start index at which object should be removed
        int startI = thisIndexI - Mathf.RoundToInt(cutOffRadius / tileSize);
        int startK = thisIndexK - Mathf.RoundToInt(cutOffRadius / tileSize);

        // Set the maximal values of i and k so the algorithm never tries to go outside the borders of the map
        int maxI = length / tileSize;
        int maxK = width / tileSize;

        // loop through all tiles tiles that are in the neighbourhood
        for (int i = 0; i < cutOffRadius / tileSize * 2; i++)
        {
            for (int j = 0; j < cutOffRadius / tileSize * 2; j++)
            {
                // if it is outside the borders continue
                if (startI + i < 0 || startI + i > maxI || startK + j < 0 || startK + j > maxK)
                    continue;

                // get the current tile that needs to be checked
                GridTile checkGridTile = world[startI + i, startK + j];

                // if the tile has a distance that is lower than the cutoffraduis and the gridtile has an occupant and it is said that the occupants should be removed
                if (Vector3.Distance(checkGridTile.position, tile.position) < cutOffRadius && checkGridTile.occupant != null && removeOccupants)
                {
                    // remove the occupant
                    RemoveOccupant(checkGridTile, removeSelfBuild);
                    yield return null;
                }
                // if the gridtile is within the cutoffradius and it should be set as not build set canbuild to false
                if (Vector3.Distance(checkGridTile.position, tile.position) < cutOffRadius && setNotBuild)
                    checkGridTile.canBuild = false;
            }
        }
    }

    // Method that removes an occupant from a grid tile
    public void RemoveOccupant(GridTile tile, bool removeSelfBuild)
    {
        // if tile has no occupant return
        if (tile.type == 0)
            return;

        // if type is 2 only remove if removeselfbuild is true
        if (tile.type == 2)
        {
            if (removeSelfBuild)
            {
                Destroy(tile.occupant.obj);
                tile.occupant = null;
                tile.type = 0;
            }
            return;
        }

        // if type is 1
        if (tile.type == 1)
        {
            // Get the terrainObject script from the occupant
            TerrainObject terrainObj = tile.occupant.obj.GetComponent<TerrainObject>();

            // Get the submeshes that should be in this particular terrain object from the biomemeshes
            Mesh[] objMeshes = thisTerrainController.biomeMeshes[terrainObj.biome].mesh[terrainObj.objectNR];

            // create a array of meshes that should be removed
            Mesh[] removeMesh = new Mesh[objMeshes.Length];

            // for each of these meshes
            for (int k = 0; k < objMeshes.Length; k++)
            {
                // Add a copy of the mesh that should be removed. Move, rotate and scale it as it is in the game.
                removeMesh[k] = MoveMesh(objMeshes[k], tile.position - terrainObj.transform.position, tile.occupant.rotation, tile.occupant.scale);
            }

            // remove the meshes
            terrainObj.RemoveMesh(removeMesh);

            // recalculate the normals of the mesh
            terrainObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            // set the tile occupant to null
            tile.occupant = null;
            tile.type = 0;
            
            // for each of the mesh copies that have been created destory it so it will not fill op the ram memory
            if (removeMesh != null)
            {
                foreach (Mesh destroyMesh in removeMesh)
                {
                    Destroy(destroyMesh);
                }
            }
        }
    }

    // Method that builds an object on a tile and checks if area around it should be checked for object that need to be removed
    public GameObject BuildObject(GameObject obj, Quaternion rotation, Vector3 scale, GridTile tile, float cutOffRadius, bool removeOccupants, bool removeSelfBuild, bool setNotBuild)
    {
        // Instantiate the object at tile with a certain rotation
        GameObject objInst = (GameObject)Instantiate(obj, tile.position, rotation);

        // Set the scale
        objInst.transform.localScale = scale;

        // Create a grid tile occupant for this gameobject
        GridTileOccupant tileOccupant = new GridTileOccupant(objInst);

        // Remove objects around it if indicated
        StartCoroutine(SetOccupant(tile, tileOccupant, cutOffRadius, removeOccupants, removeSelfBuild, setNotBuild));

        return objInst;
    }

    // Method that saves the terrain
    public void Save(string fileName)
    {
        // Set the target path where the file should be placed
        string path = Application.dataPath + "/Resources/Saved Maps/";

        // if the directory does not exist yet create one
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        // open a filestream for the file that is created
        FileStream fileStream = File.Open(path + fileName, FileMode.OpenOrCreate);

        // Create a new binaryFormatter
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        // make a list of terrainsaveobjects
        List<TerrainSaveObject> terrainSave = new List<TerrainSaveObject>();

        // Create a terrainsaveobject for all chunks and add them to the list
        foreach (Chunk chunk in chunks)
            terrainSave.Add(new TerrainSaveObject(chunk.map, chunk.biomeMap, chunk.transform.position));
        
        // Serialize the data by creating a terrainsaver
        binaryFormatter.Serialize(fileStream, new TerrainSaver(terrainSave, length, width, maxHeight, seed, terrainOctaves, terrainPersistance, terrainFrequency, biomeOctaves, biomePersistance, biomeFrequency, chunkSize, tileSize, tileSlope, waterChunkSize, waterTileSize, waterOctaves, waterPersistance, waterFrequency, waterLevel, maxWaveHeight));
        
        // close the filestream
        fileStream.Close();
    }

    // Method that loads a terrain
    public IEnumerator Load(string fileName, bool instantiateCamera)
    {

        // Create the load path
        string path = "Saved Maps/" + fileName;

        // Create a binaryFormatter
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        // load the map at path as a textasset
        TextAsset loaded = Resources.Load(path) as TextAsset;

        // create a memory stream of the bytes in the loaded textasset
        MemoryStream stream = new MemoryStream(loaded.bytes);

        // Deserialize to a world terrainsaver objects
        TerrainSaver world = (TerrainSaver)binaryFormatter.Deserialize(stream);

        // get all terrain parameters
        length = world.length;
        width = world.width;
        maxHeight = world.maxHeight;
        seed = world.seed;

        terrainOctaves = world.terrainOctaves;
        terrainPersistance = world.terrainPersistance;
        terrainFrequency = world.terrainFrequency;
        biomeOctaves = world.biomeOctaves;
        biomePersistance = world.biomePersistance;
        biomeFrequency = world.biomeFrequency;

        chunkSize = world.chunkSize;
        tileSize = world.tileSize;
        tileSlope = world.tileSlope;

        waterChunkSize = world.waterChunkSize;
        waterTileSize = world.waterTileSize;
        waterOctaves = world.waterOctaves;
        waterPersistance = world.waterPersistance;
        waterFrequency = world.waterFrequency;
        waterLevel = world.waterLevel;
        maxWaveHeight = world.maxWaveHeight;

        // Get the list of all chunks
        List<TerrainSaveObject> chunkList = world.terrainSaveList;

        // Initialize the code so the map can be created
        Initialize();

        // Loop through all chunks and recreate them with their respective parameters
        float timeSinceUpdate = Time.realtimeSinceStartup;
        int index = 0;
        foreach (TerrainSaveObject obj in chunkList)
        {
            GameObject curChunk = (GameObject)Instantiate(chunkPrefab, obj.chunkLoc.GetVec3(), Quaternion.identity);
            Chunk chunkScript = curChunk.GetComponent<Chunk>();
            chunkScript.map = obj.GetVec3Map();
            chunkScript.biomeMap = obj.biomeMap;

            // try to save ram by deleting the chunks from the chunklist
            chunkList[index] = null;
            System.GC.Collect();

            // Show an update if a certain ammount of chunks has been built
            if (Time.realtimeSinceStartup - timeSinceUpdate > 1f / 10f)
            {
                timeSinceUpdate = Time.realtimeSinceStartup;
                yield return null;
            }

            index++;
        }

        // Create the water
        BuildWater();

        // Create all biome objects
        StartCoroutine(GenerateBiomeAttributes());
    }

    // Method that destroys the whole terrain
    void DestroyAll()
    {
        // Destroy chunks
        foreach (Chunk chunk in chunks)
            Destroy(chunk.gameObject);

        // Destory waterChunks
        foreach (WaterChunk chunk in waterChunks)
            Destroy(chunk.gameObject);

        // Clear their lists
        chunks.Clear();
        waterChunks.Clear();
        
        // Destroy all terrain objects
        foreach (List<List<GameObject>> worldList2D in worldObjects)
        {
            foreach (List<GameObject> worldList1D in worldList2D)
            {
                int index = 0;
                foreach (GameObject obj in worldList1D)
                {
                    if(index != 0)
                        Destroy(obj);
                    index++;
                }
                worldList1D.Clear();
            }
            worldList2D.Clear();
        }


        // Destory the city
        Destroy(curCity);
    }


    // Structs that are used to visualize the biome parameters in the editor
    [System.Serializable]
    public struct Biome
    {
        public BiomeObject[] biomeObjects;
    }

    [System.Serializable]
    public struct BiomeObject
    {
        public GameObject prefab;
        public GameObject emptyPrefab;
        [Range(0, 1)]
        public float occurance;
        public float minScale;
        public float maxScale;
        [Range(0,360)]
        public float minRotation;
        [Range(0,360)]
        public float maxRotation;
    }


}
