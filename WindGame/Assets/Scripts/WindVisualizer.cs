using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class WindVisualizer : MonoBehaviour
{
    public List<ThreadVector2>[] uvs;
    List<List<ThreadVector2>> threadUvs = new List<List<ThreadVector2>>();
    List<List<ThreadVector3>> threadVerts = new List<List<ThreadVector3>>();
    TerrainController terrain;

    List<GameObject> windSpeedChunks = new List<GameObject>();
    public static WindVisualizer instance;
    Thread newThread;
    Thread[] chunkThreads = new Thread[50];

    float absMax;

    public float height = 0;

    //float averageTime;
    //int timesUSed = 0;

    void Start()
    {
        instance = this;
    }

    public void VisualizeWind()
    {
        terrain = TerrainController.thisTerrainController;
        StartCoroutine("_VisualizeWind");

        foreach (Thread thread in chunkThreads)
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }

    public void StopVisualizeWind()
    {
        StopCoroutine("_VisualizeWind");

        foreach (GameObject windSpeedChunk in windSpeedChunks)
        {
            if (windSpeedChunk != null)
            {
                Destroy(windSpeedChunk.GetComponent<MeshFilter>().mesh);
                Destroy(windSpeedChunk);
            }
        }
        windSpeedChunks.Clear();
        threadUvs.Clear();
        threadVerts.Clear();

        if (newThread != null && newThread.IsAlive)
            newThread.Abort();

        windSpeedChunks.Clear();
    }

    IEnumerator _VisualizeWind()
    {
        CreateChunks();

        while (true)
        {
            if (newThread == null || !newThread.IsAlive)
            {
                newThread = new Thread(() => GetWindUVs());
                newThread.Start();
            }

            while (newThread.IsAlive)
            {
                yield return null;
            }

            for(int i = 0; i<windSpeedChunks.Count; i++)
            {
                windSpeedChunks[i].GetComponent<MeshFilter>().mesh.uv = ThreadVector2.ToVectorArray(uvs[i]);
            }
        }
    }

    void CreateChunks()
    {
        Texture windTexture = (Texture)Resources.Load("windTexture");
        List<ThreadVector3> threadVectors = new List<ThreadVector3>();

        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            GameObject chunk = terrain.chunks[i].gameObject;
            GameObject newWindChunk = new GameObject("WindChunk");
            newWindChunk.transform.position = chunk.transform.position + Vector3.up * 0.5f;
            newWindChunk.AddComponent<MeshFilter>().mesh = chunk.GetComponent<MeshFilter>().mesh;
            Mesh mesh = newWindChunk.GetComponent<MeshFilter>().mesh;

            newWindChunk.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            newWindChunk.GetComponent<MeshRenderer>().material.mainTexture = windTexture;
            threadUvs.Add(ThreadVector2.ToThreadVectorList(newWindChunk.GetComponent<MeshFilter>().mesh.uv));
            Vector3[] meshverts = newWindChunk.GetComponent<MeshFilter>().mesh.vertices;
            threadVectors = new List<ThreadVector3>();

            for (int j = 0; j < meshverts.Length; j++)
            {
                threadVectors.Add(new ThreadVector3(meshverts[j] + chunk.transform.position));
            }
            threadVerts.Add(threadVectors);
            windSpeedChunks.Add(newWindChunk);
        }
    }

    void GetWindUVs()
    {
        int timeAtStart = System.DateTime.Now.Millisecond;
        uvs = new List<ThreadVector2>[terrain.chunks.Count];
        absMax = 0;

        for (int i = 0; i<chunkThreads.Length; i++)
        {
            System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(CalculateChunk);
            chunkThreads[i] = new System.Threading.Thread(pts);
            chunkThreads[i].Start(i);       
        }

        bool canContinue = false;

        while (!canContinue)
        {
            for (int j = 0; j < chunkThreads.Length; j++)
            {
                if (chunkThreads[j].IsAlive)
                {
                    canContinue = false;
                    break;
                }
                canContinue = true;
            }
        }


        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            List<ThreadVector3> vert = threadVerts[i];
            for (int j = 0; j < uvs[i].Count; j++)
            {
                GridTile tile = GridTile.FindClosestGridTile(vert[j]);
                if (tile == null || tile.isOutsideBorder)
                    continue;
                else
                {
                    uvs[i][j] = new ThreadVector2(1 - uvs[i][j].x / absMax, 1 - uvs[i][j].y / absMax);
                }
            }
        }

        //if (System.DateTime.Now.Millisecond - timeAtStart >= 0)
        //{
        //    timesUSed++;
        //    averageTime = (averageTime * (timesUSed-1) + System.DateTime.Now.Millisecond - timeAtStart) / timesUSed;
        //}
        //print(averageTime);

    }

    void CalculateChunk(object i)
    {
        int operationsPerThread = Mathf.CeilToInt(windSpeedChunks.Count/chunkThreads.Length);

        for (int k = (int)i * operationsPerThread; k < ((int)i + 1) * operationsPerThread; k++)
        {
            if (windSpeedChunks[k] == null)
                break;

            List<ThreadVector2> uvsOfThisChunk = threadUvs[(int)k];
            List<ThreadVector3> vert = threadVerts[(int)k];
            List<ThreadVector2> CurChunkUVs = new List<ThreadVector2>();
            for (int j = 0; j < vert.Count; j++)
            {
                GridTile tile = GridTile.FindClosestGridTile(vert[j]);
                if (tile == null || tile.isOutsideBorder)
                    CurChunkUVs.Add(new ThreadVector2(0, 1));
                else
                {
                    float curWind = WindController.GetWindAtTile(tile, height);
                    if (curWind > absMax)
                        absMax = curWind;
                    CurChunkUVs.Add(new ThreadVector2(curWind, curWind));
                }
            }

            uvs[(int)k] = CurChunkUVs;
        }
    }

    void OnApplicationQuit()
    {
        foreach(Thread thread in chunkThreads)
        {
            if(thread != null)
                thread.Abort();
        }
        if(newThread!= null)
            newThread.Abort();
        
    }
}
