using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {

    private static UIScript instance;

    List<GameObject> menus;

    [Header("Prefabs")]
    public GameObject eventSystemPrefab;
    public GameObject buildMenuPrefab;
    public GameObject resourcesMenuPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject mainMenuPrefab;
    public GameObject radialMenuPrefab;
    public GameObject tutorialPrefab;
    public GameObject panemoneInformationPrefab;
    public GameObject tileInformationPrefab;
    public GameObject loadingMenuPrefab;

    GameObject managerObj;
    GameObject cameraObj;

    int menuActive;

    bool startGiven;

    // Use this for initialization
    void Awake ()
    {
        CreateSingleton();
        menus = new List<GameObject>();
        InstantiateStartPrefabs();
        menuActive = -1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (AccessSiblingData() & !startGiven)
        {
            GameObject obj7 = Instantiate(tutorialPrefab);
            obj7.transform.SetParent(transform);
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
        GameObject obj = Instantiate(eventSystemPrefab);
        obj.transform.SetParent(transform);

        GameObject obj2 = Instantiate(resourcesMenuPrefab);
        obj2.transform.SetParent(transform);
        menus.Add(obj2);

        GameObject obj3 = Instantiate(pauseMenuPrefab);
        obj3.transform.SetParent(transform);
        obj3.SetActive(false);
        menus.Add(obj3);

        GameObject obj4 = Instantiate(buildMenuPrefab);
        obj4.transform.SetParent(transform);
        obj4.SetActive(false);
        menus.Add(obj4);

        GameObject obj5 = Instantiate(panemoneInformationPrefab);
        obj5.transform.SetParent(transform);
        obj5.SetActive(false);
        menus.Add(obj5);

        GameObject obj6 = Instantiate(tileInformationPrefab);
        obj6.transform.SetParent(transform);
        obj6.SetActive(false);
        menus.Add(obj6);

        GameObject obj7 = Instantiate(loadingMenuPrefab);
        obj7.transform.SetParent(transform);
        menus.Add(obj7);
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
            menus[1].SetActive(false);
            GameResources.unPause();
            return true;
        } else
        {
            if (menuActive > -1) return true;
            menuActive = 1;
            menus[1].SetActive(true);
            GameResources.pause();
            return false;
        }
    }

    public void OpenTurbineMenu(GameObject target)
    {
        if (menuActive > -1) return;

        menuActive = 3;
        menus[3].GetComponent<PanemoneInformationMenu>().SetTurbine(target.GetComponent<TurbineController>());
        menus[3].SetActive(true);
    }

    public void CloseTurbineMenu()
    {
        menuActive = -1;
        menus[3].SetActive(false);
        menus[3].GetComponent<PanemoneInformationMenu>().ClearTurbine();
    }

    public void setActiveTurbine(TurbineController tur)
    {
        menus[3].GetComponent<PanemoneInformationMenu>().SetTurbine(tur);
    }

    public void clearActiveTurbine()
    {
        menus[3].GetComponent<PanemoneInformationMenu>().ClearTurbine();
    }

    public void OpenTileMenu()
    {
        if (menuActive > -1) return;

        menuActive = 4;
        menus[4].SetActive(true);

        WorldInteractionController.GetInstance().SetInInfoMode(true);
    }

    public void SetActiveTile(GridTile til)
    {
        menus[4].GetComponent<TileInfomationMenu>().setTile(til);
    }

    public void CloseTileMenu()
    {
        menuActive = -1;
        menus[4].SetActive(false);

    }

    public void BuildMenu()
    {
        menus[2].SetActive(true);

    }

    // Get to know if the camera has zoomed on the village
    bool AccessSiblingData()
    {
        managerObj = gameObject.transform.parent.gameObject;
        cameraObj = managerObj.transform.GetChild(2).gameObject;

        return cameraObj.GetComponent<CameraController>().haveControl;
    }

    public void DisableLoadingScreen()
    {
        menus[5].SetActive(false);
    }

}
