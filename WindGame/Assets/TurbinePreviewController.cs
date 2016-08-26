using UnityEngine;
using System.Collections;

public class TurbinePreviewController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 0.5f, 0);
	}
}
