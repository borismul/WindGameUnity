using UnityEngine;
using System.Collections;

public class TerrainLoader : MonoBehaviour {

    public string levelName;

	// Use this for initialization
	void Start ()
    {
        TerrainController.thisTerrainController.StartCoroutine(TerrainController.thisTerrainController.Load(levelName, true));
	}
	
}
