using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIMainMenuManager : MonoBehaviour {
    public Text objectivesValue;

    public Button closeMenuButton;

    public Button restartButton;

    public Button exitButton;

    public Text menuText;

    // Use this for initialization
    void Start () {
        closeMenuButton.onClick.AddListener(CloseMenu);
        restartButton.onClick.AddListener(RestartMission);
        exitButton.onClick.AddListener(Exit);
    }
	
	// Update is called once per frame
	void Update () {
        /*
        string result = "";
        for (int i = 0; i < missionController.objectives.Length; i++)
        {
            if (missionController.objectivesCompleted[i])
            {
                result += "<color=green> ";
            }
            else
            {
                result += "<color=red> ";
            }
            result += missionController.objectives[i] + "</color> \n";
        }
        objectivesValue.text = result;
        */
    }

    void Exit()
    {
        Application.Quit();
    }

    void RestartMission()
    {
        SceneManager.LoadScene("Mission1");
    }

    void CloseMenu()
    {
        GameResources.setGameSpeed(200);
        //Destroy the menu
    }
}
