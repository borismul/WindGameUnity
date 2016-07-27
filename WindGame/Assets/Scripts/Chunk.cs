using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Chunk : MonoBehaviour{

    int chunkWidth;
    int chunkLength;
    int maxHeight;
    float textureSize = 3;

    int cubeSize;

    float stretch;

    int seed;

    List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    List<Vector3> norm = new List<Vector3>();

    Vector3[,] map;

    Vector3 offset1;
    Vector3 offset2;


    // Use this for initialization
    void Start ()
    {
        chunkWidth = TerrainController.chunkSize;
        chunkLength = TerrainController.chunkSize;
        maxHeight = TerrainController.maxHeight;
        cubeSize = TerrainController.cubeSize;
        stretch = TerrainController.stretch;
        seed = TerrainController.seed;
        Random.seed = seed;
        offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
        offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);

        createMesh();
    }

    void createMesh()
    {
        map = new Vector3[chunkLength / cubeSize + 1, chunkWidth / cubeSize + 1];

        for (int i = 1; i < chunkLength / cubeSize + 2; i++)
        {
            for (int j = 1; j < chunkWidth / cubeSize + 2; j++)
            {
                bool isNull = false;

                for (int k = 1; k < (maxHeight + 3) / cubeSize + 1; k++)
                {
                    float xPos = i * cubeSize - 0.5f * cubeSize + transform.position.x;
                    float yPos = k * cubeSize - 0.5f * cubeSize + transform.position.y;
                    float zPos = j * cubeSize - 0.5f * cubeSize + transform.position.z;

                    float noise = GenerateNoise(xPos, yPos, zPos, stretch, 0, 2, offset1);
                    noise += GenerateNoise(xPos, yPos, zPos, stretch * 10, 0, maxHeight - 2, offset2);

                    if (noise > k* cubeSize)
                    {
                        if (isNull)
                        {
                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos, zPos - transform.position.z);
                            map[i-1, j-1] = pos;
                        }   

                        isNull = false;
                    }
                    else
                    {
                        if (!isNull)
                        {
                            Vector3 pos = new Vector3(xPos - transform.position.x, yPos, zPos - transform.position.z);
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

        float noise = 1 + baseHeight + heightSwing * ((1 + Noise.Generate((x + offset.x) / stretch, (y + offset.y * 10000) / stretch, (z + offset.z * 10000) / stretch)) / 2);
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
                if (pos[0].y > maxHeight * 0.8f)
                    type = 1;
                else if (pos[0].y > maxHeight * 0.7f)
                    type = 7;
                else if (pos[0].y > maxHeight * 0.3f)
                {
                    int[] choices = new int[] { 0, 3, 6 };
                    int choice = Random.Range(0, 3);
                    type = choices[choice];
                }
                else
                {
                    int[] choices = new int[] { 5, 8 };
                    int choice = Random.Range(0, 2);
                    type = choices[choice];
                }
                CreatePlane(pos, type);

                mesh.vertices = vert.ToArray();
                mesh.triangles = tri.ToArray();
                mesh.uv = uv.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
        }
    }

    void CreatePlane(Vector3[] pos, int type = 0)
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
    }
}
