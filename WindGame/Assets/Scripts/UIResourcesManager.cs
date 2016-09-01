using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UIResourcesManager : MonoBehaviour {    

    public Button menuButton;

    public Text capitalText;

    public Text publicAcceptanceText;

    public Text powerText;

    public Text scoreText;

    public Text dateText;

    public Button pauseButton;

    public Button speedButton1x;

    public Button speedButton10x;

    public Button buildButton;

    public Button infoButton;

    float coeffWealth = 1;              // coefficient for total wealth
    float coeffProduction = 100;          // coefficient for total production (power)
    float coeffValue = 0.3f;               // coefficient for total value of the deployed turbines
    float coeffTime = -0.1f;                // coefficient for spent time (or time left)
    // float coeffSize = 1;             // coefficient for city size                    CITY GROWTH STILL TO BE IMPLEMENTED!!!!

    DateTime initialDate = new DateTime(800, 10, 10);               // initial game date

    // Use this for initialization
    void Start ()
    {
        pauseButton.onClick.AddListener(Pause);
        speedButton1x.onClick.AddListener(SetSpeed1x);
        speedButton10x.onClick.AddListener(SetSpeed10x);
        menuButton.onClick.AddListener(Menu);
        buildButton.onClick.AddListener(BuildButton);
        infoButton.onClick.AddListener(InfoButton);
    }
	
	// Update is called once per frame
	void Update ()
    {
        capitalText.text = GameResources.getWealth().ToString("0");
        publicAcceptanceText.text = GameResources.getPublicAcceptance().ToString("F1");
        powerText.text = GameResources.getProduction().ToString("0");
        dateText.text = GameResources.getDate().ToString("dd-MM-yyyy");
        scoreText.text = CalculateScore().ToString("F0");
    }

    void Pause()
    {
        GameResources.pause();
    }

    void SetSpeed1x()
    {
        GameResources.unPause();
        GameResources.setGameSpeed(200);
    }

    void SetSpeed10x()
    {
        GameResources.unPause();
        GameResources.setGameSpeed(2000);
    }

    void BuildButton()
    {
        UIScript.GetInstance().BuildMenu();
    }

    void InfoButton()
    {
        UIScript.GetInstance().OpenTileMenu();

    }

    void Menu()
    {
        bool hideSpeedButtons = UIScript.GetInstance().menuButtonPress();
        pauseButton.gameObject.SetActive(hideSpeedButtons);
        speedButton1x.gameObject.SetActive(hideSpeedButtons);
        speedButton10x.gameObject.SetActive(hideSpeedButtons);
    }

    float CalculateScore()
    {

        float value = 0;
        
        for (int i = 0; i < TurbineManager.GetInstance().GetTurbineCount(); i++)
        {
            value += TurbineManager.GetInstance().transform.GetChild(i).GetComponent<TurbineController>().price;
        }
        
        double days = (GameResources.getDate() - initialDate).TotalDays;

        return coeffWealth * GameResources.getWealth() + coeffProduction * GameResources.getProduction() + coeffValue * value + coeffTime * Convert.ToSingle(days);

    }
}
