using UnityEngine;
using System.Collections;

public class WindVaneController : MonoBehaviour {

    public float windRadius = 100;
    public GameObject flag;
	// Use this for initialization
	void Start ()
    {
	    foreach(GridTile tile in GridTile.FindGridTilesAround(transform.position, windRadius))
        {
            tile.canSeeWind = true;
        }
	}

    void Update()
    {
        flag.transform.rotation = Quaternion.Euler(-90, 90 -(180 - WindController.direction), 0);
    }
    
}
