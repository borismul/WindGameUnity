using UnityEngine;
using System.Collections;

public class TerrainLoader : MonoBehaviour {

    public string levelName;

	// Use this for initialization
	void Awake ()
    {
        TerrainController.thisTerrainController.StartCoroutine(TerrainController.thisTerrainController.Load(levelName, true));
	}
}
