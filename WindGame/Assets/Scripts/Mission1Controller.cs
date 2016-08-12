using UnityEngine;
using System.Collections;

public class Mission1Controller : MonoBehaviour {
    public bool[] objectivesCompleted;
    public string[] objectives;
    public System.DateTime obj1Date;
    public System.DateTime obj2Date;

    [SerializeField]
    GameObject worldObj;

    [SerializeField]
    GameObject UIObj;

    // WorldController world;
    UIScript ui;


	// Use this for initialization
	void Start () {
        objectivesCompleted = new bool[] { false, false, true };
        objectives = new string[] { "Generate 200 power before 830 AD", "Get 1000 capital before 850 AD", "Keep public acceptance above -40" };
        // world = worldObj.GetComponent<WorldController>();
        ui = UIObj.GetComponent<UIScript>();

        obj1Date = new System.DateTime(830, 1, 1);
        obj2Date = new System.DateTime(850, 1, 1);
	}

    // Update is called once per frame
    void Update() {
        // if (world.totalPower >= 200)
        // {
        //     objectivesCompleted[0] = true;
        // } else
        // {
        //     objectivesCompleted[0] = false;
        // }

        // if (world.capital >= 1000)
        // { 
        //     objectivesCompleted[1] = true;
        // } else
        // {
        //     objectivesCompleted[1] = false;
        // }

        // if (System.DateTime.Compare(world.date, obj1Date) > 0 && !objectivesCompleted[0])
        //     gameOver();

        // if (System.DateTime.Compare(world.date, obj2Date) > 0 && !objectivesCompleted[1])
        //     gameOver();

        // if (world.publicAcceptance < -40)
        //     gameOver();

        // if (objectivesCompleted[0] && objectivesCompleted[1])
        //     gameWon();

	}

    void gameOver()
    {
        ui.GameOver();
    }

    void gameWon()
    {
        ui.GameWon();
    }
}
