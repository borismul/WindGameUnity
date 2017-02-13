using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIMainMenuManager : MonoBehaviour {
    public Text objectivesValue;

    public Button restartButton;
    public Button mainMenuButton;

    public GameObject nextMissionButton;

    public Text menuText;
    
    void Start () {
        mainMenuButton.onClick.AddListener(MainMenu);
        restartButton.onClick.AddListener(RestartMission);
        nextMissionButton.GetComponent<Button>().onClick.AddListener(NextMission);
        restartButton.onClick.AddListener(MainMenu);
        nextMissionButton.SetActive(false);
    }
	
	void Update () {
        //objectivesValue.text = GameResources.getObjectiveText();
        string objText = GameResources.getObjectiveText();
        if(GameResources.finalScore != 0)
        {
            objText += "\n Your final score was: " + GameResources.finalScore.ToString("0");
        }
        objectivesValue.text = objText;
    }

    void RestartMission()
    {
        TerrainController.thisTerrainController.DestroyAll();
        SceneManager.LoadScene("Mission1");
    }

    void NextMission()
    {
        GameResources.currentMission++;
        SceneManager.LoadScene("Mission1");
    }
    
    void MainMenu()
    {
        TerrainController.thisTerrainController.DestroyAll();
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        menuText.text = "Game Over";
    }

    public void GameWon()
    {
        menuText.text = "Mission Succes";
        nextMissionButton.SetActive(true);
    }
}
