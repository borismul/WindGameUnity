using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CompassController : MonoBehaviour {

    public GameObject pointer;
    public GameObject windPointer;
    public GameObject background;
    public GameObject north;
    public GameObject east;
    public GameObject south;
    public GameObject west;
    float r;


    // Use this for initialization
    void Start ()
    {
        r = Vector3.Distance(background.transform.position, north.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
        float rotation = Camera.main.transform.rotation.eulerAngles.y;
        background.transform.rotation = Quaternion.Euler(0, 0, rotation);
        east.transform.position = new Vector3(r * Mathf.Cos(rotation * Mathf.Deg2Rad), r * Mathf.Sin(rotation * Mathf.Deg2Rad)) + background.transform.position;
        north.transform.position = new Vector3(r * Mathf.Cos((rotation + 90) * Mathf.Deg2Rad), r * Mathf.Sin((rotation + 90) * Mathf.Deg2Rad)) + background.transform.position;
        west.transform.position = new Vector3(r * Mathf.Cos((rotation + 180) * Mathf.Deg2Rad), r * Mathf.Sin((rotation + 180) * Mathf.Deg2Rad)) + background.transform.position;
        south.transform.position = new Vector3(r * Mathf.Cos((rotation + 270) * Mathf.Deg2Rad), r * Mathf.Sin((rotation + 270) * Mathf.Deg2Rad)) + background.transform.position;
        windPointer.transform.rotation = Quaternion.Euler(background.transform.rotation.eulerAngles + new Vector3(0, 0, 180 - WindController.direction));
    }
}
