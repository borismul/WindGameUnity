using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    Button PersiaButton;        // Button that links to Persia mission

    [SerializeField]
    Button DutchButton;         // Button that links to Dutch Repulic mission

    [SerializeField]
    Button RuralButton;         // Button that links to Rural Town mission

    [SerializeField]
    Button USButton;            // Button that links to United States mission

    [SerializeField]
    Button EuropeButton;        // Button that links to Western Europe mission

    [SerializeField]
    Button SeaButton;           // Button that links to Norh Sea mission

    public static MainMenuController curMainMenu;   // Static object this main menu controller so it can be obtained easily from anywhere

    // Method gets called when object is initiated
    void Start()
    {
        // Add a method to a click on the button
        PersiaButton.onClick.AddListener(Persia);
        DutchButton.onClick.AddListener(Dutch);
        RuralButton.onClick.AddListener(Rural);
        USButton.onClick.AddListener(US);
        EuropeButton.onClick.AddListener(Europe);
        SeaButton.onClick.AddListener(Sea);
    }

    // Method loads the Persia scene
    void Persia()
    {
        SceneManager.LoadScene("1_Persia");
    }

    // Method loads the Dutch Repulic scene
    void Dutch()
    {
        SceneManager.LoadScene("2_DutchRepulic");
    }

    // Method loads the Rural Town scene
    void Rural()
    {
        SceneManager.LoadScene("3_RuralTown");
    }

    // Method loads the United States scene
    void US()
    {
        SceneManager.LoadScene("4_UnitedStates");
    }

    // Method loads the Western Europe scene
    void Europe()
    {
        SceneManager.LoadScene("5_WesternEurope");
    }

    // Method loads the North Sea scene
    void Sea()
    {
        SceneManager.LoadScene("6_NorthSea");
    }



}
