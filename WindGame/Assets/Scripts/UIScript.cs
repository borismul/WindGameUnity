using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {

    private static UIScript instance;

    //List<GameObject> menus;

    //[Header("Prefabs")]
    //public GameObject eventSystemPrefab;
    //public GameObject[] BuildMenuPrefab;
    //public GameObject USBuildMenuPrefab;
    //public GameObject[] MissionCanvasPrefab;
    //public GameObject pauseMenuPrefab;
    //public GameObject tutorialPrefab;
    //public GameObject panemoneInformationPrefab;
    //public GameObject tileInformationPrefab;
    //public GameObject loadingMenuPrefab;
    //public GameObject weatherMenuPrefab;
    CameraController cameraController;

    GameObject managerObj;
    GameObject cameraObj;

    int menuActive;

    int activeUIElements = 1;

    bool startGiven;

    bool inBuildMode;

    // Use this for initialization
    void Awake ()
    {
        CreateSingleton();
        //menus = new List<GameObject>();
        InstantiateStartPrefabs();
        menuActive = -1;
        
    }

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AccessSiblingData() & !startGiven)
        {
            //GameObject obj7 = Instantiate(tutorialPrefab);
            //obj7.transform.SetParent(transform);
            GameResources.unPause();
            GameResources.setGameSpeed(200);
            startGiven = true;
        }
    }

    // Create the singleton for the UIManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("UIManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the UIScript
    void InstantiateStartPrefabs()
    {
        //GameObject obj = Instantiate(eventSystemPrefab);
        //obj.transform.SetParent(transform);

        //GameObject obj2 = Instantiate(MissionCanvasPrefab[SceneManager.GetActiveScene().buildIndex - 1]);
        //obj2.transform.SetParent(transform);
        //menus.Add(obj2);

        //GameObject obj3 = Instantiate(pauseMenuPrefab);
        //obj3.transform.SetParent(transform);
        //obj3.SetActive(false);
        //menus.Add(obj3);

        //GameObject obj4 = Instantiate(BuildMenuPrefab[SceneManager.GetActiveScene().buildIndex - 1]);
        //obj4.transform.SetParent(transform);
        //obj4.SetActive(false);
        //menus.Add(obj4);

        //GameObject obj5 = Instantiate(panemoneInformationPrefab);
        //obj5.transform.SetParent(transform);
        //obj5.SetActive(false);
        //menus.Add(obj5);

        //GameObject obj6 = Instantiate(tileInformationPrefab);
        //obj6.transform.SetParent(transform);
        //obj6.SetActive(false);
        //menus.Add(obj6);

        //GameObject obj7 = Instantiate(loadingMenuPrefab);
        //obj7.transform.SetParent(transform);
        //menus.Add(obj7);

        //GameObject obj8 = Instantiate(weatherMenuPrefab);
        //obj8.transform.SetParent(transform);
        //obj8.SetActive(false);
        //menus.Add(obj8);
    }

    // Get the singleton instance
    public static UIScript GetInstance()
    {
        return instance;
    }

    public bool menuButtonPress()
    {
        if (menuActive == 1)
        {
            menuActive = -1;
            //menus[1].SetActive(false);
            GameResources.unPause();
            activeUIElements--;
            return true;
        } else
        {
            if (menuActive > -1) return true;
            menuActive = 1;
            //menus[1].SetActive(true);
            //menus[1].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
            GameResources.pause();
            activeUIElements++;
            return false;
        }
    }

    public void OpenTurbineMenu(TurbineController target)
    {
        menuActive = 3;
        //menus[3].GetComponent<PanemoneInformationMenu>().SetTurbine(target);
        //menus[3].SetActive(true);
        //menus[3].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;

        activeUIElements++;
    }

    public void CloseTurbineMenu()
    {
        menuActive = -1;
        //menus[3].SetActive(false);
        //menus[3].GetComponent<PanemoneInformationMenu>().ClearTurbine();
        activeUIElements--;
    }

    public void OpenTileMenu()
    {
        menuActive = 4;
        //menus[4].SetActive(true);
        //menus[4].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        WorldInteractionController.GetInstance().SetInInfoMode(true);
        activeUIElements++;
    }

    public void SetActiveTile(GridTile til)
    {
        //menus[4].GetComponent<TileInfomationMenu>().setTile(til);
    }

    public void CloseTileMenu()
    {
        menuActive = -1;
       // menus[4].SetActive(false);
        activeUIElements--;
    }

    public void BuildMenu()
    {
        //menus[2].SetActive(true);
        //menus[2].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        cameraController.SetHaveControl(false);
        activeUIElements++;
    }

    public void CloseBuildMenu()
    {
        //menus[2].SetActive(false);
        activeUIElements--;
    }

    public void OpenWindVaneMenu(WindVaneController windVaneController)
    {
        //menus[6].SetActive(true);
        //menus[6].GetComponentInChildren<Canvas>().sortingOrder = activeUIElements + 1;
        activeUIElements++;

    }

    //Get to know if the camera has zoomed on the village
    bool AccessSiblingData()
    {
        managerObj = gameObject.transform.parent.gameObject;
        cameraObj = managerObj.transform.GetChild(2).gameObject;

        return cameraObj.GetComponent<CameraController>().GetHaveControl();
    }

    public void DisableLoadingScreen()
    {
        //menus[5].SetActive(false);
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
        //menus[0].SetActive(false);
        //menus[1].GetComponent<UIMainMenuManager>().GameWon();
    }

    public void GameOver()
    {
        //menus[1].GetComponent<UIMainMenuManager>().GameOver();
    }

}
