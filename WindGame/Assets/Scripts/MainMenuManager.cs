using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour {

    [SerializeField]
    GameObject mainMenuPrefab;      // Prefab of the main menu GameObject

    public Button editorButton;

    void Start()
    {
        // Instantiate the main menu when the main menu scene starts
        InstantiateMainMenu();

        #if UNITY_EDITOR
            editorButton.gameObject.SetActive(true);
        #endif
    }

    // Method that instatiates the main menu if it does not exists already
    void InstantiateMainMenu()
    {
        if (MainMenuController.curMainMenu == null)
            MainMenuController.curMainMenu = Instantiate(mainMenuPrefab).GetComponent<MainMenuController>();
        else
            Debug.LogError("Main Menu is already instantiated.");
    }

}
