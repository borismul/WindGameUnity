using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class WindVisualizer : MonoBehaviour
{
    static bool isBusy;

    public List<ThreadVector2>[] uvs;
    List<List<ThreadVector2>> threadUvs = new List<List<ThreadVector2>>();
    List<List<ThreadVector3>> threadVerts = new List<List<ThreadVector3>>();
    TerrainController terrain;

    List<GameObject> windSpeedChunks = new List<GameObject>();
    public static WindVisualizer instance;
    Thread newThread;
    Thread[] chunkThreads = new Thread[8];

    float absMax;

    public float height = 0;

    void Start()
    {
        instance = this;
    }

    public void VisualizeWind()
    {
        terrain = TerrainController.thisTerrainController;
        StartCoroutine("_VisualizeWind");
    }

    public void StopVisualizeWind()
    {
        StopCoroutine("_VisualizeWind");

        foreach (GameObject windSpeedChunk in windSpeedChunks)
            Destroy(windSpeedChunk);

        foreach (Thread thread in chunkThreads)
        {
            if (thread != null)
                thread.Abort();
        }
        if (newThread != null)
            newThread.Abort();

        windSpeedChunks.Clear();
    }

    IEnumerator _VisualizeWind()
    {
        CreateChunks();

        while (true)
        {
            isBusy = true;
            newThread = new Thread(() => GetWindUVs());
            newThread.Start();

            while (isBusy)
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

        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            GameObject chunk = terrain.chunks[i].gameObject;
            GameObject newWindChunk = new GameObject("WindChunk");
            newWindChunk.transform.position = chunk.transform.position + Vector3.up * 0.5f;
            newWindChunk.AddComponent<MeshFilter>().mesh = chunk.GetComponent<MeshFilter>().mesh;
            newWindChunk.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            newWindChunk.GetComponent<MeshRenderer>().material.mainTexture = windTexture;
            threadUvs.Add(ThreadVector2.ToThreadVectorList(newWindChunk.GetComponent<MeshFilter>().mesh.uv));
            Vector3[] meshverts = newWindChunk.GetComponent<MeshFilter>().mesh.vertices;
            List<ThreadVector3> threadVectors = new List<ThreadVector3>();
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
        uvs = new List<ThreadVector2>[terrain.chunks.Count];
        absMax = 0;
        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            bool ranThread = false;
            while (!ranThread)
            {
                bool canRunNewThread = false;
                int pos = 0;
                for (int j = 0; j < chunkThreads.Length; j++)
                {
                    if (chunkThreads[j] == null || !chunkThreads[j].IsAlive)
                    {
                        canRunNewThread = true;
                        pos = j;
                        break;
                    }
                }
                if (canRunNewThread)
                {
                    System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(CalculateChunk);
                    chunkThreads[pos] = new System.Threading.Thread(pts);
                    chunkThreads[pos].Start(i);
                    ranThread = true;
                }
            }
        }

        bool canContinue = false;

        while (!canContinue)
        {
            for (int j = 0; j < chunkThreads.Length; j++)
            {
                if (chunkThreads[j] != null && chunkThreads[j].IsAlive)
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

        isBusy = false;
    }

    void CalculateChunk(object i)
    {
        List<ThreadVector2> uvsOfThisChunk = threadUvs[(int)i];
        List<ThreadVector3> vert = threadVerts[(int)i];
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
        uvs[(int)i] = CurChunkUVs;
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

    private static List<T> CreateList<T>(int capacity)
    {
        return Enumerable.Repeat(default(T), capacity).ToList();
    }
}
