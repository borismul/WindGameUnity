using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Chunk : MonoBehaviour{

    int chunkWidth;
    int chunkLength;
    int maxHeight;
    float textureSize = 3;

    int cubeSize;
    float cubeHeight;

    float stretch;

    int seed;

    List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    List<Vector3> norm = new List<Vector3>();

    Vector3[,] map;
    int[,] biome;

    Vector3 offset1;
    Vector3 offset2;
    Vector2 offset3;

    Mesh treeMesh;
    Mesh treeTop;

    Mesh finalTree = new Mesh();

    List<CombineInstance> meshes = new List<CombineInstance>();


    // Use this for initialization
    void Start ()
    {
        chunkWidth = TerrainController.statChunkSize;
        chunkLength = TerrainController.statChunkSize;
        maxHeight = TerrainController.statMaxHeight;
        cubeSize = TerrainController.statCubeSize;
        cubeHeight = TerrainController.statCubeHeight;
        stretch = TerrainController.statStretch;
        seed = TerrainController.statSeed;
        Random.seed = seed;
        treeMesh = TerrainController.statTreeMesh;
        treeTop = TerrainController.statTreeTopMesh;
        offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        offset3 = new Vector2(Random.value * 10000, Random.value * 10000);

        FinalTree();

        createMesh();
    }

    void FinalTree()
    {
        CombineInstance[] treeParts = new CombineInstance[2];
        CombineInstance treePart1 = new CombineInstance();
        CombineInstance treePart2 = new CombineInstance();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < treeMesh.vertexCount; i++)
        {
            uvs.Add(new Vector2(3f/4f, 3f / 4f));
        }


        treePart1.mesh = treeMesh;
        treeMesh.uv = uvs.ToArray();
        treePart1.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90, 0, 0), Vector3.one);
        treeParts[0] = treePart1;
        treePart2.mesh = treeTop;
        treePart2.transform = Matrix4x4.TRS(Vector3.zero + new Vector3(0, 8, 0), Quaternion.Euler(-90,0,0), new Vector3(2,2,2f));
        treeParts[1] = treePart2;
        finalTree.CombineMeshes(treeParts, true, true);

    }
    void createMesh()
    {
        map = new Vector3[chunkLength / cubeSize + 1, chunkWidth / cubeSize + 1];
        biome = new int[chunkLength / cubeSize + 1, chunkWidth / cubeSize + 1];

        for (int i = 1; i < chunkLength / cubeSize + 2; i++)
        {
            for (int j = 1; j < chunkWidth / cubeSize + 2; j++)
            {
                bool isNull = false;
                float xPos = i * cubeSize - 0.5f * cubeSize + transform.position.x;
                float zPos = j * cubeSize - 0.5f * cubeSize + transform.position.z;
                float biomeNoise = GenerateNoise(xPos, zPos, stretch, 0, 3, offset3);

                if (biomeNoise > 3)
                {
                    biome[i - 1, j - 1] = 1;
                }
                else
                {
                    biome[i - 1, j - 1] = 0;
                }


                for (int k = 1; k < (maxHeight + 3) / cubeSize + 1; k++)
                {
                    float yPos = k * cubeSize - 0.5f * cubeSize + transform.position.y;


                    float noise = GenerateNoise(xPos, yPos, zPos, stretch, 0, 2, offset1);
                    noise += GenerateNoise(xPos, yPos, zPos, stretch * 10, 0, maxHeight - 2, offset2);


                    if (noise > k* cubeSize)
                    {
                        if (isNull)
                        {
                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos * cubeHeight/cubeSize, zPos - transform.position.z);
                            map[i-1, j-1] = pos;
                        }   

                        isNull = false;
                    }
                    else
                    {
                        if (!isNull)
                        {
                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos * cubeHeight / cubeSize, zPos - transform.position.z);
                            map[i-1, j-1] = pos;
                        }
                        isNull = true;
                    }
                }
            }
        }

        BuildChunk();
    }

    float GenerateNoise(float x, float y, float z, float stretch, float baseHeight, float maxHeight, Vector3 offset)
    {
        float heightSwing = maxHeight - baseHeight;

        float noise = 1 + baseHeight + heightSwing * ((1 + Noise.Generate((x + offset.x) / stretch, (y + offset.y * 10000)/stretch, (z + offset.z * 10000) / stretch)) / 2);
        return noise;
    }
    float GenerateNoise(float x, float z, float stretch, float baseHeight, float maxHeight, Vector2 offset)
    {
        float heightSwing = maxHeight - baseHeight;

        float noise = 1 + baseHeight + heightSwing * ((1 + Noise.Generate((x + offset.x) / stretch, (z + offset.y * 10000) / stretch)) / 2);
        return noise;
    }

    void BuildChunk()
    {
        Mesh mesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        for (int i = 0; i < chunkLength / cubeSize; i++)
        {
            for (int j = 0; j < chunkWidth / cubeSize; j++)
            {
                Vector3[] pos = new Vector3[4];
                pos[0] = map[i, j];
                pos[1] = map[i, j + 1];
                pos[2] = map[i + 1, j + 1];
                pos[3] = map[i + 1, j];

                int type;
                if (pos[0].y > maxHeight * cubeHeight/cubeSize * 0.8f)
                    type = 1;
                else if (pos[0].y > maxHeight * cubeHeight / cubeSize * 0.7f)
                    type = 7;
                else if (pos[0].y > maxHeight * cubeHeight / cubeSize * 0.3f)
                {
                    int[] choices = new int[] { 0, 3, 6 };
                    int choice = Random.Range(0, 0);
                    type = choices[choice];
                }
                else
                {
                    int[] choices = new int[] { 5, 8 };
                    int choice = Random.Range(1, 1);
                    type = choices[choice];
                }
                CreatePlane(pos, type, biome[i,j], Mathf.Abs(Interpolate(pos[0].x, pos[1].x, pos[2].x, pos[3].x))/cubeHeight - pos[0].y);
            }
        }
        Mesh terrainMesh = new Mesh();
        terrainMesh.vertices = vert.ToArray();
        terrainMesh.triangles = tri.ToArray();
        terrainMesh.uv = uv.ToArray();
        CombineInstance terrainMeshInt = new CombineInstance();
        terrainMeshInt.mesh = terrainMesh;
        terrainMeshInt.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        meshes.Add(terrainMeshInt);
        mesh.CombineMeshes(meshes.ToArray(), true, true);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = terrainMesh;
    }

    void CreatePlane(Vector3[] pos, int type, int biome, float steepness)
    {
        int index = vert.Count;

        vert.Add(pos[0]);
        vert.Add(pos[1]);
        vert.Add(pos[2]);
        vert.Add(pos[3]);

        tri.Add(index);
        tri.Add(index + 1);
        tri.Add(index + 3);
        tri.Add(index + 1);
        tri.Add(index + 2);
        tri.Add(index + 3);

        float uvCornerX = 0;
        float uvCornerY = 0;

        switch (type)
        {
            case 0:
                uvCornerX = 0;
                uvCornerY = 0;
                break;
            case 1:
                uvCornerX = 1;
                uvCornerY = 0;
                break;

            case 2:
                uvCornerX = 2;
                uvCornerY = 0;
                break;

            case 3:
                uvCornerX = 0;
                uvCornerY = 1;
                break;

            case 4:
                uvCornerX = 1;
                uvCornerY = 1;
                break;

            case 5:
                uvCornerX = 2;
                uvCornerY = 1;
                break;

            case 6:
                uvCornerX = 0;
                uvCornerY = 2;
                break;

            case 7:
                uvCornerX = 1;
                uvCornerY = 2;
                break;

            case 8:
                uvCornerX = 2;
                uvCornerY = 2;
                break;

        }
        uv.Add(new Vector2(uvCornerX / textureSize, uvCornerY / textureSize));
        uv.Add(new Vector2(uvCornerX / textureSize, uvCornerY / textureSize + 1/textureSize));
        uv.Add(new Vector2(uvCornerX / textureSize + 1/textureSize, uvCornerY / textureSize + 1/textureSize));
        uv.Add(new Vector2(uvCornerX / textureSize + 1/textureSize, uvCornerY / textureSize));
        float yPos = Interpolate(pos[0].y, pos[1].y, pos[2].y, pos[3].y);

        if (biome == 1 && steepness < 1)
        {
            float scaleXZ = Random.Range(0.9f, 1.5f);
            float scaleY = scaleXZ * Random.Range(0.8f, 2);
            meshes.Add(BuildTree(pos[0], yPos, new Vector3(scaleXZ, scaleY, scaleXZ), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))));
        }

        TerrainController.world.Add(new GridTile(new Vector3(pos[0].x + cubeSize * 0.5f + transform.position.x, yPos, pos[0].z + cubeSize * 0.5f + transform.position.z), biome, true, null));
    }

    float Interpolate(float corner0, float corner1, float corner2, float corner3)
    {
        float value;
        float value2;
        value = corner0 + (corner2 - corner0) / 3 * (cubeHeight / cubeSize);
        value2 = corner1 + (corner3 - corner1) / 3 * (cubeHeight / cubeSize);

        value = Mathf.Min(new float[] { value, value2 });

        return value;
    }

    CombineInstance BuildTree(Vector3 corner0, float yPos, Vector3 scale, Quaternion rotation)
    {
        Vector3 position = new Vector3(corner0.x + cubeSize * 0.5f, yPos, corner0.z + cubeSize * 0.5f);
        CombineInstance tree = new CombineInstance();

        Matrix4x4 mat = Matrix4x4.TRS(position, rotation, scale);
        tree.mesh = finalTree;
        tree.transform = mat;
        return tree;
    }
}
