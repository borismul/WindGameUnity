using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    //public static MainMenuController curMainMenu;   // Static object this main menu controller so it can be obtained easily from anywhere

    // Declaration of all the different buttons in the menu
    public Button persiaButton;
    public Button dutchButton;
    public Button ruralButton;
    public Button usButton;
    public Button euButton;
    public Button seaButton;


    // Add listener to all buttons
    void Start()
    {
        // Add listeners to all, and delegate loading of a scene OnClick (lambda expression)
        persiaButton.onClick.AddListener(() => GameManager.instance.LoadLevel(1));
        dutchButton.onClick.AddListener(() => GameManager.instance.LoadLevel(2));
        ruralButton.onClick.AddListener(() => GameManager.instance.LoadLevel(3));
        usButton.onClick.AddListener(() => GameManager.instance.LoadLevel(4));
        euButton.onClick.AddListener(() => GameManager.instance.LoadLevel(5));
        seaButton.onClick.AddListener(() => GameManager.instance.LoadLevel(6));
    }   
}