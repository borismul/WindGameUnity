using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rand = System.Random;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class TerrainController : MonoBehaviour {

    [Header("Terrain Details")]
    public int length;
    public int width;
    public int maxHeight;
    public Biome[] biomes;
    public int objectPerTile;
    public int seed;
    public bool isFlatShaded;

    [Header("Island options")]
    public bool isIsland;
    public float radius;
    public float baseHeight;

    [Header("Mountain Surrounded options")]
    public bool isSourroundedByMountains;
    public float flatRadius;

    [Header("Coast Line Options")]
    public bool isCoastLine;
    public float coastSteepness;
    public float coastBaseHeight;

    [HideInInspector]
    public float islandSteepness;


    [Header("Perline Noise Attributes")]
    public int terrainOctaves;
    public float terrainPersistance;
    public float terrainFrequency;
    public int biomeOctaves;
    public float biomePersistance;
    public float biomeFrequency;

    [Header("Chunk Details")]
    public GameObject chunkPrefab;
    public int chunkSize;
    public int tileSize;
    public int tileSlope;
    public float chunkFadeDistance;
    public LayerMask chunkMask;

    [Header("City Details")]
    public GameObject city;

    [Header("Water Details")]
    public GameObject waterChunkPrefab;
    public GameObject waterUnderLayerPrefab;
    public int waterChunkSize;
    public int waterTileSize;
    public int waterOctaves;
    public float waterPersistance;
    public float waterFrequency;
    public float waterLevel;
    public float maxWaveHeight;

    [Header("Camera")]
    public GameObject mainCamera;

    // Check with this if level is done loading
    [HideInInspector]
    public bool levelLoaded = false;

    // WorldController
    public WorldController worldController;

    // The world, containing all gridtiles in matrix
    public GridTile[,] world;
    public GridNode[,] worldNodes;

    // List of chunks and waterChunks to be able to destroy them
    [HideInInspector]
    public List<Chunk> chunks = new List<Chunk>();
    [HideInInspector]
    public List<WaterChunk> waterChunks = new List<WaterChunk>();
    [HideInInspector]
    public List<WindChunk> windChunks = new List<WindChunk>();

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

    public Vector3 middlePoint;

    int treeCounter = -1;

    int tempCount;
    List<Vector3> vertNew = new List<Vector3>();

    void Awake()
    {
        Application.runInBackground = true;
        int poolThreads = Mathf.Min(3, Mathf.Max(SystemInfo.processorCount - 1, 1));
        MyThreadPool.StartThreadPool(poolThreads);

        // Set this to the terraincontroller
        thisTerrainController = this;

        worldController.Reset();
    }

    void Start()
    {
        // In the main menu load the scene of mission 1
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            BuildButton();
        }
    }

    private void Update()
    {
        if (!levelLoaded)
            return;

        ChunkActivate();


    }

    void ChunkActivate()
    {

        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].Disable();
        }

        foreach (Chunk chunk in chunks)
        {

            if (chunk.ren.IsVisibleFrom(Camera.main))
            {
                if (!chunk.isActive)
                    chunk.Enable();

            }
            else
            {
                if (chunk.isActive)
                    chunk.Disable();
            }
        }

        foreach (WaterChunk chunk in waterChunks)
        {

            if (chunk.ren.IsVisibleFrom(Camera.main))
            {
                chunk.gameObject.SetActive(true);

            }
            else
            {
                chunk.gameObject.SetActive(false);
            }
        }

        foreach (WindChunk chunk in windChunks)
        {

            if (chunk.ren.IsVisibleFrom(Camera.main))
            {
                //chunk.gameObject.SetActive(true);
                //chunk.isEnabled = true;

            }
            else
            {
                //chunk.gameObject.SetActive(false);
                //chunk.isEnabled = false;
            }
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
        worldNodes = new GridNode[length / tileSize + 1, width / tileSize + 1];

        // Set the seed to a random value if set seed is 0, else keep it
        if (seed == 0)
            seed = Random.Range(0, int.MaxValue);

        // Creating new rand instance
        rand = new Rand();

        middlePoint = new Vector3(length / 2, 0, width / 2);
        islandSteepness = baseHeight / (radius * radius);

        // Create biome object meshes
        biomeMeshes = BuildMeshes();

        GameObject light = new GameObject();
        light.transform.SetParent(transform);
        light.AddComponent<Light>().type = LightType.Directional;
        light.GetComponent<Light>().shadows = LightShadows.Soft;
        light.transform.rotation = Quaternion.Euler(45, 0, 0);
        light.name = "Sun";
        light.GetComponent<Light>().shadowNormalBias = 0.94f;
        light.GetComponent<Light>().shadowBias = 0.433f;
        light.GetComponent<Light>().shadowNearPlane = 0.86f;

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

                Mesh[] subMeshes = GetSubmeshes(obj);
                // Add a mesh to the current biome adding also its details
                biomeMesh.AddMesh(GetSubmeshes(obj), occurance, objectPerTile, minScale, maxScale, minRot, maxRot);
                
                // Add to the worldObjects list a new list of GameObjects that holds on the first place the prefab mesh. (Is used to instantiate new ones from)
                worldObjects[i].Add(new List<GameObject>());
                biomes[i].biomeObjects[j].emptyPrefab.GetComponent<TerrainObject>().numVerticesPerObject = new List<int>();

                for (int k = 0; k < subMeshes.Length; k++)
                {
                    biomes[i].biomeObjects[j].emptyPrefab.GetComponent<TerrainObject>().numVerticesPerObject.Add(subMeshes[k].vertexCount);
                }
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
        float timeNow = Time.realtimeSinceStartup;
        // Loop thourgh all chunks
        for (int i = 0; i < length / chunkSize; i++)
        {
            for (int j = 0; j < width / chunkSize; j++)
            {
                // Instantiate one at the right position
                GameObject chunk = (GameObject)Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity);
                chunk.transform.parent = this.transform;
                timeNow = Time.realtimeSinceStartup;
            }
            yield return null;
        }

        yield return null;

        //if (isIsland || isCoastLine)
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
        for (int i = 0; i < chunks.Count; i++)
        {
            for (int k = 0; k < chunks[i].tiles.Count; k++)
            {
                GridTile tile = chunks[i].tiles[k];
                // If the grid tile is under the water level, don't do anything.
                if (tile == null)
                    continue;
                if (tile.position.y < waterLevel)
                    continue;

                // Get all Meshes with parameters of the biome
                BiomeMesh curBiomeMesh = biomeMeshes[Mathf.FloorToInt(tile.biome)];
                List<float> occurances = curBiomeMesh.occurance;
                int objectsPerTile = curBiomeMesh.objectsPerTile*tileSize/20;

                if (objectsPerTile < 1)
                    objectPerTile = 1;

                List<float> minScale = curBiomeMesh.minScale;
                List<float> maxScale = curBiomeMesh.maxScale;
                List<float> minRot = curBiomeMesh.minRot;
                List<float> maxRot = curBiomeMesh.maxRot;

                for (int l = 0; l < objectsPerTile; l++)
                {
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
                        if (choice < upper*tileSize/20 && choice > lower*tileSize/20)
                        {
                            // Determine a random scale and rotation, based on the min and max set by the user
                            float scale = minScale[j] + (float)rand.NextDouble() * (maxScale[j] - minScale[j]);
                            float rot = minRot[j] + (float)rand.NextDouble() * (maxRot[j] - minRot[j]);

                            Vector3 pos = tile.position + Vector3.right * tileSize * ((float)rand.NextDouble() -0.5f) + Vector3.forward * tileSize * ((float)rand.NextDouble() - 0.5f);
                            
                            // Generate the object
                            GenerateObject(pos, Quaternion.Euler(0, rot, 0), Vector3.one * scale, Mathf.FloorToInt(tile.biome), j, tile, chunks[i]);

                            // Break the loop for this grid tile
                            break;
                        }

                        // Set the current upper to the lower limit
                        lower = upper;
                    }
                }


                // index is bigger than the set value wait for one frame update
                if (Time.realtimeSinceStartup - timeSinceUpdate > 1f/10)
                {
                    yield return null;
                    timeSinceUpdate = Time.realtimeSinceStartup;
                }
            }

            for (int biome = 0; biome < biomes.Length; biome++)
            {
                for (int objs = 0; objs < worldObjects[biome].Count; objs++)
                {

                    List<GameObject> curGameObjectList = worldObjects[biome][objs];

                    if (curGameObjectList.Count < 2 && !curGameObjectList[curGameObjectList.Count - 1].GetComponent<TerrainObject>().hasReloaded)
                        continue;

                    curGameObjectList[curGameObjectList.Count - 1].GetComponent<TerrainObject>().verticesNow = 65001;
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
        StartCoroutine("InitiateCity");
    }

    // Initiate the city
    IEnumerator InitiateCity()
    {
        yield return null;
        // instantiate the city

        if (SceneManager.GetActiveScene().name == "1_Persia")
        {
            curCity = Instantiate(city);
            curCity.transform.SetParent(transform);
        }


        if (SceneManager.GetActiveScene().name == "1_Persia")
            WorldController.SetBorders(new Vector3(width / 2, 0, length / 2), 100, 100, 100, 100, true);
        else if (SceneManager.GetActiveScene().name == "4_UnitedStates")
            WorldController.SetBorders(new Vector3(width / 2, 0, length / 2), 30, 30, 40, 40, true);
        else if (SceneManager.GetActiveScene().name == "6_NorthSea")
            WorldController.SetBorders(new Vector3(width / 2 , 0, length / 2), 30, 20, 50, 30, false);

        yield return StartCoroutine(GetWindTiles());

        // When that is done the level loading is complete
        levelLoaded = true;

    }

    IEnumerator GetWindTiles()
    {
        for(int i = 0; i < chunks.Count; i++)
        {
            for(int j = 0; j < chunks[i].tiles.Count; j++)
            {
                chunks[i].tiles[j].SaveWindTiles();
            }

            if (i % 10 == 0)
                yield return null;
        }
    }

    // Method that generates an object on the terrain based on the inputs
    void GenerateObject(Vector3 position, Quaternion rotation, Vector3 scale, int biome, int objIndex, GridTile gridTile, Chunk chunk)
    { 
        // create a TerrainObject
        TerrainObject curterrainObject;

        // Get the gameobjectList of this terrain object
        List<GameObject> curGameObjectList = worldObjects[biome][objIndex];

        // Determine the count
        int count = curGameObjectList.Count;

        // If the gameobject has more vertices than the maximum allowed reload the mesh and set it to full
        if (curGameObjectList[count - 1].GetComponent<TerrainObject>().verticesNow > curGameObjectList[count - 1].GetComponent<TerrainObject>().vertexMax)
        {
            curGameObjectList[count - 1].GetComponent<TerrainObject>().Reload();
        }

        // if the count is lower than 2 or the current gameobject is full add a new gameobject to the list
        if (curGameObjectList.Count < 2 || curGameObjectList[count - 1].GetComponent<TerrainObject>().isFull)
        {
            // Add another instance of the prefab (on index 0) to the list
            GameObject obj = Instantiate(curGameObjectList[0]);
            curGameObjectList.Add(obj);
            chunk.AddTerrainObject(obj.GetComponent<TerrainObject>());
            tempCount = 0;
            // recalculate the count
            count = curGameObjectList.Count;

            // Set the parent of the gameobject to this gameobject
            curGameObjectList[count - 1].transform.parent = transform;

            // Get the terrainobject component
            curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();

            if (curterrainObject.GetComponent<AnimationParameters>() != null)
            {
                treeCounter++;
                TreeAnimationController.instance.treeObjects.Add(curterrainObject);
                TreeAnimationController.instance.lowestVertPerObject.Add(new List<float>());
            }

            // Add the biome and object index to this
            curterrainObject.biome = biome;
            curterrainObject.objectNR = objIndex;
        }
        tempCount++;
        // Get the terrainobject component of the last gameobject in the list
        curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();

        if (curterrainObject.GetComponent<AnimationParameters>() != null)
        {
            List<List<float>> lowestVertPerObject = TreeAnimationController.instance.lowestVertPerObject;
            TreeAnimationController.instance.lowestVertPerObject[treeCounter].Add(position.y);
        }

        // Get the submeshes of the object that needs to be placed
        Mesh[] subMeshes = biomeMeshes[biome].mesh[objIndex];

        // Loop trough the submeshes
        for (int i = 0; i < subMeshes.Length; i++)
        {
            // Add the submesh, moved to the right spot, scale and rotation, to the newcomponents list
            MoveMesh(curterrainObject, subMeshes[i], i, position, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale);

            // Determine the vertex count of the total mesh
            curterrainObject.verticesNow += subMeshes[i].vertexCount;
        }


        // Set the occupant on the gridtile to this generated object
        GridTileOccupant gridOccupant = new GridTileOccupant(curterrainObject.gameObject, position, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale, GridTileOccupant.OccupantType.TerrainGenerated);
        gridTile.AddOccupant(gridOccupant);

    }

    // Method that moves, rotates and scales a mesh
    void MoveMesh(TerrainObject terrainObject, Mesh mesh, int subMesh, Vector3 trans, Quaternion rotate, Vector3 scale)
    {
        // make a new list of vector3 which will hold the vertices
        vertNew.Clear();

        // Keep track of the index in the foreach loop
        int index = 0;

        // loop through all vertices in the inputted mesh
        foreach (Vector3 vert in mesh.vertices)
        {
            // scale, rotate and move the vertex according to inputs
            vertNew.Add(new Vector3(vert.x * scale.x, vert.y* scale.y, vert.z* scale.z));
            vertNew[index] = rotate * vertNew[index];
            vertNew[index] = vertNew[index] + trans;

            index++;
        }

        terrainObject.AddTriangles(mesh.triangles, subMesh);
        terrainObject.AddVertices(vertNew);

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
            ;

            // add it to the list of submeshes
            allMeshes[i] = newMesh;
        }

        // return the array of submeshes
        return allMeshes;
    }

    // Method that removes an occupant from a grid tile
    public void RemoveTerrainTileOccupant(GridTile tile)
    {
        for (int i = 0; i < tile.occupants.Count; i++)
        {
            // if type is Terrain Generated
            if (tile.occupants[i].type == GridTileOccupant.OccupantType.TerrainGenerated)
            {
                // Get the terrainObject script from the occupant
                TerrainObject terrainObj = tile.occupants[i].obj.GetComponent<TerrainObject>();

                // Get the submeshes that should be in this particular terrain object from the biomemeshes
                Mesh[] objMeshes = thisTerrainController.biomeMeshes[terrainObj.biome].mesh[terrainObj.objectNR];

                // create an object of with info of the object that should be removed

                // for each of these meshes
                for (int k = 0; k < objMeshes.Length; k++)
                {
                    Vector3 startVert = objMeshes[k].vertices[0];
                    startVert = new Vector3(startVert.x * tile.occupants[i].scale.x, startVert.y * tile.occupants[i].scale.y, startVert.z * tile.occupants[i].scale.z);
                    // scale, rotate and move the vertex according to inputs
                    startVert = tile.occupants[i].rotation * startVert;
                    startVert = startVert + (tile.occupants[i].position - terrainObj.transform.position);

                    int numVerts = objMeshes[k].vertexCount;
                    // Add a copy of the mesh that should be removed. Move, rotate and scale it as it is in the game.
                    terrainObj.RemoveMesh(new MoveMeshObj(startVert, numVerts));
                }
            }
        }

        for (int i = 0; i < tile.occupants.Count; i++)
        {
            // if type is Terrain Generated
            if (tile.occupants[i].type == GridTileOccupant.OccupantType.TerrainGenerated)
            {
                // set the tile occupant to null
                tile.occupants.RemoveAt(i);
            }
        }
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
        ResourceRequest request = Resources.LoadAsync(path);

        yield return request;

        TextAsset loaded = (TextAsset)request.asset;
        // create a memory stream of the bytes in the loaded textasset
        MemoryStream stream = new MemoryStream(loaded.bytes);
        float timeS = Time.realtimeSinceStartup;
        // Deserialize to a world terrainsaver objects
        TerrainSaver world = (TerrainSaver)binaryFormatter.Deserialize(stream);
        Debug.Log(Time.realtimeSinceStartup - timeS);
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
            curChunk.transform.SetParent(transform);
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
    public void DestroyAll()
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

    private void OnDestroy()
    {
        MyThreadPool.DestroyThreadPool();
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