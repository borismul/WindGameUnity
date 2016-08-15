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
    public GameObject AreYouSurePanel;

    public bool isDirty = false;

    public static EditorMenuController singleton;

    void Start()
    {
        singleton = this;

        createMap.onClick.AddListener(CreateMap);
        mainMenu.onClick.AddListener(MainMenu);
        saveMap.onClick.AddListener(SaveMapPanel);
    }

    void CreateMap()
    {
        if (!isDirty && CreateMapPanelController.createPanelMapController == null)
        {
            Instantiate(createMapPanel);
        }
        else if(isDirty && AreYouSureMenuController.singleton == null)
        {
            AreYouSureMenuController controller = Instantiate(AreYouSurePanel).GetComponent<AreYouSureMenuController>();
            controller.yesCall += InstantiateMapPanel;
        }
    }

    void InstantiateMapPanel()
    {
        Instantiate(createMapPanel);
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
