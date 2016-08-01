using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BiomeMesh
{
    public List<Mesh[]> mesh;
    public List<float> occurance;

    public BiomeMesh()
    {
        this.mesh = new List<Mesh[]>();
        this.occurance = new List<float>();
    }

    public void AddMesh(Mesh[] mesh, float occurance)
    {
        this.mesh.Add(mesh);
        this.occurance.Add(occurance);
    }
}
