using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

    public GameObject chunkPrefab;

    [Header("Terrain Details")]
    public int length;
    public int width;
    public int seed;
    public int maxHeight;
    public int stretch;

    [Header("Chunk Details")]
    public int chunkSize;
    public int cubeSize;
    public int cubeHeight;

    [Header("Grid Details")]
    public GameObject gridPrefab;

    [Header("Terrain Extra's")]
    public Mesh tree;
    public Mesh treeTop;

    public static int statChunkSize;
    public static int statMaxHeight;
    public static int statCubeSize;
    public static int statStretch;
    public static int statSeed;
    public static int statwidth;
    public static int statLength;
    public static float statCubeHeight;
    public static Mesh statTreeMesh;
    public static Mesh statTreeTopMesh;


    public static bool levelLoaded;
    bool instGrid;

    public static Grid grid;

    public static List<GridTile> world;

	// Use this for initialization
	void Start ()
    {
        world = new List<GridTile>();

        if (seed == 0)
            statSeed = Random.Range(0, int.MaxValue);
        else
            statSeed = seed;
        statChunkSize = chunkSize;
        statCubeSize = cubeSize;
        statCubeHeight = cubeHeight;
        statStretch = stretch;
        statwidth = width;
        statLength = length;
        statMaxHeight = maxHeight * (cubeSize / cubeHeight);
        statTreeMesh = tree;
        statTreeTopMesh = treeTop;


        StartCoroutine(BuildTerrain());
       
	}

    void Update()
    {
        if (levelLoaded && !instGrid)
        {
            grid = Instantiate(gridPrefab).GetComponent<Grid>();
            instGrid = true;
        }
    }

    IEnumerator BuildTerrain()
    {
        for (int i = 0; i < length / chunkSize; i++)
        {
            for (int j = 0; j < width / chunkSize; j++)
            {
                Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity);
            }
            yield return null;

        }
        levelLoaded = true;
    }
	
}
