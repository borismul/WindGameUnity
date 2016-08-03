using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rand = System.Random;

public class TerrainController : MonoBehaviour {

    [Header("Terrain Details")]
    public int length;
    public int width;
    public int maxHeight;
    public Biome[] biomes;
    public int seed;

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

    [Header("City Details")]
    public GameObject city;

    // Check with this if level is done loading
    public bool levelLoaded;

    // The world, containing all gridtiles in matrix
    public GridTile[,] world;

    // The Objects in the world
    public List<List<List<GameObject>>> worldObjects = new List<List<List<GameObject>>>();

    // List that contains all meshes for object on terrain
    public List<BiomeMesh> biomeMeshes;

    // this current terrainController so it can be obtained easy
    public static TerrainController thisTerrainController;

    public GameObject debugPrefab;
    // Another random number generator
    Rand rand;

    // Use this for initialization
    void Awake()
    {
        Initialize();
        StartCoroutine(BuildTerrain());
    }

    // Set all static variables to variables set in the editor
    void Initialize()
    {
        // Set this to the terraincontroller
        thisTerrainController = this;

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

        int index = -1;
        // Add all object meshes to right biome
        foreach (Biome biome in biomes)
        {
            index++;
            worldObjects.Add(new List<List<GameObject>>());
            BiomeMesh biomeMesh = new BiomeMesh();
            for (int i = 0; i < biome.biomeObjects.Length; i++)
            {
                GameObject obj = biome.biomeObjects[i].prefab;
                float occurance = biome.biomeObjects[i].occurance;
                float minScale = biome.biomeObjects[i].minScale;
                float maxScale = biome.biomeObjects[i].maxScale;
                float minRot = biome.biomeObjects[i].minRotation;
                float maxRot = biome.biomeObjects[i].maxRotation;

                biomeMesh.AddMesh(GetSubmeshes(obj), occurance, minScale, maxScale, minRot, maxRot);
                worldObjects[index].Add(new List<GameObject>());
                worldObjects[index][i].Add(biome.biomeObjects[i].emptyPrefab);
            }
            biomeMeshes.Add(biomeMesh);
        }
        return biomeMeshes;
    }

