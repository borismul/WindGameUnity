using UnityEngine;
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
    public List<int> numVerticesPerObject;
    public int animationNumber;
    public readonly object verticesLocker = new object();
    public bool isEmpty;
    Mesh thisMesh;
    AnimationParameters thisAnimationParameters;
    Vector3[] currentVertices;
    List<Mesh> nonCombinedMesh = new List<Mesh>();
    Mesh result;
    public bool isEnabled = true;

    List<CombineInstance> instances = new List<CombineInstance>();

    List<Mesh[]> removeList = new List<Mesh[]>();

    // Use this for initialization
    void Awake ()
    {
        result = new Mesh();

        for (int i = 0; i< GetComponent<MeshRenderer>().materials.Length; i++)
        {
            newComponents.Add(new List<Mesh>());
            nonCombinedMesh.Add(new Mesh());
            instances.Add(new CombineInstance());
        }
	}

    void Start()
    {
        thisMesh = GetComponent<MeshFilter>().mesh;
        thisAnimationParameters = GetComponent<AnimationParameters>();
        StartCoroutine("RemoveObject");
    }

    private void Update()
    {
        isEnabled = true;
    }

    public void Reload()
    {
        if (result != null)
        {
            result.Clear();
        }
        else
        {
            result = new Mesh();
        }
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
        if (result.vertexCount > vertexMax || !hasReloaded)
        {
            if (GetComponent<AnimationParameters>() != null)
                GetComponent<AnimationParameters>().DetermineAnimationParameters();

            isFull = true;
            hasReloaded = true;
            currentVertices = GetComponent<MeshFilter>().mesh.vertices;
        }
    }

    public void RemoveMesh(Mesh[] mesh)
    {
        removeList.Add(mesh);
    }

    void RemoveMeshCompletely(Mesh[] mesh)
    {
        Mesh[] subMeshes = nonCombinedMesh.ToArray();
        CombineInstance[] combiner = new CombineInstance[subMeshes.Length];
        for (int k = 0; k < subMeshes.Length; k++)
        {
            // Vertices
            List<Vector3> currentVertices = subMeshes[k].vertices.ToList();
            List<Vector3> verticesToRemove = mesh[k].vertices.ToList();

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
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    void MoveMesh(object args)
    { 
        List<Vector3[]> verticesToRemoveList = (List<Vector3[]>)args;
        lock (verticesLocker)
        {
            for (int k = 0; k < verticesToRemoveList.Count; k++)
            {
                // Vertices
                if (thisAnimationParameters != null)
                    currentVertices = TreeAnimationController.instance.meshVertices[animationNumber];

                Vector3[] verticesToRemove = verticesToRemoveList[k];
                Vector3 firstVert = verticesToRemove[0];

                int minVertPos = 0;

                for (int i = 0; i < currentVertices.Length; i++)
                {
                    if (Vector3.Distance(currentVertices[i], firstVert) < 0.001)
                    {
                        minVertPos = i;
                        break;
                    }
                }

                for (int i = 0; i < verticesToRemove.Length; i++)
                {
                    currentVertices[minVertPos + i] += new Vector3(0, 10000, 0);
                }

                if (thisAnimationParameters != null)
                {
                    TreeAnimationController.instance.meshVertices[animationNumber] = currentVertices;
                }
            }
        }
    }

    public IEnumerator RemoveObject()
    {
        while (true)
        {
            while (currentVertices == null)
            {
                yield return null;
            }

            if (removeList.Count > 0)
            {
                List<Vector3[]> verticesToRemove = new List<Vector3[]>();

                for (int j = 0; j < removeList[0].Length; j++)
                    verticesToRemove.Add(removeList[0][j].vertices);
                object args = verticesToRemove;

                MyThreadPool.AddActionToQueue(MoveMesh, args);
                removeList.RemoveAt(0);
            }

            lock (verticesLocker)
            {
                if (thisAnimationParameters == null && currentVertices != null)
                    GetComponent<MeshFilter>().mesh.vertices = currentVertices;
            }

            yield return null;
        }
    }
    
}
