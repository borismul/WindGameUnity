using UnityEngine;
using System.Collections;

public class GridTileOccupant
{
    public GameObject obj;
    public Quaternion rotation;
    public Vector3 scale;
    public TerrainObject terrainObject;
    public WindEffectController windEffectController;

	public GridTileOccupant(GameObject obj, Quaternion rotation, Vector3 scale)
    {
        this.obj = obj;
        this.rotation = rotation;
        this.scale = scale;
        this.terrainObject = obj.GetComponent<TerrainObject>();
        this.windEffectController = obj.GetComponent<WindEffectController>();
    }

    public GridTileOccupant(GameObject obj)
    {
        this.obj = obj;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        this.windEffectController = obj.GetComponent<WindEffectController>();
    }

    public enum OccupantType { Empty, TerrainGenerated, Turbine, City, Other};

}