    // Create chunks by instantiating the prefabs at the desired locations
    IEnumerator BuildTerrain()
    {
        for (int i = 0; i < length / chunkSize; i++)
        {
            for (int j = 0; j < width / chunkSize; j++)
            {
                GameObject chunk = (GameObject)Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity);
                chunk.transform.parent = this.transform;
                yield return null;
            }
        }
        StartCoroutine(GenerateBiomeAttributes());
    }

    IEnumerator GenerateBiomeAttributes()
    {
        int index = 0;
        for (int i = 0; i < world.GetLength(0); i++)
        {
            for (int k = 0; k < world.GetLength(1); k++)
            {
                BiomeMesh curBiomeMesh = biomeMeshes[world[i,k].biome];
                List<float> occurances = curBiomeMesh.occurance;
                List<float> minScale = curBiomeMesh.minScale;
                List<float> maxScale = curBiomeMesh.maxScale;
                List<float> minRot = curBiomeMesh.minRot;
                List<float> maxRot = curBiomeMesh.maxRot;

                float choice = (float)rand.NextDouble();
                float lower = 0;
                float upper = 0;
                for (int j = 0; j < occurances.Count; j++)
                {
                    upper += occurances[j];
                    if (choice < upper && choice > lower)
                    {
                        float scale = minScale[j] + (float)rand.NextDouble() * (maxScale[j] - minScale[j]);
                        float rot = minRot[j] + (float)rand.NextDouble() * (maxRot[j] - minRot[j]);
                        GenerateObject((world[i,k].position), Quaternion.Euler(0, rot, 0), Vector3.one * scale, world[i,k].biome, j, world[i,k]);
                        index++;
                        break;
                    }
                    lower = upper;
                }

                if (index > 100)
                {
                    yield return null;
                    index = 0;
                }

            }
        }
        StartCoroutine(BuildMeshesAll());
    }

    IEnumerator BuildMeshesAll()
    {
        foreach (List<List<GameObject>> biomeList in worldObjects)
        {
            foreach (List<GameObject> objList in biomeList)
            {
                int index = -1;
                foreach (GameObject curObject in objList)
                {
                    if(index % 10 == 0)
                        yield return null;

                    index++;
                    if(!curObject.GetComponent<TerrainObject>().hasReloaded && index != 0)
                        curObject.GetComponent<TerrainObject>().Reload();
                }
            }
        }
        InitiateCity();
        levelLoaded = true;
    }

    void InitiateCity()
    {
        Instantiate(city);
    }

    void GenerateObject(Vector3 position, Quaternion rotation, Vector3 scale, int biome, int objIndex, GridTile gridTile)
    {
        TerrainObject curterrainObject;
        List<GameObject> curGameObjectList = worldObjects[biome][objIndex];
        int count = curGameObjectList.Count;
        if (curGameObjectList.Count < 2 || curGameObjectList[count - 1].GetComponent<TerrainObject>().isFull)
        {
            curGameObjectList.Add(Instantiate(curGameObjectList[0]));
            count = curGameObjectList.Count;
            curGameObjectList[count - 1].transform.parent = transform;
            curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();
            curterrainObject.biome = biome;
            curterrainObject.objectNR = objIndex;
        }
        curterrainObject = curGameObjectList[count - 1].GetComponent<TerrainObject>();
        Mesh[] subMeshes = biomeMeshes[biome].mesh[objIndex];
        for (int i = 0; i < subMeshes.Length; i++)
        {
            curterrainObject.newComponents[i].Add(MoveMesh(subMeshes[i], position, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale));
            curterrainObject.verticesNow += subMeshes[i].vertexCount;
        }

        if (curterrainObject.verticesNow > curterrainObject.vertexMax)
        {
            curterrainObject.isFull = true;
            curterrainObject.hasReloaded = true;
            curterrainObject.Reload();
        }
        GridTileOccupant gridOccupant = new GridTileOccupant(curterrainObject.gameObject, Quaternion.Euler(new Vector3(-90, 0, 0) + rotation.eulerAngles), scale);
        gridTile.occupant = gridOccupant;
        gridTile.type = 1;

    }

    Mesh MoveMesh(Mesh mesh, Vector3 trans, Quaternion rotate, Vector3 scale)
    {
        Mesh result = new Mesh();
        Vector3[] vertNew = new Vector3[mesh.vertexCount];
        int index = 0;
        foreach (Vector3 vert in mesh.vertices)
        {
            vertNew[index] = new Vector3(vert.x * scale.x, vert.y* scale.y, vert.z* scale.z);
            vertNew[index] = rotate * vertNew[index];
            vertNew[index] = vertNew[index] + trans;

            index++;
        }

        result.vertices = vertNew;
        result.subMeshCount = mesh.subMeshCount;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            result.SetTriangles(mesh.GetTriangles(i), i);
        }
        result.uv = mesh.uv;
        result.normals = mesh.normals;
        return result;
    }

    // Get a mesh of a gameobject
    Mesh[] GetSubmeshes(GameObject obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

        if (mesh.vertexCount == 0)
        {
            mesh = obj.GetComponent<MeshFilter>().mesh;
        }
        Mesh[] allMeshes = new Mesh[mesh.subMeshCount];
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            Mesh newMesh = new Mesh();
            List<Vector3> vert = new List<Vector3>();
            Vector3[] allVert = mesh.vertices;
            List<int> finTriangles = new List<int>();
            int[] tri = mesh.GetTriangles(i);
            List<Vector2> uvs, uvs2, uvs3, uvs4;
            uvs = new List<Vector2>();
            uvs2 = new List<Vector2>();
            uvs3 = new List<Vector2>();
            uvs4 = new List<Vector2>();

            for (int j = 0; j < tri.Length; j++)
            {
                int curVertIndex = tri[j];
                if (!vert.Contains(allVert[curVertIndex]))
                {
                    vert.Add(allVert[curVertIndex]);
                    finTriangles.Add(vert.Count - 1);

                    if (mesh.uv != null && mesh.uv.Length > curVertIndex)
                        uvs.Add(mesh.uv[curVertIndex]);
                    if (mesh.uv2 != null && mesh.uv2.Length > curVertIndex)
                        uvs2.Add(mesh.uv2[curVertIndex]);
                    if (mesh.uv3 != null && mesh.uv3.Length > curVertIndex)
                        uvs3.Add(mesh.uv3[curVertIndex]);
                    if (mesh.uv4 != null && mesh.uv4.Length > curVertIndex)
                        uvs4.Add(mesh.uv4[curVertIndex]);
                }
                else
                {
                    Vector3 tryThis = allVert[curVertIndex];
                    finTriangles.Add(vert.IndexOf(tryThis));
                }
            }
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
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            newMesh.Optimize();
            allMeshes[i] = newMesh;
        }


        return allMeshes;
    }

    public IEnumerator SetOccupant(GridTile tile, GridTileOccupant occupant, float cutOffRadius, bool removeOccupants, bool removeSelfBuild, bool setNotBuild)
    {

        RemoveOccupant(tile, false);
        tile.occupant = occupant;

        TerrainController terrain = TerrainController.thisTerrainController;
        int tileSize = terrain.tileSize;

        int thisX = Mathf.RoundToInt((tile.position.x - 0.5f * tileSize) / tileSize);
        int thisZ = Mathf.RoundToInt((tile.position.z - 0.5f * tileSize) / tileSize);

        int startX = thisX - Mathf.RoundToInt(cutOffRadius / tileSize);
        int startZ = thisZ - Mathf.RoundToInt(cutOffRadius / tileSize);

        int maxX = terrain.length / tileSize;
        int maxZ = terrain.width / tileSize;

        for (int i = 0; i < cutOffRadius / tileSize * 2; i++)
        {
            for (int j = 0; j < cutOffRadius / tileSize * 2; j++)
            {
                if (startX + i < 0 || startX + i > maxX || startZ + j < 0 || startZ + j > maxZ)
                    continue;

                GridTile checkGridTile = terrain.world[startX + i, startZ + j];
                if (Vector3.Distance(checkGridTile.position, tile.position) < cutOffRadius && checkGridTile.occupant != null && removeOccupants)
                {
                    RemoveOccupant(checkGridTile, removeSelfBuild);
                    yield return null;
                }
                if (Vector3.Distance(checkGridTile.position, tile.position) < cutOffRadius && setNotBuild)
                    checkGridTile.canBuild = false;
            }
        }
    }

    public void RemoveOccupant(GridTile tile, bool removeSelfBuild)
    {
        Mesh[] removeMesh = null;
        if (tile.type == 0)
            return;
        if (tile.type == 2)
        {
            if (removeSelfBuild)
            {
                Destroy(tile.occupant.obj);
                tile.occupant = null;
            }
            return;
        }
        else if (tile.type == 1)
        {
            TerrainObject terrainObj = tile.occupant.obj.GetComponent<TerrainObject>();

            Mesh[] objMeshes = thisTerrainController.biomeMeshes[terrainObj.biome].mesh[terrainObj.objectNR];
            removeMesh = new Mesh[objMeshes.Length];
            for (int k = 0; k < objMeshes.Length; k++)
            {
                removeMesh[k] = MoveMesh(objMeshes[k], tile.position - terrainObj.transform.position, tile.occupant.rotation, tile.occupant.scale);
            }
            terrainObj.RemoveMesh(removeMesh);
            terrainObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            tile.occupant = null;

            if (removeMesh != null)
            {
                foreach (Mesh destroyMesh in removeMesh)
                {
                    Destroy(destroyMesh);
                }
            }
        }
    }

    public void BuildObject(GameObject obj, Quaternion rotation, Vector3 scale, Vector3 globalPosition, float cutOffRadius, bool removeOccupants, bool removeSelfBuild, bool setNotBuild)
    {
        GridTile tile = GridTile.FindClosestGridTile(globalPosition);
        GameObject objInst = (GameObject)Instantiate(obj, tile.position, rotation);
        objInst.transform.localScale = scale;
        GridTileOccupant tileOccupant = new GridTileOccupant(objInst);
        tile.type = 2;
        StartCoroutine(SetOccupant(tile, tileOccupant, cutOffRadius, removeOccupants, removeSelfBuild, setNotBuild));
    }

    public void BuildObject(GameObject obj, Quaternion rotation, Vector3 scale, GridTile tile, float cutOffRadius, bool removeOccupants, bool removeSelfBuild, bool setNotBuild)
    {
        GameObject objInst = (GameObject)Instantiate(obj, tile.position, rotation);
        objInst.transform.localScale = scale;
        GridTileOccupant tileOccupant = new GridTileOccupant(objInst);
        tile.type = 2;
        StartCoroutine(SetOccupant(tile, tileOccupant, cutOffRadius, removeOccupants, removeSelfBuild, setNotBuild));
    }

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
