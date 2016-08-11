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
    int nOffset = 3;

    // Lists of vertices, triangles and uvs for the mesh of the chunk
    List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    List<Vector3> norm = new List<Vector3>();

    // Ammount of textures in a line on the chunk texture
    int texturesPerLine = 4;

    // Map terrain and biome of this chunk
    public Vector3[,] map;
    public int[,] biomeMap;

    GridTile previousTile;

    Mesh mesh;

    // Use this for initialization
    void Start()
    {
        bool generateMap = map == null;
        Initialize();

        if (generateMap)
        {
            GenerateTerrain();
        }
        //GenerateGridTiles();
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
        if (map == null)
        {
            map = new Vector3[chunkSize / tileSize + 3, chunkSize / tileSize + 3];
            biomeMap = new int[chunkSize / tileSize + 3, chunkSize / tileSize + 3];
        }

        // Generate the noise offsets
        GenerateNoiseOffsets(nOffset);

        terrain.chunks.Add(this);
    }

    void GenerateTerrain()
    {
        int numTiles = chunkSize / tileSize;
        Vector3 pos = new Vector3();

        for (int i = 0; i < numTiles + 3; i++)
        {
            for (int k = 0; k < numTiles + 3; k++)
            {
                pos.Set((i-1) * tileSize, 0, (k-1) * tileSize);
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

    void GenerateGridTile(List<Vector3> positions, List<int> vertIndices, int biome, int iPos, int jPos)
    {
        int startI = Mathf.RoundToInt(transform.position.x / tileSize);
        int startK = Mathf.RoundToInt(transform.position.z / tileSize);

        float xAvg = 0;
        float yAvg = 0;
        float zAvg = 0;

        List<Vector3> worldPositions = new List<Vector3>();
        for (int i = 0; i< positions.Count; i++)
        {
            xAvg += positions[i].x;
            yAvg += positions[i].y;
            zAvg += positions[i].z;

            worldPositions.Add(positions[i] + transform.position);
        }

       
        Vector3 position = new Vector3(xAvg / positions.Count, yAvg / positions.Count, zAvg / positions.Count);
        terrain.world[startI+ iPos, startK + jPos] = new GridTile(position + transform.position, worldPositions, vertIndices, biome, 0, true, null);
    }

    public void GenerateTerrainMesh()
    {
        vert.Clear();
        tri.Clear();
        uv.Clear();

        int n = map.GetLength(0);

        AddVertsAndUVAndNorm(n);
        AddTrisAndGridTiles(n);
        
        SetMesh(false);
    }

    void AddVertsAndUVAndNorm(int n)
    {
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < n - 1; j++)
            {
                vert.Add(map[i, j]);
                uv.Add(DetermineUV(biomeMap[i, j]));
                AddNormal(map[i, j], map[i - 1, j], map[i, j + 1], map[i + 1, j], map[i, j - 1]);
            }
        }
    }

    void AddTrisAndGridTiles(int n)
    {
        for (int i = 0; i < vert.Count - (n - 3 + n - 2); i++)
        {
            int rowL = Mathf.FloorToInt(i / (n - 3));
            int rowU = rowL + 1;
            int col = i - rowL * (n - 3);
            int BL = rowL * (n - 2) + col;
            int BR = BL + 1;
            int UL = rowU * (n - 2) + col;
            int UR = UL + 1;

            tri.Add(BL);
            tri.Add(UR);
            tri.Add(UL);
            tri.Add(BL);
            tri.Add(BR);
            tri.Add(UR);

            List<Vector3> positions = new List<Vector3>();
            List<int> vertIndices = new List<int>();
            positions.Add(new Vector3(vert[BL].x, vert[BL].y, vert[BL].z));
            int BLcopy = BL;
            vertIndices.Add(BL);
            positions.Add(new Vector3(vert[UL].x, vert[UL].y, vert[UL].z));
            int ULcopy = UL;
            vertIndices.Add(UL);
            positions.Add(new Vector3(vert[UR].x, vert[UR].y, vert[UR].z));
            int URcopy = UR;
            vertIndices.Add(UR);
            positions.Add(new Vector3(vert[BR].x, vert[BR].y, vert[BR].z));
            int BRcopy = BR;
            vertIndices.Add(BR);
            GenerateGridTile(positions, vertIndices, biomeMap[rowL, col], rowL, col);
        }
    }

    void AddNormal(Vector3 curPos, Vector3 left, Vector3 up, Vector3 right, Vector3 down)
    {
        left = left - curPos;
        up = up - curPos;
        right = right - curPos;
        down = down - curPos;

        Vector3 normLeftUp = Vector3.Cross(left, up).normalized;
        Vector3 normUpRight = Vector3.Cross(up, right).normalized;
        Vector3 normRightDown = Vector3.Cross(right, down).normalized;
        Vector3 normDownLeft = Vector3.Cross(down, left).normalized;

        Vector3 normal = (normLeftUp + normUpRight + normRightDown + normDownLeft).normalized;
        norm.Add(normal);
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
        mesh.normals = norm.ToArray();

        if (easy)
        {
            mesh.normals = GetComponent<MeshFilter>().mesh.normals;
            Destroy(GetComponent<MeshFilter>().mesh);
            GetComponent<MeshFilter>().mesh = mesh;
            return;
        }
        Destroy(GetComponent<MeshFilter>().mesh);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    Vector2 DetermineUV(int biome)
    {
        Vector2 uv;
        int row = (biome / texturesPerLine);
        int col = biome - row * texturesPerLine;
        uv = new Vector2((float)col / texturesPerLine + 1f/(texturesPerLine)/2f, (float)row / texturesPerLine + 1f / (texturesPerLine) / 2f);
        return uv;
    }

}
