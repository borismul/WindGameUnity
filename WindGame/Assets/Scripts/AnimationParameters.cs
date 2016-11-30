using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rand = System.Random;
using System.Threading;

public class AnimationParameters : MonoBehaviour {

    TerrainObject terrainObject;
    public Vector3[] vertices;
    
    int totalObjects;
    Mesh objectMesh;
    Rand rand;

    void Awake()
    {
        rand = new Rand(Random.Range(int.MinValue,  int.MaxValue));
        terrainObject = GetComponent<TerrainObject>();
    }

    public void DetermineAnimationParameters()
    {
        terrainObject = GetComponent<TerrainObject>();
        objectMesh = GetComponent<MeshFilter>().mesh;
        TreeAnimationController treeAnimationController = TreeAnimationController.instance;
        vertices = objectMesh.vertices;
        terrainObject.animationNumber = treeAnimationController.meshVertices.Count;
        treeAnimationController.meshVertices.Add(objectMesh.vertices);

        List<float> randomValues = new List<float>();

        for (int i = 0; i < vertices.Length; i++)
        {
            randomValues.Add((float)rand.NextDouble());
        }

        treeAnimationController.randomValues.Add(randomValues);

        int sum = 0;

        foreach(int submesh in terrainObject.numVerticesPerObject)
        {
            sum += submesh;
        }

        totalObjects = terrainObject.verticesNow / sum;
        List<List<int>> vertPosPerObject = new List<List<int>>();

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

        treeAnimationController.vertPosPerObject.Add(vertPosPerObject);
    }
}
