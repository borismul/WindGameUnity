using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TerrainObject : MonoBehaviour {

    public int biome;
    public int objectNR;
    public List<List<Mesh>> newComponents = new List<List<Mesh>>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<List<int>> trianglesPerSubMesh = new List<List<int>>();
    public int vertexMax = 65000;
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
    bool updateCurrentVertices;
    List<Mesh> nonCombinedMesh = new List<Mesh>();
    Mesh result;
    public bool isEnabled = true;

    List<CombineInstance> instances = new List<CombineInstance>();

    List<MoveMeshObj> removeList = new List<MoveMeshObj>();
    int count;

    public Renderer ren;
    // Use this for initialization
    void Awake ()
    {
        result = new Mesh();

        for (int i = 0; i< GetComponent<MeshRenderer>().materials.Length; i++)
        {
            trianglesPerSubMesh.Add(new List<int>());
        }
	}

    void Start()
    {
        thisMesh = GetComponent<MeshFilter>().mesh;
        thisAnimationParameters = GetComponent<AnimationParameters>();
        ren = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        StartCoroutine("RemoveObject");

    }

    public void Reloadt()
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
        if (result.vertexCount > vertexMax || verticesNow > vertexMax || !hasReloaded)
        {
            if (GetComponent<AnimationParameters>() != null)
                GetComponent<AnimationParameters>().DetermineAnimationParameters();

            isFull = true;
            hasReloaded = true;
            currentVertices = GetComponent<MeshFilter>().mesh.vertices;
        }
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

        result.subMeshCount = trianglesPerSubMesh.Count;
        result.SetVertices(vertices);

        for (int i = 0; i < trianglesPerSubMesh.Count; i++)
        {
            result.SetTriangles(trianglesPerSubMesh[i], i);
        }
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
        if (result.vertexCount > vertexMax || verticesNow > vertexMax || !hasReloaded)
        {
            if (GetComponent<AnimationParameters>() != null)
                GetComponent<AnimationParameters>().DetermineAnimationParameters();

            isFull = true;
            hasReloaded = true;
            currentVertices = GetComponent<MeshFilter>().mesh.vertices;
        }
    }

    public void AddTriangles (int[] triangles, int submesh)
    {
        int startTri = vertices.Count;
        for(int i = 0; i < triangles.Length; i++)
        {
            trianglesPerSubMesh[submesh].Add(triangles[i] + startTri);
        }
    }

    public void AddVertices(List<Vector3> vertices)
    {
        this.vertices.AddRange(vertices);
    }

    public void RemoveMesh(MoveMeshObj moveMesh)
    {
        removeList.Add(moveMesh);
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
        MoveMeshObj removeObj = (MoveMeshObj)args;

            
            // Vertices
            if (thisAnimationParameters != null)
                currentVertices = TreeAnimationController.instance.meshVertices[animationNumber];

            Vector3 firstVert = removeObj.startVert;

            int minVertPos = 0;

            for (int i = 0; i < currentVertices.Length; i++)
            {
                if (Vector3.Distance(currentVertices[i], firstVert) < 0.01f)
                {
                    minVertPos = i;
                    break;
                }
            }


            for (int i = 0; i < removeObj.totalVert; i++)
            {
                lock(currentVertices)
                    currentVertices[minVertPos + i] += new Vector3(0, 10000, 0);
            }

            if (thisAnimationParameters != null)
            {
            
                TreeAnimationController.instance.meshVertices[animationNumber] = currentVertices;
            }
            
        

        updateCurrentVertices = true;
    }

    public IEnumerator RemoveObject()
    {
        List<Task> tasks = new List<Task>();
        count = 0;
        while (true)
        {
            while (currentVertices == null || removeList.Count == 0)
            {
                yield return null;
            }

            while (removeList.Count > 0)
            {


                object args = removeList[0];

                tasks.Add(MyThreadPool.AddActionToQueue(MoveMesh, args, TaskPriority.medium));
                removeList.RemoveAt(0);
            }

            for(int i = 0; i < tasks.Count;i++)
            {
                while (!tasks[i].isDone)
                    yield return null;
            }

            if (thisAnimationParameters == null && currentVertices != null && updateCurrentVertices)
            {
                GetComponent<MeshFilter>().mesh.vertices = currentVertices;
                updateCurrentVertices = false;
            }
            

            if (count++ > 1)
            {
                yield return null;
                count = 0;
            }
        }
    }
    
}


public struct MoveMeshObj
{
    public Vector3 startVert;
    public int totalVert;
    public MoveMeshObj(Vector3 startVert, int totalVert)
    {
        this.startVert = startVert;
        this.totalVert = totalVert;
    }
}
