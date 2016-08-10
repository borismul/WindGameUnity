using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterChunk : MonoBehaviour {

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    TerrainController terrain;

    int size;
    int tileSize;
    float level;
    int seed;
    int octaves;
    float persistance;
    float frequency;
    float maxWaveHeight;

    Mesh mesh = new Mesh();

    void Start()
    {
        Initialize();
        CreateWater();
        CreateMesh();
    }

    void Update()
    {
        UpdateWater();
        CreateMesh();
    }

    void Initialize()
    {
        terrain = TerrainController.thisTerrainController;
        terrain.waterChunks.Add(this);

        size = terrain.waterChunkSize;
        tileSize = terrain.waterTileSize;
        level = terrain.waterLevel;
        seed = terrain.seed;
        octaves = terrain.waterOctaves;
        persistance = terrain.waterPersistance;
        frequency = terrain.waterFrequency;
        maxWaveHeight = terrain.maxWaveHeight;

        Random.seed = seed;
    }

    void CreateWater()
    {
        List<Vector3> panelCorners = new List<Vector3>();

        for (int i = 0; i< size / tileSize; i++)
        {
            for (int j = 0; j < size / tileSize; j++)
            {
                panelCorners.Clear();
                panelCorners.Add(new Vector3(i * tileSize, level, j * tileSize));
                panelCorners.Add(new Vector3(i * tileSize, level, j * tileSize + tileSize));
                panelCorners.Add(new Vector3(i * tileSize + tileSize, level, j * tileSize + tileSize));
                panelCorners.Add(new Vector3(i * tileSize + tileSize, level, j * tileSize));

                CreatePanel(panelCorners);
            }
        }
    }

    void CreatePanel(List<Vector3> panelCorners)
    {
        int index = verts.Count;
        verts.AddRange(panelCorners);

        tris.Add(index);
        tris.Add(index + 1);
        tris.Add(index + 2);
        tris.Add(index);
        tris.Add(index + 2);
        tris.Add(index + 3);
    }

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void UpdateWater()
    {
        int index = 0;
        foreach (Vector3 vert in verts)
        {
            float diff = Noise.PerlinNoise(new Vector2(vert.x, vert.z) + new Vector2(transform.position.x, transform.position.z), Vector2.up * Time.timeSinceLevelLoad * 100, octaves, persistance, frequency, -maxWaveHeight, maxWaveHeight);
            verts[index] = (new Vector3(verts[index].x, level + diff, verts[index].z));
            index++;
        }
    }
}
