using UnityEngine;
using System.Collections;

public class GridTile {

    public Vector3 position;
    public int type;
    public bool canBuild;
    public GameObject occupant;
    public GameObject gridPointDebug;

	public GridTile(Vector3 position, int type, bool canBuild, GameObject occupant, GameObject gridPointDebug)
    {
        this.position = position;
        this.type = type;
        this.canBuild = canBuild;
        this.occupant = occupant;
        this.gridPointDebug = gridPointDebug;
    }

    public GridTile(Vector3 position, int type, bool canBuild, GameObject occupant)
    {
        this.position = position;
        this.type = type;
        this.canBuild = canBuild;
        this.occupant = occupant;
    }
}
