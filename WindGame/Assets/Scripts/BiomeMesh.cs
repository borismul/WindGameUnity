using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BiomeMesh
{
    public int objectsPerTile;
    public List<Mesh[]> mesh;
    public List<float> occurance;
    public List<float> minScale;
    public List<float> maxScale;
    public List<float> minRot;
    public List<float> maxRot;

    public BiomeMesh()
    {
        this.mesh = new List<Mesh[]>();
        this.occurance = new List<float>();
        this.minScale = new List<float>();
        this.maxScale = new List<float>();
        this.minRot = new List<float>();
        this.maxRot = new List<float>();

    }

    public void AddMesh(Mesh[] mesh, float occurance, int objectsPerTile, float minScale, float maxScale, float minRot, float maxRot)
    {
        this.mesh.Add(mesh);
        this.occurance.Add(occurance);
        this.minScale.Add(minScale);
        this.maxScale.Add(maxScale);
        this.minRot.Add(minRot);
        this.maxRot.Add(maxRot);
        this.objectsPerTile = objectsPerTile;

    }
}
