using UnityEngine;
using System.Collections;

public class Mission2Controller : MonoBehaviour
{
    public bool[] firstObjectivesCompleted;
    public string[] firstObjectives;

    public bool[] secondObjectivesCompleted;
    public string[] secondObjectives;

    public System.DateTime objDate;

    private static Mission2Controller instance;
    private bool gameFinished = false;

    [Header("Prefabs")]
    public GameObject worldManagerPrefab;
    public GameObject resourceManagerPrefab;

    // Use this for initialization
    void Start()
    {

        CreateSingleton();
        InstantiateStartPrefabs();

        firstObjectives = new string[] { "Build a turbine", "Build a turbine with efficiency of 0.7 or higher" };
        firstObjectivesCompleted = new bool[] { false, false };

        secondObjectivesCompleted = new bool[] { false, false, true };
        secondObjectives = new string[] { "Generate 1000 power before 830 AD", "Get 20000 capital before 830 AD", "Keep public acceptance above -1" };

        objDate = new System.DateTime(830, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (firstObjectivesCompleted[0] && firstObjectivesCompleted[1])
        {
            checkSecondObjectives();
        }
        else
        {
            checkFirstObjectives();
        }

        GameResources.setObjectiveText(getObjectiveText());
    }

    void checkSecondObjectives()
    {
        if (GameResources.getProduction() >= 1000)
        {
            secondObjectivesCompleted[0] = true;
        }
        else
        {
            secondObjectivesCompleted[0] = false;
        }

        if (GameResources.getWealth() >= 20000)
        {
            secondObjectivesCompleted[1] = true;
        }
        else
        {
            secondObjectivesCompleted[1] = false;
        }

        if (System.DateTime.Compare(GameResources.getDate(), objDate) > 0 && !secondObjectivesCompleted[0])
            gameOver();

        if (System.DateTime.Compare(GameResources.getDate(), objDate) > 0 && !secondObjectivesCompleted[1])
            gameOver();

        if (GameResources.getPublicAcceptance() < -1)
            gameOver();

        if (secondObjectivesCompleted[0] && secondObjectivesCompleted[1])
            gameWon();
    }

    void checkFirstObjectives()
    {
        if (!firstObjectivesCompleted[0])
        {
            foreach (Transform child in TurbineManager.GetInstance().transform)
            {
                firstObjectivesCompleted[0] = true;
            }
        }
        else
        {
            foreach (Transform child in TurbineManager.GetInstance().transform)
            {
                if (child.gameObject.GetComponent<TurbineController>().efficiency > 0.7)
                    firstObjectivesCompleted[1] = true;
            }
        }
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
    public static Mission2Controller GetInstance()
    {
        return instance;
    }

    public string getObjectiveText()
    {
        string[] objectives;
        bool[] completion;
        if (firstObjectivesCompleted[0] && firstObjectivesCompleted[1])
        {
            objectives = secondObjectives;
            completion = secondObjectivesCompleted;
        }
        else
        {
            objectives = firstObjectives;
            completion = firstObjectivesCompleted;
        }


        string result = "";
        for (int i = 0; i < objectives.Length; i++)
        {
            if (completion[i])
            {
                result += "<color=green> ";
            }
            else
            {
                result += "<color=red> ";
            }
            result += objectives[i] + "</color> \n";
        }
        return result;
    }

    void gameOver()
    {
        if (!gameFinished)
        {
            gameFinished = true;
            UIScript.GetInstance().GameOver();
        }
    }

    void gameWon()
    {
        if (!gameFinished)
        {
            gameFinished = true;
            UIScript.GetInstance().GameWon();
        }
    }
}
