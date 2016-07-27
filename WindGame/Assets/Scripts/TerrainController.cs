using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {

    public GameObject chunkPrefab;

    public int length;
    public int width;
    public int setSeed;

    public static int chunkSize = 30;
    public static int maxHeight = 200;
    public static int cubeSize = 3;
    public static int stretch = 50;
    public static int seed;


	// Use this for initialization
	void Start ()
    {
        if (setSeed == 0)
            seed = Random.Range(0, int.MaxValue);
        else
            seed = setSeed;
        StartCoroutine(BuildTerrain());
	}

    IEnumerator BuildTerrain()
    {
        for (int i = 0; i < length / chunkSize; i++)
        {
            for (int j = 0; j < width / chunkSize; j++)
            {
                Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity);
                yield return null;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
