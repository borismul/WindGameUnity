using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TurbinePreviewController : MonoBehaviour {


    public List<GameObject> blades = new List<GameObject>();
    [HideInInspector]
    public GameObject wall;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 0.5f, 0);
	}
}
