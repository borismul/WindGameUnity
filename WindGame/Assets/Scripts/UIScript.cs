using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

    [SerializeField]
    GameObject worldObj;

    [SerializeField]
    GameObject menuButton;

    [SerializeField]
    GameObject closeMenuButton;

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

    float oldspeed;

    // Use this for initialization
    void Start () {
        world = worldObj.GetComponent<WorldController>();
        pauseButton.GetComponent<Button>().onClick.AddListener(Pause);
        speedButton1x.GetComponent<Button>().onClick.AddListener(SetSpeed1x);
        speedButton10x.GetComponent<Button>().onClick.AddListener(SetSpeed10x);
        menuButton.GetComponent<Button>().onClick.AddListener(Menu);
        closeMenuButton.GetComponent<Button>().onClick.AddListener(Menu);

        menuScreen.SetActive(false);
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
}
