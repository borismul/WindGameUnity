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

    private static Mission1Controller instance;

    [Header("Prefabs")]
    public GameObject worldManagerPrefab;
    public GameObject resourceManagerPrefab;

    // Use this for initialization
    void Start () {

        CreateSingleton();
        InstantiateStartPrefabs();

        objectivesCompleted = new bool[] { false, false, true };
        objectives = new string[] { "Generate 1000 power before 830 AD", "Get 20000 capital before 850 AD", "Keep public acceptance above -1" };
        // world = worldObj.GetComponent<WorldController>();
        ui = UIObj.GetComponent<UIScript>();

        obj1Date = new System.DateTime(830, 1, 1);
        obj2Date = new System.DateTime(850, 1, 1);
	}

    // Update is called once per frame
    void Update() {
        if (GameResources.getProduction() >= 1000)
        {
            objectivesCompleted[0] = true;
        }
        else
        {
            objectivesCompleted[0] = false;
        }

        if (GameResources.getWealth() >= 20000)
        {
            objectivesCompleted[1] = true;
        }
        else
        {
            objectivesCompleted[1] = false;
        }

        if (System.DateTime.Compare(GameResources.getDate(), obj1Date) > 0 && !objectivesCompleted[0])
            gameOver();

        if (System.DateTime.Compare(GameResources.getDate(), obj2Date) > 0 && !objectivesCompleted[1])
            gameOver();

        if (GameResources.getPublicAcceptance() < -1)
            gameOver();

        if (objectivesCompleted[0] && objectivesCompleted[1])
            gameWon();
    }

    // Create the singletone for the Mission1Manager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("Mission1Manager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the Mission1Manager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(worldManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(resourceManagerPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public Mission1Controller GetInstance()
    {
        return instance;
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
