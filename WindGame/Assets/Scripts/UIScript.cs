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
        if (CameraController.haveControl & !startGiven)
        {
            GameObject obj5 = Instantiate(tutorialPrefab);
            obj5.transform.SetParent(transform);
            startGiven = true;
        }
    }

    // Create the singletone for the UIManager. Also checks if there is another present and logs and error.
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
        obj4.transform.SetParent(transform);
        obj4.SetActive(false);
        menus.Add(obj5);

    }

    // Get the singleton instance
    public static UIScript GetInstance()
    {
        return instance;
    }

    public bool menuButtonPress()
    {
        if (menus[1].activeSelf)
        {
            menus[1].SetActive(false);
            GameResources.unPause();
            return true;
        } else
        {
            menus[1].SetActive(true);
            GameResources.pause();
            return false;
        }
    }

    public void OpenTurbineMenu(GameObject target)
    {
        menus[3].GetComponent<PanemoneInformationMenu>().setTurbine(target.GetComponent<TurbineController>());
        menus[3].SetActive(true);
    }

    public void CloseTurbineMenu()
    {
        menus[3].SetActive(false);
        menus[3].GetComponent<PanemoneInformationMenu>().clearTurbine();
    }

    public void OpenTileMenu(GridTile target)
    {
        
    }

    public void CloseTileMenu()
    {

    }
}
