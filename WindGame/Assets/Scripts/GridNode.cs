using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridNode
{
    public Vector3 position;
    public bool heighIsSet;
    public int uvPos;

    public GridNode(Vector3 position, int uvPos)
    {
        this.position = position;
        heighIsSet = false;
        this.uvPos = uvPos;
    }

    public static GridNode FindGridNode(Vector3 position)
    {
        GridNode[,] gridNodes = TerrainController.thisTerrainController.worldNodes;
        float tileSize = TerrainController.thisTerrainController.tileSize;

        int x = Mathf.RoundToInt(position.x / tileSize);
        int z = Mathf.RoundToInt(position.z / tileSize);

        if (gridNodes.GetLength(0) <= x || gridNodes.GetLength(1) <= z)
            return null;

        return gridNodes[x, z];
    }

}
