using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChunk : MonoBehaviour {

    public Renderer ren;
    public MeshFilter meshFilter;
    public bool isEnabled = true;
    public List<GridTile> tiles;

	// Use this for initialization
	void Awake () {
        ren = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
	}
	
}
