using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {

    public GameObject canvasPre;

    GameObject canvas;
    GameObject okButton;
    GameObject skipButton;

    List<string> head;
    List<string> message;

    Text textHead;
    Text textMess;

    int counter;
    
    bool tutorialStarted;
    bool tutorialEnded;
    bool okAdded;

	// Use this for initialization
	void Start () {

        head = new List<string>();
        head.Add("Welcome farmer!");
        head.Add("Camera controls");
        head.Add("Good job!");
        head.Add("Build menu");
        head.Add("Good job!");
        head.Add("Build your wind pole");
        head.Add("Good job!");

        message = new List<string>();
        message.Add("Welcome to Windgame! In the first mission you will be a farmer of ancient Persia and your objective is to " + 
                    "mill as much grain as possible for your population. But before letting you play around with turbines let's " +
                    "see a few features of the game together. You'll have your time to try all things out and get confident with them! " +
                    "Oh and by the way, if you already know about the game you can always press the Skip button in the bottom right " +
                    "corner of the window to skip this tutorial. Press Ok if you want to see what's next.");
        message.Add("So let's start off with some simple controls to see the world around you. Central mouse button and directional " +
                    "keys will help you with that. Come on, check them out now!");
        message.Add("Well done! Let's proceed to the next part with the Ok button.");
        message.Add("As you may have learnt, it is essential to choose wisely the placement and orientation of your windmills. " +
                    "This is why we need a wind pole. So let us build one! Right-click on any point in the world. A radial menu will " +
                    "pop up. Then choose the hammer icon to open up the build menu. Remember that you can always close the radial menu " +
                    "by simply left-clicking in any point in the world. Come on now, open up the build menu!");
        message.Add("Well done! Let's proceed to the next part with the Ok button.");
        message.Add("So this is the build menu! As you can see only the wind pole is available for construction now. So let's select it " +
                    "and add it in the map with the Build button. A shaded wind pole will appear in the world. You can move it around " +
                    "by simply moving your mouse in the world, then place it by left-clicking. Enough said, go build your wind pole!");
        message.Add("Well done! Let's proceed to the next part with the Ok button.");

        Invoke("OpenTutorial", 2);

	}
	
	// Update is called once per frame
	void Update () {

        if (!tutorialStarted || tutorialEnded || okAdded)
        {
            return;
        }

        okButton = canvas.transform.GetChild(0).transform.GetChild(2).gameObject;
        okButton.GetComponent<Button>().onClick.AddListener(CloseWindow);

        okAdded = true;

        skipButton = canvas.transform.GetChild(0).transform.GetChild(3).gameObject;
        skipButton.GetComponent<Button>().onClick.AddListener(CloseTutorial);

    }

    void OpenTutorial()
    {

        canvas = (GameObject)Instantiate(canvasPre);

        textHead = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        textHead.text = head[0];

        textMess = canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        textMess.text = message[0];

        counter = 0;

        tutorialStarted = true;
        
    }

    void CloseTutorial()
    {

        Destroy(canvas.gameObject);
        tutorialEnded = true;
        print(tutorialEnded);

    }

    void OpenWindow()
    {

        textHead = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        textHead.text = head[counter];

        textMess = canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        textMess.text = message[counter];

        canvas.transform.GetChild(0).gameObject.SetActive(true);
  

    }

    void CloseWindow()
    {

        canvas.transform.GetChild(0).gameObject.SetActive(false);

        counter++;

        if (counter < head.Count)
        {
            OpenWindow();
        }
        else
        {
            CloseTutorial();
        }

    }

}
