using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIResourcesManager : MonoBehaviour {    
    public Button menuButton;

    public Text capitalText;

    public Text publicAcceptanceText;

    public Text powerText;

    public Text dateText;

    public Button pauseButton;

    public Button speedButton1x;

    public Button speedButton10x;

    public Button buildButton;

    public Button infoButton;

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
}
