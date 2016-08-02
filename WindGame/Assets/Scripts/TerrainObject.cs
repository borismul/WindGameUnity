﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TerrainObject : MonoBehaviour {

    public int biome;
    public int objectNR;
    public List<List<Mesh>> newComponents = new List<List<Mesh>>();
    public int vertexMax = 10000;
    public int verticesNow;
    public bool isFull;
    public bool hasReloaded;

    List<Mesh> nonCombinedMesh = new List<Mesh>();
    Mesh currentMesh = new Mesh();
    Mesh result = new Mesh();

    List<CombineInstance> instances = new List<CombineInstance>();

    // Use this for initialization
    void Awake ()
    {
	    foreach(Material mat in GetComponent<MeshRenderer>().materials)
        {
            newComponents.Add(new List<Mesh>());
            nonCombinedMesh.Add(new Mesh());
            instances.Add(new CombineInstance());
        }
	}
	
    public void Reload()
    {
        result.Clear();
        int index2 = -1;
        List<CombineInstance> finalCombiner = new List<CombineInstance>();
        foreach (List<Mesh> component in newComponents)
        {
            int index = 0;
            index2++;
            List<CombineInstance> combiner = new List<CombineInstance>();
            foreach (Mesh mesh in component)
            {
                index++;
                CombineInstance instance = new CombineInstance();
                instance.mesh = mesh;
                instance.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                combiner.Add(instance);
            }
            nonCombinedMesh[index2].Clear();
            nonCombinedMesh[index2].CombineMeshes(combiner.ToArray(), true);
            CombineInstance instanceFinal = instances[index2];
            instanceFinal.mesh = nonCombinedMesh[index2];
            instanceFinal.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            finalCombiner.Add(instanceFinal);
        }
        result.CombineMeshes(finalCombiner.ToArray(), false);
        result.RecalculateNormals();
        Destroy(GetComponent<MeshFilter>().mesh);
        this.GetComponent<MeshFilter>().mesh = result;

        foreach (List<Mesh> destroyMeshList in newComponents)
        {
            foreach (Mesh destroyMesh in destroyMeshList)
            {
                Destroy(destroyMesh);
            }
        }
        newComponents.Clear();
        if (result.vertexCount > vertexMax)
        {
            isFull = true;
        }

    }

    public void RemoveMesh(Mesh[] mesh)
    {
        Mesh[] subMeshes = nonCombinedMesh.ToArray();
        CombineInstance[] combiner = new CombineInstance[subMeshes.Length];
        for (int k = 0; k < subMeshes.Length; k++)
        {
            // Vertices
            List<Vector3> currentVertices = subMeshes[k].vertices.ToList();
            List<Vector3> verticesToRemove = mesh[k].vertices.ToList<Vector3>();

            Vector3 firstVert = mesh[k].vertices[0];

            int minVertPos = 0;

            for (int i = 0; i < currentVertices.Count; i++)
            {
                if (Vector3.Distance(currentVertices[i], firstVert) < 0.001)
                {
                    minVertPos = i;
                    break;
                }
            }

            currentVertices.RemoveRange(minVertPos, verticesToRemove.Count);

            // Triangles
            List<int> currentTriangles = subMeshes[k].triangles.ToList();
            int firstTrianglePos = mesh[k].triangles[0] + minVertPos;
            bool hasFoundStart = false;
            int minTrianglePos = 0;
            int upperLimit = minVertPos + verticesToRemove.Count;

            for (int i = 0; i < currentTriangles.Count; i++)
            {
                int currentTrianglePos = currentTriangles[i];

                if (currentTrianglePos == firstTrianglePos && !hasFoundStart)
                {
                    hasFoundStart = true;
                    minTrianglePos = i;
                }

                if (currentTrianglePos >= upperLimit)
                    currentTriangles[i] = currentTrianglePos - verticesToRemove.Count;
            }

            currentTriangles.RemoveRange(minTrianglePos, mesh[k].triangles.Length);
            nonCombinedMesh[k].Clear();
            nonCombinedMesh[k].vertices = currentVertices.ToArray();
            nonCombinedMesh[k].triangles = currentTriangles.ToArray();

            combiner[k].mesh = nonCombinedMesh[k];
            combiner[k].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        result.Clear();

        result.CombineMeshes(combiner, false);
        Destroy(GetComponent<MeshFilter>().mesh);
        GetComponent<MeshFilter>().mesh = result;
    }

    
}
