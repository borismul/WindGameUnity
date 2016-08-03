using UnityEngine;
using System.Collections;

public class GridTileOccupant
{

    public GameObject obj;
    public Quaternion rotation;
    public Vector3 scale;

	public GridTileOccupant(GameObject obj, Quaternion rotation, Vector3 scale)
    {
        this.obj = obj;
        this.rotation = rotation;
        this.scale = scale;
    }

    public GridTileOccupant(GameObject obj)
    {
        this.obj = obj;
        rotation = Quaternion.identity;
        scale = Vector3.one;
    }
}
