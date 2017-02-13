using UnityEngine;
using System.Collections;

public class ForestSound : MonoBehaviour
{

    public AudioClip[] forestSounds;
    GameObject soundObject;
    void Start()
    {
        soundObject = new GameObject();
        soundObject.AddComponent<AudioSource>();
        soundObject.GetComponent<AudioSource>().clip = forestSounds[0];
        soundObject.GetComponent<AudioSource>().loop = true;
        soundObject.GetComponent<AudioSource>().spatialBlend = 1;
        soundObject.GetComponent<AudioSource>().dopplerLevel = 0;
        soundObject.GetComponent<AudioSource>().minDistance = 15;

        soundObject.GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        GridTile closestForestTile = FindClosestForestTile();

        if (closestForestTile != null)
        {
            soundObject.transform.position = closestForestTile.position;
        }
    }

    GridTile FindClosestForestTile()
    {
        Vector3 pos = transform.position;

        GridTile[] tiles = GridTile.FindGridTilesAround(pos, 100);

        float curDist = float.MaxValue;
        GridTile curTile = null;
        for (int i = 0; i < tiles.Length; i++)
        {
            float dist = Vector3.Distance(tiles[i].position, pos);
            if (dist < curDist && tiles[i].biome == 0)
            {
                curDist = dist;
                curTile = tiles[i];
            }
        }

        return curTile;
    }
	
}
