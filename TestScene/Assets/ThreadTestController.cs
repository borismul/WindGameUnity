using UnityEngine;
using System.Collections;
using System.Threading;
public class ThreadTestController : MonoBehaviour {

    Vector3 lookDir;
    Vector3 forward;
    float result;
    bool isThreadDone;
    // Use this for initialization
    void Start ()
    {
        lookDir = transform.forward;
        forward = Vector3.forward;
        StartCoroutine("GetThreadedDot");
	}

    IEnumerator GetThreadedDot()
    {
        Thread thread = new Thread(() => ThreadedDot(lookDir, forward));
        thread.Start();
        Debug.Log(result);
        while (!isThreadDone)
            yield return null;

        isThreadDone = false;
        Debug.Log(result);
    }

    void ThreadedDot(Vector3 a, Vector3 b)
    {
        result = ThreadVector3.Dot(new ThreadVector3(a), new ThreadVector3(b));
        isThreadDone = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
