using UnityEngine;
using System.Collections;

public class WindVaneController : MonoBehaviour {

    public float windRadius = 100;

	// Use this for initialization
	void Start ()
    {
	    foreach(GridTile tile in GridTile.FindGridTilesAround(transform.position, windRadius))
        {
            tile.canSeeWind = true;
        }
	}
}
