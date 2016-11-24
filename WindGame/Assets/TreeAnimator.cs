using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rand = System.Random;

public class TreeAnimator : MonoBehaviour {

    TerrainObject terrainObject;
    bool wasFull = false;
    List<List<int>> vertPosPerObject;
    List<float> lowestVertPerObject;
    public Vector3[] vertices;
    List<float> randomValues;
    int totalObjects;
    Mesh objectMesh;
    Rand rand;

    public float shakeSpeed = 0.6f;
    void Start()
    {
        rand = new Rand();
        terrainObject = GetComponent<TerrainObject>();
    }

	void Update()
    {
        if (!terrainObject.isFull)
            return;

        if (!wasFull)
            DetermineStartVertex();

        Animate();
    }

    void Animate()
    {
        Vector3[] animationVert = objectMesh.vertices;
        Vector3 windVector = -Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * WindController.direction), 0, Mathf.Cos(Mathf.Deg2Rad * WindController.direction)));
        for (int i = 0; i < vertPosPerObject.Count; i++)
        { 
            Vector3 move =  windVector * shakeSpeed * (1 + Mathf.Sin(Time.realtimeSinceStartup * 1f + 2*randomValues[i]));

            for (int j = 0; j < vertPosPerObject[i].Count; j++)
            {
                float height = vertices[vertPosPerObject[i][j]].y - lowestVertPerObject[i];
                animationVert[vertPosPerObject[i][j]] = vertices[vertPosPerObject[i][j]];
                animationVert[vertPosPerObject[i][j]] += Mathf.Sqrt(height) * move;
            }
        }

        objectMesh.vertices = animationVert;
        GetComponent<MeshFilter>().mesh = objectMesh;
    }

    void DetermineStartVertex()
    {
        objectMesh = GetComponent<MeshFilter>().mesh;
        vertices = objectMesh.vertices;

        randomValues = new List<float>();

        for (int i = 0; i < vertices.Length; i++)
        {
            randomValues.Add((float)rand.NextDouble());
        }

        int sum = 0;

        foreach(int submesh in terrainObject.numVerticesPerObject)
        {
            sum += submesh;
        }

        totalObjects = terrainObject.verticesNow / sum;
        vertPosPerObject = new List<List<int>>();

        for (int i = 0; i < totalObjects; i++)
        {
            List<int> objectVertPos = new List<int>();
            for (int j = 0; j < terrainObject.numVerticesPerObject.Count; j++)
            {
                int add = 0;
                for (int l = 0; l < j; l++)
                {
                    int a = terrainObject.numVerticesPerObject[l];
                    add += totalObjects * a;
                }
                for (int k = 0; k < terrainObject.numVerticesPerObject[j]; k++)
                {
                    int vertPos = k + add + i * terrainObject.numVerticesPerObject[j];
                    objectVertPos.Add(k + add + i * terrainObject.numVerticesPerObject[j]);
                }
            }
            vertPosPerObject.Add(objectVertPos);
        }

        wasFull = true;
        lowestVertPerObject = new List<float>();

        for (int i = 0; i < totalObjects; i++)
        {
            float lowest = Mathf.Infinity;
            foreach (int vertex in vertPosPerObject[i])
            {
                if (objectMesh.vertices[vertex].y < lowest)
                    lowest = objectMesh.vertices[vertex].y;
            }
            
            lowestVertPerObject.Add(lowest);
        }
    }
}
