using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EditorMenuController : MonoBehaviour {

    public Button createMap;
    public Button mainMenu;
    public Button saveMap;

    public GameObject createMapPanel;
    public GameObject saveMapPanel;

    void Start()
    {
        createMap.onClick.AddListener(CreateMap);
        mainMenu.onClick.AddListener(MainMenu);
        saveMap.onClick.AddListener(SaveMapPanel);
    }

    void CreateMap()
    {
        if (CreateMapPanelController.createPanelMapController == null)
        {
            GameObject mapPanel = (GameObject)Instantiate(createMapPanel);
        }
    }

    void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void SaveMapPanel()
    {
        if (SavePanelController.thisSavePanel == null)
        {
            Instantiate(saveMapPanel);
        }
    }
}
