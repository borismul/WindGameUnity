using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterChunk : MonoBehaviour {

    List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    List<Vector3> norm = new List<Vector3>();

    TerrainController terrain;

    int size;
    int tileSize;
    float level;
    int seed;
    int octaves;
    float persistance;
    float frequency;
    float maxWaveHeight;

    Mesh mesh;

    Vector3[,] map;

    void Start()
    {
        Initialize();
        CreateWater();
        GenerateTerrainMesh();
        CreateMesh();
    }

    void Update()
    {
        if (Vector3.Magnitude(transform.position - terrain.middlePoint) > terrain.length)
            return;

        //UpdateWater();
        //CreateMesh();
    }

    void Initialize()
    {
        mesh = new Mesh();

        terrain = TerrainController.thisTerrainController;
        terrain.waterChunks.Add(this);
        GetComponent<Renderer>().material.renderQueue = 3100;

        size = terrain.waterChunkSize;
        tileSize = terrain.waterTileSize;
        level = terrain.waterLevel;
        seed = terrain.seed;
        octaves = terrain.waterOctaves;
        persistance = terrain.waterPersistance;
        frequency = terrain.waterFrequency;
        maxWaveHeight = terrain.maxWaveHeight;

        Random.InitState(seed);

        map = new Vector3[size / tileSize + 3, size / tileSize + 3];
    }

    void CreateWater()
    {
        for (int i = 0; i < size / tileSize + 3; i++)
        {
            for (int j = 0; j < size / tileSize + 3; j++)
            {
                map[i, j] = new Vector3((i - 1) * tileSize, level, (j - 1) * tileSize);
            }
        }
    }

    public void GenerateTerrainMesh()
    {
        vert.Clear();
        tri.Clear();
        uv.Clear();

        int n = map.GetLength(0);

        AddVertsAndUVAndNorm(n);
        AddTris(n);

        CreateMesh();
    }

    void AddVertsAndUVAndNorm(int n)
    {
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < n - 1; j++)
            {
                vert.Add(map[i, j]);
                AddNormal(map[i, j], map[i - 1, j], map[i, j + 1], map[i + 1, j], map[i, j - 1]);
            }
        }
    }

    void AddTris(int n)
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

            //if (!AboveGround(vert[BL]) && !AboveGround(vert[BR]) && !AboveGround(vert[UL]) && !AboveGround(vert[UR]))
            //    continue;

            tri.Add(BL);
            tri.Add(UR);
            tri.Add(UL);
            tri.Add(BL);
            tri.Add(BR);
            tri.Add(UR);
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

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vert.ToArray();
        mesh.triangles = tri.ToArray();

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void UpdateWater()
    {
        vert.Clear();
        tri.Clear();
        for (int i = 1; i < map.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                Vector3 vertex = map[i, j];
                float diff;

                diff = Noise.PerlinNoise(new Vector2(vertex.x, vertex.z) + new Vector2(transform.position.x, transform.position.z), Vector2.up * ((Time.timeSinceLevelLoad * 100) + 10000000), octaves, persistance, frequency, -maxWaveHeight, maxWaveHeight);

                map[i, j] = (new Vector3(vertex.x, level + diff, vertex.z));

                vert.Add(map[i, j]);
            }
        }

        AddTris(map.GetLength(0));
    }

    bool AboveGround(Vector3 vertex)
    {
        if (GridTile.FindClosestGridTile(vertex + transform.position) != null)
            return (vertex.y > GridTile.FindClosestGridTile(vertex + transform.position).position.y);
        else
            return false;
    }
}
