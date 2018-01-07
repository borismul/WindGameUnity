using UnityEngine;
using System.Collections;

public class GridTileOccupant
{
    public GameObject obj;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public TerrainObject terrainObject;
    public WindEffectController windEffectController;
    public OccupantType type;

    public GridTileOccupant(GameObject obj, Vector3 position, Quaternion rotation, Vector3 scale, OccupantType type)
    {
        this.obj = obj;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.terrainObject = obj.GetComponent<TerrainObject>();
        this.windEffectController = obj.GetComponent<WindEffectController>();
        this.type = type;
    }

    public GridTileOccupant(GameObject obj)
    {
        this.obj = obj;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        this.windEffectController = obj.GetComponent<WindEffectController>();
    }

    public GridTileOccupant(GameObject obj, OccupantType type)
    {
        this.obj = obj;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        this.windEffectController = obj.GetComponent<WindEffectController>();
        this.type = type;
    }

    public enum OccupantType { Empty, TerrainGenerated, Turbine, City, Other};

}

