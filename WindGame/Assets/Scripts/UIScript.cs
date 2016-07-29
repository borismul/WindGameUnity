using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

    [SerializeField]
    GameObject worldObj;

    [SerializeField]
    GameObject missionControllerObj;

    [SerializeField]
    GameObject exitButton;

    [SerializeField]
    GameObject menuButton;

    [SerializeField]
    GameObject menuText;

    [SerializeField]
    GameObject closeMenuButton;

    [SerializeField]
    GameObject objectivesValue;

    [SerializeField]
    GameObject menuScreen;

    [SerializeField]
    GameObject capitalText;

    [SerializeField]
    GameObject publicAcceptanceText;

    [SerializeField]
    GameObject powerText;

    [SerializeField]
    GameObject dateText;

    [SerializeField]
    GameObject pauseButton;

    [SerializeField]
    GameObject speedButton1x;

    [SerializeField]
    GameObject speedButton10x;

    WorldController world;
    Mission1Controller missionController;

    float oldspeed;

    // Use this for initialization
    void Start () {
        world = worldObj.GetComponent<WorldController>();
        missionController = missionControllerObj.GetComponent<Mission1Controller>();
        pauseButton.GetComponent<Button>().onClick.AddListener(Pause);
        speedButton1x.GetComponent<Button>().onClick.AddListener(SetSpeed1x);
        speedButton10x.GetComponent<Button>().onClick.AddListener(SetSpeed10x);
        menuButton.GetComponent<Button>().onClick.AddListener(Menu);
        closeMenuButton.GetComponent<Button>().onClick.AddListener(Menu);
        exitButton.GetComponent<Button>().onClick.AddListener(Exit);

        menuScreen.SetActive(false);

        SetSpeed1x();
    }

    // Update is called once per frame
    void Update () {
        Text currtext;
        currtext = capitalText.GetComponent<Text>();
        currtext.text = System.Math.Round(world.capital).ToString();

        currtext = publicAcceptanceText.GetComponent<Text>();
        currtext.text = world.publicAcceptance.ToString();

        currtext = powerText.GetComponent<Text>();
        currtext.text = world.totalPower.ToString();

        currtext = dateText.GetComponent<Text>();
        currtext.text = world.date.Date.ToString();

        if (menuScreen.activeSelf)
        {
            Text txt = objectivesValue.GetComponent<Text>();
            string result = "";
            for (int i = 0; i < missionController.objectives.Length; i++){
                if (missionController.objectivesCompleted[i])
                {
                    result += "<color=green> ";
                } else
                {
                    result += "<color=red> ";
                }
                result += missionController.objectives[i] + "</color> \n";
            }
            txt.text = result;
        }
    }

    void Pause()
    {
        world.gameSpeed = 0;
    }

    //Base gamespeed = 200
    void SetSpeed1x()
    {
        world.gameSpeed = 200;
    }

    void SetSpeed10x()
    {
        world.gameSpeed = 2000;
    }

    void Menu()
    {
        if (menuScreen.activeSelf)
        {
            pauseButton.SetActive(true);
            speedButton10x.SetActive(true);
            speedButton1x.SetActive(true);

            menuScreen.SetActive(false);
            world.gameSpeed = oldspeed;
        }
        else
        {
            pauseButton.SetActive(false);
            speedButton10x.SetActive(false);
            speedButton1x.SetActive(false);

            oldspeed = world.gameSpeed;
            menuScreen.SetActive(true);
            world.gameSpeed = 0;
        }
    }

    public void GameOver()
    {
        menuText.GetComponent<Text>().text = "Mission Failed";
        world.gameSpeed = 0;
        menuScreen.SetActive(true);
        menuButton.SetActive(false);
        closeMenuButton.SetActive(false);
        pauseButton.SetActive(false);
        speedButton10x.SetActive(false);
        speedButton1x.SetActive(false);
    }

    public void GameWon()
    {
        menuText.GetComponent<Text>().text = "Mission Finished";
        world.gameSpeed = 0;
        menuScreen.SetActive(true);
        menuButton.SetActive(false);
        closeMenuButton.SetActive(false);
        pauseButton.SetActive(false);
        speedButton10x.SetActive(false);
        speedButton1x.SetActive(false);
    }

    void Exit()
    {
        Application.Quit();
    }
}
