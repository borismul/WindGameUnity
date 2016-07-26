using UnityEngine;
using System.Collections;

public class WorldInteractionController : MonoBehaviour {

    public GameObject WindTurbine;
    public float cutOffRadius;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        MouseClick();
	}

    void MouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GridTile plantGrid = Grid.FindClosestGridTile(hit.point);
                GameObject instObj;
                if (plantGrid.canBuild && plantGrid.occupant == null)
                {
                    Vector3 plantPos = plantGrid.position;
                    instObj = (GameObject)Instantiate(WindTurbine, plantPos, Quaternion.identity);
                    Grid.SetOccupant(plantGrid, instObj, cutOffRadius);
                }
            }

        }
    }
}
