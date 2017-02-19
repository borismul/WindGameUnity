using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenuController : MonoBehaviour {

    [SerializeField]
    Button mainMenuButton;

	// Use this for initialization
	void Start ()
    {
        mainMenuButton.onClick.AddListener(MainMenu);
	}
	
	void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
