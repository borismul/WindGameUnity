using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChunkOld : MonoBehaviour
{

    //    int chunkWidth;
    //    int chunkLength;
    //    int maxHeight;
    //    float textureSize = 16;

    //    int cubeSize;
    //    float cubeHeight;

    //    float stretch;

    //    int seed;

    //    List<Vector3> vert = new List<Vector3>();
    //    List<int> tri = new List<int>();
    //    List<Vector2> uv = new List<Vector2>();
    //    List<Vector3> norm = new List<Vector3>();

    //    Vector3[,] map;
    //    int[,] biome;

    //    Vector3 offset1;
    //    Vector3 offset2;
    //    Vector2 offset3;
    //    Vector2 offset4;
    //    Vector2 offset5;

    //    Mesh finalTree;

    //    List<CombineInstance> meshes = new List<CombineInstance>();
    //    System.Random rand;


    //    // Use this for initialization
    //    void Start()
    //    {

    //        Initialize();

    //        createMesh();
    //    }

    //    // Method gets all variables from the terrain generator and sets the offset for the landscape
    //    void Initialize()
    //    {
    //        chunkWidth = TerrainController.statChunkSize;
    //        chunkLength = TerrainController.statChunkSize;
    //        maxHeight = TerrainController.statMaxHeight;
    //        cubeSize = TerrainController.statCubeSize;
    //        cubeHeight = TerrainController.statCubeHeight;
    //        stretch = TerrainController.statStretch;
    //        seed = TerrainController.statSeed;
    //        finalTree = TerrainController.statTreeMesh;
    //        seed = TerrainController.statSeed;

    //        Random.seed = seed;
    //        offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
    //        offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
    //        offset3 = new Vector2(Random.value * 10000, Random.value * 10000);
    //        offset4 = new Vector2(Random.value * 10000, Random.value * 10000);
    //        offset5 = new Vector2(Random.value * 10000, Random.value * 10000);
    //        rand = new System.Random(Mathf.RoundToInt(System.DateTime.Now.Millisecond));
    //    }

    //    void createMesh()
    //    {
    //        map = new Vector3[chunkLength / cubeSize + 1, chunkWidth / cubeSize + 1];
    //        biome = new int[chunkLength / cubeSize + 1, chunkWidth / cubeSize + 1];

    //        for (int i = 1; i < chunkLength / cubeSize + 2; i++)
    //        {
    //            for (int j = 1; j < chunkWidth / cubeSize + 2; j++)
    //            {
    //                bool isNull = false;
    //                float xPos = i * cubeSize - 0.5f * cubeSize + transform.position.x;
    //                float zPos = j * cubeSize - 0.5f * cubeSize + transform.position.z;
    //                float biomeNoise = GenerateNoise(xPos, zPos, stretch * 10, 0, 1, offset3) - 1;
    //                //biomeNoise *= GenerateNoise(xPos, zPos, stretch *5, 1, 2, offset4) - 1;
    //                //biomeNoise *= GenerateNoise(xPos, zPos, stretch * 20, 1, 2, offset5) - 1;
    //                //biomeNoise /= 8;
    //                biomeNoise *= 5;


    //                biome[i - 1, j - 1] = Mathf.FloorToInt(biomeNoise);

    //                for (int k = 1; k < (maxHeight + 3) / cubeSize + 1; k++)
    //                {
    //                    float yPos = k * cubeSize - 0.5f * cubeSize + transform.position.y;


    //                    float noise = GenerateNoise(xPos, yPos, zPos, stretch, 0, 2, offset1);
    //                    noise += GenerateNoise(xPos, yPos, zPos, stretch * 10, 0, maxHeight - 2, offset2);


    //                    if (noise > k * cubeSize)
    //                    {
    //                        if (isNull)
    //                        {
    //                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos * cubeHeight / cubeSize, zPos - transform.position.z);
    //                            map[i - 1, j - 1] = pos;
    //                        }

    //                        isNull = false;
    //                    }
    //                    else
    //                    {
    //                        if (!isNull)
    //                        {
    //                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos * cubeHeight / cubeSize, zPos - transform.position.z);
    //                            map[i - 1, j - 1] = pos;
    //                        }
    //                        isNull = true;
    //                    }
    //                }
    //            }
    //        }

    //        BuildChunk();
    //    }

    //    float GenerateNoise(float x, float y, float z, float stretch, float baseHeight, float maxHeight, Vector3 offset)
    //    {
    //        float heightSwing = maxHeight - baseHeight;

    //        float noise = 1 + baseHeight + heightSwing * ((1 + Noise.Generate((x + offset.x) / stretch, (y + offset.y * 10000) / stretch, (z + offset.z * 10000) / stretch)) / 2);
    //        return noise;
    //    }
    //    float GenerateNoise(float x, float z, float stretch, float baseHeight, float maxHeight, Vector2 offset)
    //    {
    //        float heightSwing = maxHeight - baseHeight;

    //        float noise = 1 + baseHeight + heightSwing * ((1 + Noise.Generate((x + offset.x) / stretch, (z + offset.y * 10000) / stretch)) / 2);
    //        return noise;
    //    }

    //    void BuildChunk()
    //    {
    //        Mesh mesh = new Mesh();
    //        MeshFilter meshFilter = GetComponent<MeshFilter>();
    //        meshFilter.mesh = mesh;

    //        for (int i = 0; i < chunkLength / cubeSize; i++)
    //        {
    //            for (int j = 0; j < chunkWidth / cubeSize; j++)
    //            {
    //                Vector3[] pos = new Vector3[4];
    //                pos[0] = map[i, j];
    //                pos[1] = map[i, j + 1];
    //                pos[2] = map[i + 1, j + 1];
    //                pos[3] = map[i + 1, j];

    //                CreatePlane(pos, 0, biome[i, j], Mathf.Abs(Interpolate(pos[0].x, pos[1].x, pos[2].x, pos[3].x)) / cubeHeight - pos[0].y);
    //            }
    //        }
    //        Mesh terrainMesh = new Mesh();
    //        terrainMesh.vertices = vert.ToArray();
    //        terrainMesh.triangles = tri.ToArray();
    //        terrainMesh.uv = uv.ToArray();
    //        CombineInstance terrainMeshInt = new CombineInstance();
    //        terrainMeshInt.mesh = terrainMesh;
    //        terrainMeshInt.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
    //        meshes.Add(terrainMeshInt);
    //        mesh.CombineMeshes(meshes.ToArray(), true, true);
    //        mesh.RecalculateNormals();
    //        mesh.RecalculateBounds();
    //        mesh.Optimize();

    //        GetComponent<MeshCollider>().sharedMesh = terrainMesh;
    //    }

    //    void CreatePlane(Vector3[] pos, int type, int biome, float steepness)
    //    {
    //        int index = vert.Count;

    //        vert.Add(pos[0]);
    //        vert.Add(pos[1]);
    //        vert.Add(pos[2]);
    //        vert.Add(pos[3]);

    //        tri.Add(index);
    //        tri.Add(index + 1);
    //        tri.Add(index + 3);
    //        tri.Add(index + 1);
    //        tri.Add(index + 2);
    //        tri.Add(index + 3);

    //        float yPos = Interpolate(pos[0].y, pos[1].y, pos[2].y, pos[3].y);

    //        float uvCornerX = biome;
    //        float uvCornerY = 0;
    //        if (yPos / (maxHeight / (cubeSize / cubeHeight)) > 0.8f)
    //        {
    //            uvCornerY = 14;
    //            biome = 8;
    //        }
    //        float uvOffset = 0.05f;
    //        uv.Add(new Vector2(uvCornerX / textureSize + uvOffset / textureSize, uvCornerY / textureSize + uvOffset / textureSize));
    //        uv.Add(new Vector2(uvCornerX / textureSize + uvOffset / textureSize, uvCornerY / textureSize + 1 / textureSize - uvOffset / textureSize));
    //        uv.Add(new Vector2(uvCornerX / textureSize + 1 / textureSize - uvOffset / textureSize, uvCornerY / textureSize + 1 / textureSize - uvOffset / textureSize));
    //        uv.Add(new Vector2(uvCornerX / textureSize + 1 / textureSize - uvOffset / textureSize, uvCornerY / textureSize + uvOffset / textureSize));

    //        if (biome == 0 && steepness < 1)
    //        {
    //            if (myRandomRange(0, 6) > 3)
    //                return;
    //            float scaleXZ = myRandomRange(0.9f, 1.5f);
    //            float scaleY = scaleXZ * myRandomRange(0.8f, 2);
    //            meshes.Add(BuildTree(pos[0], yPos, new Vector3(scaleXZ, scaleY, scaleXZ), Quaternion.Euler(new Vector3(0, myRandomRange(0, 360), 0))));
    //        }
    //        else if (biome == 1 && steepness < 1)
    //        {
    //            float buildTree = myRandomRange(0f, 20f);
    //            if (buildTree < 19)
    //                return;
    //            float scaleXZ = myRandomRange(0.9f, 1.5f);
    //            float scaleY = scaleXZ * myRandomRange(0.8f, 2);
    //            meshes.Add(BuildTree(pos[0], yPos, new Vector3(scaleXZ, scaleY, scaleXZ), Quaternion.Euler(new Vector3(0, myRandomRange(0, 360), 0))));
    //        }

    //        TerrainController.world.Add(new GridTile(new Vector3(pos[0].x + cubeSize * 0.5f + transform.position.x, yPos, pos[0].z + cubeSize * 0.5f + transform.position.z), biome, true, null));
    //    }

    //    float Interpolate(float corner0, float corner1, float corner2, float corner3)
    //    {
    //        float value;
    //        float value2;
    //        value = corner0 + (corner2 - corner0) / 3 * (cubeHeight / cubeSize);
    //        value2 = corner1 + (corner3 - corner1) / 3 * (cubeHeight / cubeSize);

    //        value = Mathf.Min(new float[] { value, value2 });

    //        return value;
    //    }

    //    CombineInstance BuildTree(Vector3 corner0, float yPos, Vector3 scale, Quaternion rotation)
    //    {
    //        Vector3 position = new Vector3(corner0.x + cubeSize * 0.5f, yPos, corner0.z + cubeSize * 0.5f);
    //        CombineInstance tree = new CombineInstance();

    //        Matrix4x4 mat = Matrix4x4.TRS(position, rotation, scale);
    //        tree.mesh = finalTree;
    //        tree.transform = mat;
    //        return tree;
    //    }

    //    float myRandomRange(float a, float b)
    //    {
    //        return a + (float)rand.NextDouble() * (b - a);
    //    }

    //    int myRandomRange(int a, int b)
    //    {
    //        return a + rand.Next() * (b - a);
    //    }
}
