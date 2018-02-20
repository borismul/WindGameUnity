using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class WindVisualizer : MonoBehaviour
{
    public Vector2[][][] uvs;
    public Vector2[][][] buffereduvs;

    List<List<Vector2>> threadUvs = new List<List<Vector2>>();
    List<List<Vector3>> threadVerts = new List<List<Vector3>>();
    TerrainController terrain;

    public List<WindChunk> windSpeedChunks = new List<WindChunk>();
    public List<bool> windSpeedChunkActive = new List<bool>();
    public static WindVisualizer instance;
    Thread newThread;
    Thread[] chunkThreads = new Thread[20];
    Task[] tasks;
    bool curUvs = false;
    int intpSteps = 50;

    readonly static object lockUVS = new object();

    public float height = 0;

    float calcTime;
    float startTime;

    bool isFirst;
    bool doneFirst;
    bool stopThreads = false;

    int curCount = 0;

    //float averageTime;
    //int timesUSed = 0;

    void Start()
    {
        instance = this;
    }

    public void VisualizeWind()
    {
        terrain = TerrainController.thisTerrainController;
        if (uvs == null)
        {
            uvs = new Vector2[terrain.chunks.Count][][];
            buffereduvs = new Vector2[terrain.chunks.Count][][];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2[intpSteps][];
                buffereduvs[i] = new Vector2[intpSteps][];
            }
        }
        StartCoroutine("_VisualizeWind");

    }

    public void StopVisualizeWind()
    {
        TreeAnimationController.instance.runAnimation = true;

        if (newThread != null && newThread.IsAlive)
            newThread.Abort();

        StopCoroutine("_VisualizeWind");
        
        foreach (WindChunk windSpeedChunk in windSpeedChunks)
        {
            if (windSpeedChunk != null)
            {
                Destroy(windSpeedChunk.meshFilter.mesh);
                Destroy(windSpeedChunk.gameObject);
            }
        }
        windSpeedChunks.Clear();
        threadUvs.Clear();
        threadVerts.Clear();

        terrain.windChunks.Clear();
        windSpeedChunks.Clear();
        windSpeedChunkActive.Clear();

        newThread = null;
    }

    IEnumerator _VisualizeWind()
    {
        TreeAnimationController.instance.runAnimation = false;
        CreateChunks(true);
        tasks = new Task[chunkThreads.Length];
        int curCount = 0;
        startTime = Time.realtimeSinceStartup;
        isFirst = true;
        curUvs = true;
        doneFirst = false;


        while (true)
        {
            yield return null;

            bool alldone = true;

            for (int i = 0; i < chunkThreads.Length; i++)
            {
                if (tasks[i] != null && !tasks[i].isDone)
                {
                    alldone = false;
                }
            }

            if (alldone)
            {
                curUvs = !curUvs;
                curCount = 0;

                if (isFirst)
                    curCount = intpSteps - 1;
                if (doneFirst)
                    isFirst = false;
                calcTime = Time.realtimeSinceStartup - startTime;
                startTime = Time.realtimeSinceStartup;
            }


            for (int i = 0; i < chunkThreads.Length; i++)
            {

                if (tasks[i] == null || tasks[i].isDone && (alldone))
                {
                    object[] args = new object[2];
                    args[0] = i;
                    args[1] = WindController.time;
                    object argsIn = args;
                    tasks[i] = MyThreadPool.AddActionToQueue(CalculateChunk, argsIn);
                }

            }

            for (int i = 0; i < windSpeedChunkActive.Count; i++)
                windSpeedChunkActive[i] = windSpeedChunks[i].gameObject.activeSelf;


            if ((Time.realtimeSinceStartup - startTime) - calcTime * ((float)(curCount + 1) / intpSteps) >= 0)
            {
                while ((Time.realtimeSinceStartup - startTime) - calcTime * ((float)(curCount + 1) / intpSteps) >= 0)
                    curCount++;

                curCount--;

                if (curCount > intpSteps - 1)
                    curCount = intpSteps - 1;

                for (int i = 0; i < windSpeedChunks.Count; i++)
                {
                    if (uvs[i] == null)
                        continue;

                    if (!isFirst)
                    {
                        if (curUvs)
                            windSpeedChunks[i].meshFilter.mesh.uv = buffereduvs[i][curCount];
                        else
                            windSpeedChunks[i].meshFilter.mesh.uv = uvs[i][curCount];
                    }
                    else
                    {
                        windSpeedChunks[i].GetComponent<MeshRenderer>().enabled = true;
                        windSpeedChunks[i].meshFilter.mesh.uv = buffereduvs[i][uvs[i].Length - 1];
                        doneFirst = true;

                    }

                }
                curCount++;

                if (curCount > intpSteps - 1)
                    curCount = intpSteps - 1;
            }
        }
    }

    public void Redo()
    {
        stopThreads = true;
        for(int i = 0; i < tasks.Length; i++)
        {
            while (!tasks[i].isDone)
            {
            }
        }
        stopThreads = false;
        tasks = new Task[chunkThreads.Length];
        curCount = 0;
        startTime = Time.realtimeSinceStartup;
        isFirst = true;
        curUvs = true;
        doneFirst = false;

        CreateChunks(false);
    }

    void CreateChunks(bool instantiate)
    {
        Texture windTexture = (Texture)Resources.Load("windTexture");
        List<Vector3> threadVectors = new List<Vector3>();

        Vector2[] uvsStart = new Vector2[terrain.chunks[0].GetComponent<MeshFilter>().mesh.uv.Length];

        for (int i = 0; i < uvsStart.Length; i++)
        {
            uvsStart[i] = new Vector2(0f, 1f);
        }

        if (!instantiate)
        {
            for (int j = 0; j < windSpeedChunks.Count; j++)
            {
                windSpeedChunks[j].GetComponent<MeshFilter>().mesh.uv = uvsStart;
                buffereduvs[j][uvs[j].Length - 1] = windSpeedChunks[j].GetComponent<MeshFilter>().mesh.uv;

            }
            return;
        }
            

        for (int i = 0; i < terrain.chunks.Count; i++)
        {
            GameObject chunk = terrain.chunks[i].gameObject;
            GameObject newWindChunkObj = new GameObject("WindChunk");
            newWindChunkObj.transform.position = chunk.transform.position + Vector3.up * 0.5f;

            newWindChunkObj.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Texture"));

            newWindChunkObj.GetComponent<MeshRenderer>().material.mainTexture = windTexture;
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;

            newWindChunkObj.AddComponent<MeshFilter>().mesh = mesh;
            newWindChunkObj.GetComponent<MeshFilter>().mesh.uv = uvsStart;
            threadUvs.Add(newWindChunkObj.GetComponent<MeshFilter>().mesh.uv.ToList());
            Vector3[] meshverts = newWindChunkObj.GetComponent<MeshFilter>().mesh.vertices;
            threadVectors = new List<Vector3>();
            WindChunk windChunk = newWindChunkObj.AddComponent<WindChunk>();
            for (int j = 0; j < meshverts.Length; j++)
            {
                threadVectors.Add(meshverts[j] + chunk.transform.position);
            }
            threadVerts.Add(threadVectors);
            windSpeedChunks.Add(windChunk);
            windSpeedChunkActive.Add(true);

            for (int j = 0; j < uvs[0].Length; j++)
            {
                uvs[i][j] = windChunk.meshFilter.mesh.uv;
                buffereduvs[i][j] = windChunk.meshFilter.mesh.uv;

            }

        }

        terrain.windChunks = windSpeedChunks;

    }

    public void UpdateChunks()
    {
        for(int i = 0; i < windSpeedChunks.Count; i++)
        {
            Mesh mesh = windSpeedChunks[i].GetComponent<MeshFilter>().mesh;
            mesh.vertices = terrain.chunks[i].GetComponent<MeshFilter>().mesh.vertices;
            mesh.normals = terrain.chunks[i].GetComponent<MeshFilter>().mesh.normals;
        }
    }

    void CalculateChunk(object args)
    {
        object[] argsArr = (object[])args;
        int i = (int)argsArr[0];
        float time = (float)argsArr[1];

        int operationsPerThread = Mathf.CeilToInt(windSpeedChunks.Count / chunkThreads.Length);
        int windCount = windSpeedChunks.Count;

        int startK = (int)i * operationsPerThread;
        int endK = ((int)i + 1) * operationsPerThread;

        for (int k = startK; k < endK; k++)
        {
            if (k >= windCount)
                break;

            if (!windSpeedChunks[k].isEnabled)
                continue;

            List<Vector3> vert = threadVerts[k];

            for (int j = 0; j < vert.Count; j++)
            {
                GridTile tile = GridTile.FindClosestGridTile(vert[j]);

                if (tile != null && tile.canSeeWind && !tile.isOutsideBorder)
                {
                    float curWind = WindController.GetWindAtTile(tile, height, time);
                    Vector2 curUv = new Vector2(1 - curWind / WindController.WindMagnitudeAtTime(time), 1 - curWind / WindController.WindMagnitudeAtTime(time));
                    for (int intp = 0; intp < intpSteps; intp++)
                    {

                        if (curUvs)
                        {
                            Vector2 interpolated = Vector2.Lerp(buffereduvs[k][(intpSteps - 1)][j], curUv, (float)intp / (intpSteps - 1));

                            uvs[k][intp][j].Set(interpolated.x, interpolated.y);
                        }
                        else
                        {
                            Vector2 interpolated = Vector2.Lerp(uvs[k][(intpSteps - 1)][j], curUv, (float)intp / (intpSteps - 1));

                            buffereduvs[k][intp][j].Set(interpolated.x, interpolated.y);
                        }

                        if (stopThreads)
                            return;

                    }
                }
                else if (tile != null && !tile.canSeeWind && !tile.isOutsideBorder)
                {
                    Vector2 curUv = new Vector2(0.125f, 0.875f);

                    for (int intp = 0; intp < intpSteps; intp++)
                    {

                        if (curUvs)
                        {
                            Vector2 interpolated = Vector2.Lerp(buffereduvs[k][(intpSteps - 1)][j], curUv, (float)intp / (intpSteps - 1));

                            uvs[k][intp][j].Set(interpolated.x, interpolated.y);
                        }
                        else
                        {
                            Vector2 interpolated = Vector2.Lerp(uvs[k][(intpSteps - 1)][j], curUv, (float)intp / (intpSteps - 1));

                            buffereduvs[k][intp][j].Set(interpolated.x, interpolated.y);
                        }

                        if (stopThreads)
                            return;

                    }
                }
            }  
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
