using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    Button mission1Button;          // Button that links to first mission

    public static MainMenuController curMainMenu;   // Static object this main menu controller so it can be obtained easily from anywhere

    // Method gets called when object is initiated
    void Start()
    {
        // Add a method to a click on the button
        mission1Button.onClick.AddListener(Mission1Button);
    }

    // Method loads the mission1 scene
    void Mission1Button()
    {
        SceneManager.LoadScene("Mission1");
    }

}
