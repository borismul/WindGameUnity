using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class TreeAnimationController : MonoBehaviour
{
    public List<TerrainObject> treeObjects;
    public List<Vector3[]> meshVertices;
    public List<List<List<int>>> vertPosPerObject;
    public List<Vector3[]> updatedMeshVertices;
    public List<List<float>> lowestVertPerObject;
    public List<List<float>> randomValues;

    Thread[] threads = new Thread[4];
    Thread mainThread;
    List<ManualResetEvent> events = new List<ManualResetEvent>();

    public static TreeAnimationController instance;
    bool initialized = false;

    public float shakeSpeed = 0.6f;
    bool stopThread = false;

    readonly object lockUpdatedMeshVertices = new object();
    void Start()
    {
        treeObjects = new List<TerrainObject>();
        meshVertices = new List<Vector3[]>();
        vertPosPerObject = new List<List<List<int>>>();
        updatedMeshVertices = new List<Vector3[]>();
        lowestVertPerObject = new List<List<float>>();
        randomValues = new List<List<float>>();
        instance = this;
        StartCoroutine("AnimateTrees");
    }

    IEnumerator AnimateTrees()
    {
        while (true)
        {
            if (!TerrainController.thisTerrainController.levelLoaded)
            {
                yield return null;
                continue;
            }

            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            events.Clear();
            for (int i = 0; i < threads.Length; i++)
            {
                //System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(CalculateChunk);
                ManualResetEvent mre = new ManualResetEvent(false);
                ThreadPool.SetMaxThreads(4, 1);
                ThreadPool.QueueUserWorkItem(AnimatePart, i);
                mre.Set();
                events.Add(mre);
            }
            yield return new WaitForSeconds(0.05f);
            yield return WaitHandle.WaitAll(events.ToArray());

            //print("____________________________");
            //for (int i = 0; i < meshVertices.Count; i++)
            //{
            //    print(treeObjects[i].GetComponent<MeshFilter>().mesh.vertices.Length + "  " + updatedMeshVertices[i].Length);
            //}
            //print("____________________________");
            //while (!isdone)
            //    yield return null;

            for (int i = 0; i < meshVertices.Count; i++)
            {
                lock (lockUpdatedMeshVertices)
                    treeObjects[i].GetComponent<MeshFilter>().mesh.vertices = updatedMeshVertices[i];
            }
        }
    }

    void Initialize()
    {
        for (int i = 0; i < meshVertices.Count; i++)
        {
            updatedMeshVertices.Add((Vector3[])meshVertices[i].Clone());
        }
    }

    void AnimatePart(object i)
    {
        try
        {
            int operationsPerThread = Mathf.CeilToInt(treeObjects.Count / threads.Length) + 1;
            // For each terrain object that this thread has to do
            for (int j = (int)i * operationsPerThread; j < (((int)i + 1) * operationsPerThread); j++)
            {
                if (j >= updatedMeshVertices.Count)
                    break;

                Vector3[] newVertPos = new Vector3[meshVertices[j].Length];
                Vector3 windVector = -Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * WindController.direction), 0, Mathf.Cos(Mathf.Deg2Rad * WindController.direction)));

                // for each tree in this object
                for (int k = 0; k < vertPosPerObject[j].Count; k++)
                {
                    Vector3 move = windVector * shakeSpeed * (1 + (0.5f * Mathf.Sin(System.DateTime.Now.Second + System.DateTime.Now.Millisecond / 1000f * 1f) + 0.5f * Mathf.Sin((System.DateTime.Now.Second + System.DateTime.Now.Millisecond / 1000f) * randomValues[j][k])));
                    // for each vertex in this tree
                    for (int l = 0; l < vertPosPerObject[j][k].Count; l++)
                    {

                        float a = lowestVertPerObject[j][k];
                        float height = meshVertices[j][vertPosPerObject[j][k][l]].y - lowestVertPerObject[j][k];
                        newVertPos[vertPosPerObject[j][k][l]] = meshVertices[j][vertPosPerObject[j][k][l]] + Mathf.Sqrt(Mathf.Abs(height)) * move;


                    }
                }
                lock (lockUpdatedMeshVertices)
                {
                    updatedMeshVertices[j] = newVertPos;
                }
            }
        }
        catch(System.Exception e)
        {
            print(e);
        }
    }

    void OnApplicationQuit()
    {
        stopThread = true;

        if (mainThread != null)
            mainThread.Abort();

        foreach (Thread thread in threads)
        {
            if (thread != null)
                thread.Abort();
        }




    }
}
