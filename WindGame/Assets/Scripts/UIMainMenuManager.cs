using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenuManager : MonoBehaviour {
    public Text objectivesValue;

    public Button closeMenuButton;

    public Button exitButton;

    public Text menuText;

    float oldspeed;

    // Use this for initialization
    void Start () {
        closeMenuButton.onClick.AddListener(CloseMenu);
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

    public void setOldSpeed(float speed)
    {
        oldspeed = speed;
    }

    void Exit()
    {
        Application.Quit();
    }

    void CloseMenu()
    {
        GameResources.setGameSpeed(oldspeed);
        //Destroy the menu
    }
}
