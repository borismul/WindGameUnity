using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIMainMenuManager : MonoBehaviour {
    public Text objectivesValue;

    public Button restartButton;

    public Button exitButton;

    public Text menuText;
    
    void Start () {
        restartButton.onClick.AddListener(RestartMission);
        exitButton.onClick.AddListener(Exit);
    }
	
	void Update () {
        Mission1Controller missionController = Mission1Controller.GetInstance();
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
    }

    void Exit()
    {
        Application.Quit();
    }

    void RestartMission()
    {
        SceneManager.LoadScene("Mission1");
    }
}
