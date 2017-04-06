using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    [Header("Top Level Prefabs")]
    public GameObject MainMenuCanvasPrefab;
    public GameObject[] MissionCanvasPrefab;

    [Header("Mission Specific Prefabs")]
    public GameObject[] BuildMenuPrefab;
    //public GameObject USBuildMenuPrefab;
    public GameObject pauseMenuPrefab;
    //public GameObject panemoneInformationPrefab;
    public GameObject tileInformationPrefab;
    public GameObject weatherMenuPrefab;

    // Local GameObjects
    List<GameObject> menus;
    CameraController cameraController;


    // Local variables
    int menuActive;                 // If a menu is currently open?
    int activeUIElements = 1;       // AMount of active UI elements?
    bool startGiven;                // ?
    bool inBuildMode;               // If build mode is active

    void Start()
    {
        CreateSingleton();
        //InstantiateUIPrefabs();
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    
    // Method to create the correct mission canvas with the scene
    public void LoadMissionCanvas(int level)
    {
        GameObject missionCanvas = Instantiate(MissionCanvasPrefab[level-1]);
        missionCanvas.transform.SetParent(transform);
    }
   

    // Create a singleton instance of the UI Manager
    void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("UI Manager already exists!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    // Instantiate the starting prefabs as the children of the UIScript
    void InstantiateUIPrefabs()
    {
        GameObject pauseMenu = Instantiate(pauseMenuPrefab);
        pauseMenu.transform.SetParent(transform);
        pauseMenu.SetActive(false);
        menus.Add(pauseMenu);

        GameObject buildMenu = Instantiate(BuildMenuPrefab[SceneManager.GetActiveScene().buildIndex - 1]);
        buildMenu.transform.SetParent(transform);
        buildMenu.SetActive(false);
        menus.Add(buildMenu);

        //GameObject panemoneInformation = Instantiate(panemoneInformationPrefab);
        //panemoneInformation.transform.SetParent(transform);
        //panemoneInformation.SetActive(false);
        //menus.Add(panemoneInformation);

        GameObject tileInformation = Instantiate(tileInformationPrefab);
        tileInformation.transform.SetParent(transform);
        tileInformation.SetActive(false);
        menus.Add(tileInformation);

        GameObject weatherMenu = Instantiate(weatherMenuPrefab);
        weatherMenu.transform.SetParent(transform);
        weatherMenu.SetActive(false);
        menus.Add(weatherMenu);
    }
    
    // Get the singleton instance
    public static UIManager GetInstance()
    {
        return instance;
    }
    
    public bool menuButtonPress()
    {
        if (menuActive == 1)
        {
            menuActive = -1;
            menus[2].SetActive(false);
            GameResources.unPause();
            activeUIElements--;
            return true;
        } else
        {
            if (menuActive > -1) return true;
            menuActive = 1;
            menus[1].SetActive(true);
            menus[1].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
            GameResources.pause();
            activeUIElements++;
            return false;
        }
    }

    public void OpenTurbineMenu(TurbineController target)
    {
        menuActive = 3;
        menus[3].GetComponent<PanemoneInformationMenu>().SetTurbine(target);
        menus[3].SetActive(true);
        menus[3].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;

        activeUIElements++;
    }

    public void CloseTurbineMenu()
    {
        menuActive = -1;
        menus[3].SetActive(false);
        menus[3].GetComponent<PanemoneInformationMenu>().ClearTurbine();
        activeUIElements--;
    }

    public void OpenTileMenu()
    {
        menuActive = 4;
        menus[4].SetActive(true);
        menus[4].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        WorldInteractionController.GetInstance().SetInInfoMode(true);
        activeUIElements++;
    }

    public void SetActiveTile(GridTile til)
    {
        menus[4].GetComponent<TileInfomationMenu>().setTile(til);
    }

    public void CloseTileMenu()
    {
        menuActive = -1;
        menus[4].SetActive(false);
        activeUIElements--;
    }

    public void BuildMenu()
    {
        menus[2].SetActive(true);
        menus[2].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        cameraController.SetHaveControl(false);
        activeUIElements++;
    }

    public void CloseBuildMenu()
    {
        menus[2].SetActive(false);
        activeUIElements--;
    }

    public void OpenWindVaneMenu(WindVaneController windVaneController)
    {
        menus[6].SetActive(true);
        menus[6].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        activeUIElements++;

    }

    public void DisableLoadingScreen()
    {
        menus[5].SetActive(false);
    }

    public void SetInBuildMode(bool mode)
    {
        inBuildMode = mode;
    }

    public bool GetInBuildMode()
    {
        return inBuildMode;
    }

    public void GameWon()
    {
        menuButtonPress();
        menus[0].SetActive(false);
        menus[1].GetComponent<UIMainMenuManager>().GameWon();
    }

    public void GameOver()
    {
        menus[1].GetComponent<UIMainMenuManager>().GameOver();
    }
    

}
