using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour {

    // The TerrainController that created this chunk
    TerrainController terrain;

    // Level details
    int maxHeight;
    int seed;

    // Noise details
    int terOctaves;
    float terFrequency;
    float terPersistance;
    int bioOctaves;
    float bioFrequency;
    float bioPersistance;

    // Chunk Details
    GameObject chunkPrefab;
    int chunkSize;
    int tileSize;

    // List of 3D, 2D offsets that is used to calculate noise at different x,y,z and x,y
    List<Vector3> offset3D = new List<Vector3>();
    List<Vector2> offset2D = new List<Vector2>();
    // Number of offsets to create
    int nOffset = 20;

    // Lists of vertices, triangles and uvs for the mesh of the chunk
    List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    List<Vector2> uv = new List<Vector2>();

    // Ammount of textures in a line on the chunk texture
    int texturesPerLine = 4;

    // Map terrain and biome of this chunk
    Vector3[,] map;
    int[,] biomeMap;

    int previousHighlightI;
    int previousHighlightK;

    Mesh mesh;

    // Use this for initialization
    void Start()
    {
        Initialize();
        GenerateTerrain();
        GenerateGridTiles();
        GenerateTerrainMesh();
    }

    void Initialize()
    {
        // get the terrainController
        terrain = TerrainController.thisTerrainController;

        // Obtain level details
        maxHeight = terrain.maxHeight;
        seed = terrain.seed;

        // Obtain noise details
        terOctaves = terrain.terrainOctaves;
        terFrequency = terrain.terrainFrequency;
        terPersistance = terrain.terrainPersistance;

        bioOctaves = terrain.biomeOctaves;
        bioFrequency = terrain.biomeFrequency;
        bioPersistance = terrain.biomePersistance;

        // Obtain chunk Details
        chunkSize = terrain.chunkSize;
        tileSize = terrain.tileSize;

        // set seed
        Random.seed = seed;

        // Initialize the terrain map of this chunk
        map = new Vector3[chunkSize / tileSize + 1, chunkSize / tileSize + 1];
        biomeMap = new int[chunkSize / tileSize + 1, chunkSize / tileSize + 1];

        // Generate the noise offsets
        GenerateNoiseOffsets(nOffset);
    }

    void GenerateTerrain()
    {
        int numTiles = chunkSize / tileSize;

        for (int i = 0; i < numTiles + 1; i++)
        {
            for (int k = 0; k < numTiles + 1; k++)
            {
                Vector3 pos = new Vector3(i * tileSize, 0, k * tileSize);

                GenerateTerrainMap(i, k, pos);
                GenerateBiomes(i, k, pos);
            }
        }
    }

    void GenerateTerrainMap(int i, int k, Vector3 pos)
    {
        Vector2 pos2D = new Vector2(pos.x + transform.position.x, pos.z + transform.position.z);
        pos.y = maxHeight * (Noise.PerlinNoise(pos2D, offset2D[0], terOctaves, terPersistance, terFrequency, 0, 1)) + 1;
        map[i, k] = pos;
    }

    void GenerateBiomes(int i, int k, Vector3 pos)
    {
        float noise = (terrain.biomes.Length) * (Noise.PerlinNoise(new Vector2(pos.x, pos.z) + new Vector2(transform.position.x, transform.position.z), offset2D[1], bioOctaves, bioPersistance, bioFrequency, 0, 1));
        biomeMap[i, k] = Mathf.Clamp(Mathf.FloorToInt(noise), 0, terrain.biomes.Length - 1);
    }

    void GenerateGridTiles()
    {
        int startI = Mathf.RoundToInt(transform.position.x / tileSize);
        int startK = Mathf.RoundToInt(transform.position.z / tileSize);

        for (int i = 0; i < chunkSize / tileSize; i++)
        {
            for (int k = 0; k < chunkSize / tileSize; k++)
            {
                terrain.world[startI + i, startK + k] = (new GridTile(map[i, k] + new Vector3((float)tileSize / 2, (map[i, k].y + map[i, k + 1].y + map[i + 1, k].y + map[i + 1, k + 1].y) / 4 - map[i, k].y, (float)tileSize / 2) + transform.position, biomeMap[i, k], 0, true, null));
            }
        }
    }

    public void GenerateTerrainMesh()
    {
        vert = new List<Vector3>();
        tri = new List<int>();
        uv = new List<Vector2>();

        for (int i = 0; i < chunkSize / tileSize; i++)
        {
            for (int k = 0; k < chunkSize / tileSize; k++)
            {
                Vector3 LB = map[i, k];

                List<Vector3> pos = new List<Vector3>();

                Vector3 LT = map[i, k + 1];
                Vector3 RT = map[i + 1, k + 1];
                Vector3 RB = map[i + 1, k];

                pos.Add(LB);
                pos.Add(LT);
                pos.Add(RT);
                pos.Add(RB);

                CreatePlane(pos, biomeMap[i, k]);
            }
        }

        SetMesh(false);
    }

    void GenerateNoiseOffsets(int nOffset)
    {
        for (int i = 0; i < nOffset; i++)
        {
            Vector2 D2 = new Vector2(Random.value * 10000, Random.value * 10000);
            Vector3 D3 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);

            offset2D.Add(D2);
            offset3D.Add(D3);
        }
    }

    void SetMesh(bool easy)
    {
        if(mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.vertices = vert.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.uv = uv.ToArray();

        if (easy)
        {
            mesh.normals = GetComponent<MeshFilter>().mesh.normals;
            Destroy(GetComponent<MeshFilter>().mesh);
            //GetComponent<MeshFilter>().mesh.Clear();
            GetComponent<MeshFilter>().mesh = mesh;
            return;
        }
        Destroy(GetComponent<MeshFilter>().mesh);
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void CreatePlane(List<Vector3> vertices, int biome)
    {
        int index = vert.Count;
        vert.AddRange(vertices);

        tri.Add(index);
        tri.Add(index + 1);
        tri.Add(index + 2);
        tri.Add(index);
        tri.Add(index + 2);
        tri.Add(index + 3);

        uv.AddRange(DetermineUV(biome));
    }

    public void HighLightTile(int iHighLight, int kHighLight)
    {
        int startIndex = (iHighLight * chunkSize / tileSize + kHighLight) * 4;

        List<Vector2> uvs = DetermineUV(texturesPerLine * texturesPerLine - texturesPerLine + biomeMap[iHighLight, kHighLight]);

        for (int i = 0; i < 4; i++)
        {
            uv[startIndex + i] = uvs[i];
        }

        if(previousHighlightI != iHighLight || previousHighlightK != kHighLight)
            DeHighLight();

        previousHighlightI = iHighLight;
        previousHighlightK = kHighLight;
    }

    public void DeHighLight()
    {
        int startIndex = (previousHighlightI * chunkSize / tileSize + previousHighlightK) * 4;

        List<Vector2> uvs = DetermineUV(biomeMap[previousHighlightI, previousHighlightK]);

        for (int i = 0; i < 4; i++)
        {
            uv[startIndex + i] = uvs[i];
        }

        SetMesh(true);
    }

    List<Vector2> DetermineUV(int biome)
    {
        List<Vector2> uv = new List<Vector2>();
        int row = (biome / texturesPerLine);
        int col = biome - row * texturesPerLine;
        float offset = 0.12f / texturesPerLine;
        float textureSize = 1f / texturesPerLine;
        Vector2 corner = new Vector2((float)col / texturesPerLine, (float)row / texturesPerLine);

        uv.Add(new Vector2(corner.x + offset, corner.y + offset));
        uv.Add(new Vector2(corner.x + offset, corner.y + textureSize - offset));
        uv.Add(new Vector2(corner.x + textureSize - offset, corner.y + textureSize - offset));
        uv.Add(new Vector2(corner.x + textureSize - offset, corner.y + offset));

        return uv;
    }

}
