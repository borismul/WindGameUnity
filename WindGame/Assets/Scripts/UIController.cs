using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



using System.Collections.Generic;

public class UIController : MonoBehaviour {

    [Header("Buttons")]
    public Button PlayPause;
    public Button FastForward;
    public Button Build;
    public Button Objective;
    public Button Graph;
    public Button Exit;

    [Header("Menu's")]
    public GameObject BuildMenu;
    public GameObject ObjectivesMenu;
    public GameObject GraphMenu;
    public GameObject ExitMenu;

    // List of the names of the instantiated Prefabs here
    GameObject buildMenu;
    GameObject objectivesMenu;
    GameObject graphMenu;
    GameObject exitMenu;


    // Start of the code
    void Start () {
        InstantiateMenus();

        // Add listeners to the buttons
        PlayPause.onClick.AddListener(OpenPauseMenu);
        FastForward.onClick.AddListener(OpenFastForward);
        Build.onClick.AddListener(OpenBuildMenu);
        Objective.onClick.AddListener(OpenObjectiveMenu);
        Graph.onClick.AddListener(OpenGraphMenu);
        Exit.onClick.AddListener(OpenExitMenu);
    }

    void InstantiateMenus()
    {
        buildMenu = Instantiate(BuildMenu);
        buildMenu.transform.SetParent(transform);
        buildMenu.SetActive(false);

        /*
        objectivesMenu = Instantiate(ObjectivesMenu);
        objectivesMenu.transform.SetParent(transform);
        objectivesMenu.SetActive(false);

        graphMenu = Instantiate(GraphMenu);
        graphMenu.transform.SetParent(transform);
        graphMenu.SetActive(false);

        exitMenu = Instantiate(ExitMenu);
        exitMenu.transform.SetParent(transform);
        exitMenu.SetActive(false);
        */
    }

    void OpenPauseMenu()
    {
    }

    void OpenFastForward()
    {
    }

    void OpenBuildMenu()
    {
        if (buildMenu.activeInHierarchy == true)
        {
            buildMenu.SetActive(false);
        }
        else
        {
            buildMenu.SetActive(true);
        }
    }

    void OpenObjectiveMenu()
    {
        if (objectivesMenu.activeInHierarchy == true)
        {
            objectivesMenu.SetActive(false);
        }
        else
        {
            objectivesMenu.SetActive(true);
        }
    }
    void OpenGraphMenu()
    {
        if (graphMenu.activeInHierarchy == true)
        {
            graphMenu.SetActive(false);
        }
        else
        {
            graphMenu.SetActive(true);
        }
    }
    void OpenExitMenu()
    {
        if (exitMenu.activeInHierarchy == true)
        {
            exitMenu.SetActive(false);
        }
        else
        {
            exitMenu.SetActive(true);
        }
    }


}
