using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Declaration of static Singleton class
    public static GameManager instance;

    // Declaration of GameObjects to be managed
    public GameObject EventSystemPrefab;
    public GameObject UIManagerPrefab;
    public GameObject WorldManagerPrefab;
    public GameObject loadingMenuPrefab;
    public GameObject MainCameraPrefab;

    // Local GameObjects
    GameObject loadingScreen;

    void Start()
    {
        CreateSingleton();

        print("Gamemanager - Instantiating Event Manager");
        GameObject eventSystem = Instantiate(EventSystemPrefab);
        DontDestroyOnLoad(eventSystem);

        print("Gamemanager - Instantiating UI Manager");
        GameObject UIManager = Instantiate(UIManagerPrefab);
        
        SceneManager.sceneLoaded += LoadSceneContents;
        //instance.LoadLevel(4);  // Debug Purposes, load US immediately
    }

    // Create a singleton instance of the Game Manager
    void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("Gamemanager - Singleton created");
        }
        else if (instance != this)
        {
            Debug.LogError("GameManager already exists!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Functions handeled by the GameManager
    public void LoadLevel(int level)
    {
        print("Moving to level " + level);
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    void LoadSceneContents(Scene scene, LoadSceneMode mode)
    {
        // Open loadingscreen
        loadingScreen = Instantiate(loadingMenuPrefab);
        loadingScreen.transform.SetParent(transform);

        // Load mission specific UI layout
        UIManager.instance.LoadMissionCanvas(SceneManager.GetActiveScene().buildIndex);

        // Load world
        GameObject worldManager = Instantiate(WorldManagerPrefab);
        worldManager.transform.SetParent(transform);

        // Load main camera
        GameObject mainCamera = Instantiate(MainCameraPrefab);
        mainCamera.transform.SetParent(transform);

        // Checking for level loading
        InvokeRepeating("CloseLoadScreen", 0, 1f / 5);
        
    }

    // Close load screen if level is loaded
    void CloseLoadScreen()
    {
        print("Gamemanager - Loading level...");
        if (TerrainController.levelLoaded)
        {
            loadingScreen.SetActive(false);
            CancelInvoke("CloseLoadScreen");
            print("Gamemanager - Level loaded.");
        }
    }
   



    /*
    private static GameManager instance;

	void Awake ()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
	}

    // Create the singletone for the GameManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("GameManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the GameManager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(UIManagerPrefab);
        obj.transform.SetParent(transform);

        if (GameResources.currentMission == 1)
        {
            obj = Instantiate(mission1ManagerPrefab);
            obj.transform.SetParent(transform);
        }
        else
        {
            obj = Instantiate(mission2ManagerPrefab);
            obj.transform.SetParent(transform);
        }

        obj = Instantiate(mainCameraPrefab);
        obj.transform.SetParent(transform);

        
    }

    // Get the singleton instance
    public static GameManager GetInstance()
    {
        return instance;
    }
    */

}
