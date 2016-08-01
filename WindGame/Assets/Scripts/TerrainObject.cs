using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainObject : MonoBehaviour {

    public int biome;
    public int objectNR;
    public List<List<Mesh>> newComponents = new List<List<Mesh>>();
    public int vertexMax = 10000;
    public int verticesNow;
    public bool isFull;
    public bool hasReloaded;
	// Use this for initialization
	void Awake ()
    {
	    foreach(Material mat in GetComponent<MeshRenderer>().materials)
        {
            newComponents.Add(new List<Mesh>());
        }
	}
	
    public void Reload()
    {
        Mesh result = new Mesh();
        List<CombineInstance> finalCombiner = new List<CombineInstance>();
        foreach (List<Mesh> component in newComponents)
        {
            int index = 0;

            List<CombineInstance> combiner = new List<CombineInstance>();
            foreach (Mesh mesh in component)
            {
                index++;
                CombineInstance instance = new CombineInstance();
                instance.mesh = mesh;
                instance.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                combiner.Add(instance);
            }

            Mesh currentMesh = new Mesh();
            currentMesh.CombineMeshes(combiner.ToArray(), true);

            CombineInstance instanceFinal = new CombineInstance();
            instanceFinal.mesh = currentMesh;
            instanceFinal.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            finalCombiner.Add(instanceFinal);
        }

        result.CombineMeshes(finalCombiner.ToArray(), false);
        this.GetComponent<MeshFilter>().mesh = result;

        newComponents = new List<List<Mesh>>();
        if(result.vertexCount > vertexMax)
        {
            isFull = true;
        }

    }

    void RemoveMesh(Mesh mesh)
    {
        Vector3 firstVert = mesh.vertices[0];
    }
}
