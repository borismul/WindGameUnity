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
    }
	
	void Update () {
        Mission1Controller missionController = Mission1Controller.GetInstance();
        objectivesValue.text = GameResources.getObjectiveText();

    }

    void Exit()
    {
        Application.Quit();
    }

    void RestartMission()
    {
        TerrainController.thisTerrainController.DestroyAll();
        SceneManager.LoadScene("Mission1");
    }
    
    void MainMenu()
    {
        TerrainController.thisTerrainController.DestroyAll();
        SceneManager.LoadScene("Main Menu");
    }
}
