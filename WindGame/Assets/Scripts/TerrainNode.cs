using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainNode {

    Vector3 position;
    public int used;
    public List<TerrainNode> neighbours;

    public TerrainNode(Vector3 position)
    {
        this.position = position;
        this.used = 0;
        neighbours = new List<TerrainNode>();

    }
}
