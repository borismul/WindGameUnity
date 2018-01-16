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
    [HideInInspector]
    public int chunkSize;
    [HideInInspector]
    public int tileSize;

    // List of 3D, 2D offsets that is used to calculate noise at different x,y,z and x,y
    List<Vector3> offset3D = new List<Vector3>();
    List<Vector2> offset2D = new List<Vector2>();
    // Number of offsets to create
    int nOffset = 3;

    // Lists of vertices, triangles and uvs for the mesh of the chunk
    public List<Vector3> vert = new List<Vector3>();
    List<int> tri = new List<int>();
    public List<Vector2> uv = new List<Vector2>();
    List<Vector3> norm = new List<Vector3>();

    // Ammount of textures in a line on the chunk texture
    int texturesPerLine = 4;

    // Map terrain and biome of this chunk
    public Vector3[,] map;
    public Vector3[,] tempMap;
    public float[,] biomeMap;

    // Mesh of this chunk
    Mesh mesh;

    public List<GridTile> tiles = new List<GridTile>();

    List<TerrainObject> terrainObjects = new List<TerrainObject>();

    public bool isActive;

    public Renderer ren;

    // Use this for initialization
    void Start()
    {
        bool generateMap = map == null;
        Initialize();

        if (generateMap)
        {
            GenerateTerrain();
        }
        GenerateTerrainMesh(false, false);

        ren = GetComponent<Renderer>();
    }

    public void Disable()
    {
        ren.enabled = false;

        for (int i = 0; i < terrainObjects.Count; i++)
        {
            terrainObjects[i].ren.enabled = false;
            terrainObjects[i].isEnabled = false;
        }

        isActive = false;

    }

    public void Enable()
    {
        ren.enabled = true;

        for (int i = 0; i < terrainObjects.Count; i++)
        {
            terrainObjects[i].ren.enabled = true;
            terrainObjects[i].isEnabled = true;

        }
        isActive = true;

    }

    // Initialization of important attributes
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
        Random.InitState(seed);

        // Initialize the terrain map of this chunk
        if (map == null)
        {
            map = new Vector3[chunkSize / tileSize + 3, chunkSize / tileSize + 3];
            biomeMap = new float[chunkSize / tileSize + 3, chunkSize / tileSize + 3];
        }

        // Generate the noise offsets
        GenerateNoiseOffsets(nOffset);

        terrain.chunks.Add(this);
    }

    // Generate the terrain vertices and their respective biomes
    void GenerateTerrain()
    {
        // Determine number of tiles in a row
        int numTiles = chunkSize / tileSize;

        // Initialize a position vector
        Vector3 pos = new Vector3();

        // Loop through all grid tiles plus 3 extra. 2 on the borders (which overlap with other chunk) to calculate normals from.
        // And one extra to get the desired number of tiles from vertices. (numTiles = numVertex + 1)
        for (int i = 0; i < numTiles + 3; i++)
        {
            for (int k = 0; k < numTiles + 3; k++)
            {
                // Set the position vector to the the position of the current vertex
                pos.Set((i-1) * tileSize, 0, (k-1) * tileSize);
                float r = Vector3.Magnitude(pos + transform.position - terrain.middlePoint);

                // In case the flat area surrounded by mountains options is activated
                if (terrain.isSourroundedByMountains)
                {
                    if (Mathf.Abs(r) > terrain.flatRadius || Mathf.Abs(r) > terrain.flatRadius)
                    {
                        map[i, k] = GenerateTerrainMap(i, k, pos);

                        map[i, k].y *= (-(Mathf.Cos(Mathf.PI/(terrain.width/2f) * (r - terrain.flatRadius))) + 1) / 2;

                    }
                    else
                    {
                        map[i, k] = pos;
                    }
                }
                // In other cases
                else
                {
                    // Generate the height and biome of the vertex, depending on its horizontal position.
                    map[i, k] = GenerateTerrainMap(i, k, pos);

                    if (terrain.isIsland)
                        map[i, k] += new Vector3(0, -terrain.islandSteepness * r * r + terrain.baseHeight + terrain.waterLevel, 0);

                    else if (terrain.isCoastLine)
                        map[i, k].y -= Mathf.Sin(Mathf.PI/(terrain.width*2) * (pos.x + transform.position.x)) * (pos.x + transform.position.x) * terrain.coastSteepness - terrain.coastBaseHeight;
                }

                biomeMap[i, k] = GenerateBiomes(i, k, pos);
                if (terrain.isCoastLine)
                {
                    biomeMap[i, k] *= Mathf.Pow(Mathf.Cos(Mathf.PI / (terrain.width * 2) * (pos.x + transform.position.x)), 5) * 2;
                    biomeMap[i, k] = Mathf.Clamp(biomeMap[i, k], 0.1f, terrain.biomes.Length - 0.5f);
                }
            }
        }
        tempMap = (Vector3[,])map.Clone();
    }

    // Generate noise in y direction at a horizonal plane position, return the vector3 including the noise on the y component.
    Vector3 GenerateTerrainMap(int i, int k, Vector3 pos)
    {
        // Put the position in a vector2
        Vector2 pos2D = new Vector2(pos.x + transform.position.x, pos.z + transform.position.z);

        // Determine the perlin noise with the set maximum height depending on octaves, persistance and frequency
        pos.y = maxHeight * (Noise.PerlinNoise(pos2D, offset2D[0], terOctaves, terPersistance, terFrequency, 0, 1)) + 1;
        return pos;
    }

    // Generate biome noise at a position on a horizontal plane
    float GenerateBiomes(int i, int k, Vector3 pos)
    {
        // Gernerate the perlin noise based on position, octaves, persistance, frequency and number of set biomes.
        float noise = (terrain.biomes.Length) * (Noise.PerlinNoise(new Vector2(pos.x, pos.z) + new Vector2(transform.position.x, transform.position.z), offset2D[1], bioOctaves, bioPersistance, bioFrequency, 0, 1));
        return Mathf.Clamp(noise, 0, terrain.biomes.Length - 0.5f); ;
    }

    // Function generates a grid tile in the world 2d Array in TerrainController
    void GenerateGridTile(List<Vector3> positions, float biome, int iPos, int jPos)
    {
        int startI = Mathf.RoundToInt(transform.position.x / tileSize);
        int startK = Mathf.RoundToInt(transform.position.z / tileSize);

        float xAvg = 0;
        float yAvg = 0;
        float zAvg = 0;

        List<GridNode> worldPositions = new List<GridNode>();
        for (int i = 0; i < positions.Count; i++)
        {
            xAvg += positions[i].x;
            yAvg += positions[i].y;
            zAvg += positions[i].z;

            GridNode existingNode = GridNode.FindGridNode(positions[i] + transform.position);
            if (existingNode == null)
                existingNode = new GridNode(positions[i] + transform.position);
           
            worldPositions.Add(existingNode);
        }


        //Vector3 position = new Vector3(xAvg / positions.Count, yAvg / positions.Count, zAvg / positions.Count);
        Vector3 position = positions[0];
        //for (int i = 1; i < 4; i++)
        //{
        //    if (positions[i].y < position.y)
        //        position = positions[i];
        //}

        bool isUnderWater = yAvg / positions.Count < TerrainController.thisTerrainController.waterLevel;
        GridTile tile = new GridTile(position + transform.position, this, worldPositions, biome, isUnderWater, false, new List<GridTileOccupant>());

        terrain.worldNodes[startI + iPos, startK + jPos] = worldPositions[0];
        terrain.worldNodes[startI + iPos + 1, startK + jPos] = worldPositions[1];
        terrain.worldNodes[startI + iPos, startK + jPos  + 1] = worldPositions[3];
        terrain.worldNodes[startI + iPos + 1, startK + jPos + 1] = worldPositions[2];

        terrain.world[startI + iPos, startK + jPos] = tile;
        tiles.Add(tile);
    }

    // Generate the mesh of this chunk
    public void GenerateTerrainMesh(bool update, bool isTemp)
    {
        // Clear all lists of vertices, triangles, normals and uvs
        vert.Clear();
        tri.Clear();
        if(!update)
            uv.Clear();
        norm.Clear();

        // Determine dimension of map.
        int n = map.GetLength(0);

        // Calculate vertices uvs and normals and add them to the lists
        AddVertsAndUVAndNorm(n, update, isTemp);

        if(isTemp)
            tempMap = (Vector3[,])map.Clone();

        // calculate the triangles and grid tiles
        AddTrisAndGridTiles(n, update);
        
        // Set the mesh the the MeshFilter of this GameOject
        SetMesh(terrain.isFlatShaded);
    }

    // Calculate vertices, uvs and normal and add them to their corresponding lists
    public void AddVertsAndUVAndNorm(int n, bool update, bool isTemp)
    {
        // Loop through each of the vertices in map except for first and last one. (Those are the overlapping vertices with the chunk next to it)
        for (int i = 1; i < n - 1; i++)
        {
            for (int j = 1; j < n - 1; j++)
            {
                if (!isTemp)
                    // Add this vertex to the vert list
                    vert.Add(map[i, j]);
                else
                    vert.Add(tempMap[i, j]);

                if (!update)
                {
                    // Determine the uv of the vertex, depending on the biome
                    uv.Add(DetermineUV(biomeMap[i-1, j-1], vert[vert.Count -1]));
                }

                if(!isTemp)
                    // Add the normal of the vertex. (Takes into account the overlapping vertices)
                    AddNormal(map[i, j], map[i - 1, j], map[i, j + 1], map[i + 1, j], map[i, j - 1]);
                else
                    AddNormal(tempMap[i, j], tempMap[i - 1, j], tempMap[i, j + 1], tempMap[i + 1, j], tempMap[i, j - 1]);
            }
        }
    }

    // Calculate triangles and Generate Grid tiles
    void AddTrisAndGridTiles(int n, bool update)
    {
        // Loop through each of the vertics in the vert list
        for (int i = 0; i < vert.Count - (n - 3 + n - 2); i++)
        {
            // Determine the row in which vertex of the lower part of the panel is located
            int rowL = Mathf.FloorToInt(i / (n - 3));
            // Determine the upper part row of the panel
            int rowU = rowL + 1;
            // Determine the collumn in which the left part of the panel is situated
            int col = i - rowL * (n - 3);

            // Determine the index of the bottom left vertex
            int BL = rowL * (n - 2) + col;
            // Bottom right
            int BR = BL + 1;
            // Upper left
            int UL = rowU * (n - 2) + col;
            // Upper Right
            int UR = UL + 1;

            // Add trianle Bottom left, Upper Right, Upper Left
            tri.Add(BL);
            tri.Add(UR);
            tri.Add(UL);

            // Add triangle Bottom left , Bottom Right, Upper Right
            tri.Add(BL);
            tri.Add(BR);
            tri.Add(UR);
            // Results in a square panel (Top view)


            // Make a list of positions of vertices of which the gridTile consists
            List<Vector3> positions = new List<Vector3>();
            List<int> vertIndex = new List<int>();

            positions.Add(vert[BL]);
            positions.Add(vert[UL]);
            positions.Add(vert[UR]);
            positions.Add(vert[BR]);

            if (!update)
                // Generate the grid tile
                GenerateGridTile(positions, biomeMap[rowL, col], rowL, col);
        }
    }

    // Determines the normal of a vertex depending on its four neighbours
    void AddNormal(Vector3 curPos, Vector3 left, Vector3 up, Vector3 right, Vector3 down)
    {
        // Determine difference vectors of the neighbours wrt the curPos
        left = left - curPos;
        up = up - curPos;
        right = right - curPos;
        down = down - curPos;

        // Determine the normal
        Vector3 normLeftUp = Vector3.Cross(left, up);
        Vector3 normUpRight = Vector3.Cross(up, right);
        Vector3 normRightDown = Vector3.Cross(right, down);
        Vector3 normDownLeft = Vector3.Cross(down, left);

        // Determine the average normal
        Vector3 normal = (normLeftUp + normUpRight + normRightDown + normDownLeft);

        // Add it to the list
        norm.Add(normal);
    }

    // Generate noise offsets for the landscape and biomes
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

    // Generate the mesh based on vertices, triangles, uvs, and normals
    public void SetMesh(bool isFlatShaded)
    {
        if (mesh == null)
            mesh = new Mesh();
        else
            mesh.Clear();

        if (isFlatShaded)
        {
            List<Vector3> flatVert = new List<Vector3>();
            List<Vector2> flatUV = new List<Vector2>();
            List<int> flatTri = new List<int>();

            for (int i = 0; i < tri.Count; i++)
            {
                flatVert.Add(vert[tri[i]]);
                flatUV.Add(uv[tri[i]]);
                flatTri.Add(i);
            }


            // Set mesh
            mesh.vertices = flatVert.ToArray();
            mesh.triangles = flatTri.ToArray();
            mesh.uv = flatUV.ToArray();
            //mesh.normals = norm.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        else
        {
            // Set mesh
            mesh.SetVertices(vert);
            mesh.SetTriangles(tri, 0);
            mesh.SetUVs(0, uv);
            mesh.SetNormals(norm);

        }

        // Destroy previous mesh (So it will not fill up the ram)
        Destroy(GetComponent<MeshFilter>().mesh);

        // Set the mesh to the mesh filter and mesh collider
        ;
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Determines the UV based on the biome
    Vector2 DetermineUV(float biome, Vector3 pos)
    {
        Vector2 uv;

        // Determine the row and collum in which the texture is situated
        float col = biome;
        float noise = Mathf.Clamp((Noise.PerlinNoise(new Vector2(pos.x, pos.z) + new Vector2(transform.position.x, transform.position.z), offset2D[0], 1, 0.5f, 0.0025f, 0, 1)), 0, 0.7f);
        uv = new Vector2(col / texturesPerLine, noise / texturesPerLine);

        return uv;
    }

    // Finds index of a vertex in the map 2d array, also gives the indices of the vertices that are only used for normal calculation if the vertex is on a boundary of a chunk
    public static int[] FindClosestVertices(Vector3 pos, Chunk chunk)
    {
        int[] vertices;
        TerrainController terrain = TerrainController.thisTerrainController;

        Vector3 localPos = pos - chunk.transform.position;

        float posX = localPos.x / terrain.tileSize + 1;
        float posZ = localPos.z / terrain.tileSize + 1;

        int vertX = Mathf.RoundToInt(posX);
        int vertZ = Mathf.RoundToInt(posZ);

        vertices = new int[2] { vertX, vertZ };

        return vertices;
    }
    
    // Finds the chunk where a vertex is on
    public static Chunk[] FindChunksWithVertex(Vector3 vertex)
    {
        TerrainController terrain = TerrainController.thisTerrainController;

        List<Chunk> chunks = new List<Chunk>();
        
        // X and Z position of the vertex in chunksize unit
        float posX = vertex.x / terrain.chunkSize;
        float posZ = vertex.z / terrain.chunkSize;

        // Floor this to an int to get the chunk row and collumn.
        int chunkX = Mathf.FloorToInt(posX);
        int chunkZ = Mathf.FloorToInt(posZ);

        int chunksInRow = terrain.length / terrain.chunkSize;
        int index = chunkZ + chunkX * chunksInRow;
        chunks.Add(terrain.chunks[index]);

        // This part finds vertices that excist in two different chunks i.e. at chunk borders
        Vector3 chunkPos = terrain.chunks[index].transform.position;

        // if vertex is on left border
        if (vertex.x - chunkPos.x < 1.5f * terrain.tileSize)
        { 
            chunks.Add(terrain.chunks[index - chunksInRow]);
        }
        // if vertex is on bottom border
        if (vertex.z - chunkPos.z < 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index - 1]);
        }
        // if vertex is on bottom left corner
        if (vertex.x - chunkPos.x < 1.5f * terrain.tileSize && vertex.z - chunkPos.z < 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index - 1 - chunksInRow]);
        }
        //if vertex is on right border
        if (vertex.x - chunkPos.x > terrain.chunkSize - 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index + chunksInRow]);
        }
        // if vertex is on top border
        if (vertex.z - chunkPos.z > terrain.chunkSize - 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index + 1]);
        }
        // if vertex is on top right corner
        if (vertex.x - chunkPos.x > terrain.chunkSize - 1.5f * terrain.tileSize && vertex.z - chunkPos.z > terrain.chunkSize - 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index + 1 + chunksInRow]);
        }
        // if vertex is on top left corner
        if (vertex.x - chunkPos.x < 1.5f * terrain.tileSize && vertex.z - chunkPos.z > terrain.chunkSize - 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index + 1 - chunksInRow]);
        }
        // if vertex is on bottom right corner
        if (vertex.x - chunkPos.x > terrain.chunkSize - 1.5f * terrain.tileSize && vertex.z - chunkPos.z < 1.5f * terrain.tileSize)
        {
            chunks.Add(terrain.chunks[index - 1 + chunksInRow]);
        }

        return chunks.ToArray();

    }

    public void AddTerrainObject(TerrainObject obj)
    {
        terrainObjects.Add(obj);
    }
}
