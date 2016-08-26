using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

// The class handles the different features and phases of the tutorial. It creates the tutorial canvas and opens pop up messages.
// Moreover, it manages the time frames where the player has control to try out what is presented in the tutorial.

public class TutorialScript : MonoBehaviour {

    GameObject canvas;          // tutorial canvas
    GameObject okButton;        // button to proceed
    GameObject skipButton;      // button to skip all the rest of the tutorial

    List<string> head;          // windows headings
    List<string> message;       // messages and instructions to the player
    List<string> handle;        // tells Handle() whether the next phase is a descriptive or an interactive one

    List<Action> interactions;  // functions managing the interactive parts

    List<bool> checkList;   // keeps track of the accomplished tutorial tasks

    Text textHead;          // text of the header
    Text textMess;          // text of the message box

    int counter1;       // takes count of the tutorial phases through the list handle
    int counter2;       // takes count of the tutorial windows through the lists head and message
    int counter3;       // takes count of the tutorial interactions through the list interactions

    string kind;        // tells Handle() whether a window or an interaction part has to run

    float xInput;       // left and right arrow key inputs
    float zInput;       // up and down arrow key inputs
    float scrollInput;

    bool tutorialStarted;       // the tutorial has started
    bool tutorialEnded;         // the tutorial is complete
    bool playerTime;            // the player has controls to interact the tutorial



    // Use this for initialization
    void Start () {

        handle = new List<string>();

        handle.Add("window");               // welcome
        handle.Add("window");               // introduction to camera controls
        handle.Add("interact");             // try controls
        handle.Add("window");               // good job
        handle.Add("window");               // open build menu
        handle.Add("interact");             // try the build menu
        handle.Add("window");               // good job
        handle.Add("window");               // wind pole
        handle.Add("interact");             // try build a wind pole
        handle.Add("window");               // good job
        handle.Add("window");               // open building info
        handle.Add("interact");             // try open pole info
        handle.Add("window");               // wind rose
        handle.Add("window");               // good job
        handle.Add("window");               // again build menu
        handle.Add("interact");             // try open build menu again
        handle.Add("window");               // wind turbine
        handle.Add("interact");             // try build wind turbine
        handle.Add("window");               // good job
        handle.Add("window");               // turbine info
        handle.Add("interact");             // try open turbine info
        handle.Add("window");               // a storm is coming

        head = new List<string>();

        head.Add("Welcome farmer!");
        head.Add("Camera controls");
        head.Add("Good job!");
        head.Add("Build menu");
        head.Add("Good job!");
        head.Add("Build your wind pole");
        head.Add("Good job!");
        head.Add("The wind rose");
        head.Add("The wind rose");
        head.Add("Good job!");
        head.Add("Your first wind turbine");
        head.Add("Your first wind turbine");
        head.Add("Good job!");
        head.Add("Turbine info");
        head.Add("A storm is coming");

        message = new List<string>();

        message.Add("Welcome to Windgame! In the first mission you will be a farmer of ancient Persia and your objective is to " + 
                    "mill as much grain as possible for your population. But before letting you play around with turbines let's " +
                    "see a few features of the game together. You'll have your time to try all things out and get confident with them! " +
                    "Oh and by the way, if you already know about the game you can always press the Skip button in the bottom right " +
                    "corner of the window to skip this tutorial. Press Ok if you want to see what's next.");
        message.Add("So let's start off with some simple controls to see the world around you. Try to play around with clicking the " + 
                    "middle mouse button and dragging to rotate your camera. Pressing the directional keys will let you move around " +
                    "the world. Then you can zoom in and out by scrolling up and down. Come on, check them out now!");
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
        message.Add("Very well. Now open up the information tab of your wind pole by clicking on it. You will be able to do this with " +
                    "all your buildings.");
        message.Add("Here we are. In this window you will always see all the information relevant to your building. In this case we " +
                    "can see a wind rose, which tells us about direction and intensity of the wind in that spot. Does this sound a " +
                    "bit confusing? Don't worry! Remember that you can always go check out the lecture slides to clear things up!");
        message.Add("Well done so far! Now it's time to build a windmill!");
        message.Add("Open up the build menu as we did previously. You will find that something has changed.");
        message.Add("Here we go. As you can see you can only build a windmill right now. So go on and build one! Oh, one more thing. " +
                    "You may want to orientate it properly, according to what you saw from the wind rose before. Confused? " +
                    "It could be a good idea to review the lecture slides then!");
        message.Add("Well done! Let's proceed to the next part with the Ok button.");
        message.Add("Now that you have your turbine you can click on it to gather information about its working. You will find " +
                    "several features including production, costs, controls and upgrading options. Check it out!");
        message.Add("The people here are talking of an approaching storm. It could be bad news for your beautiful turbine. " +
                    "Are you planning on doing anything about that?");

        interactions = new List<Action>();

        interactions.Add(TryCamera);
        interactions.Add(Build1);
        interactions.Add(Pole);
        interactions.Add(Rose);
        interactions.Add(Build2);
        interactions.Add(Turbine);
        interactions.Add(Info);

        checkList = new List<bool>(new bool[6]);

        counter1 = 0;       // initialize list counters
        counter2 = 0;
        counter3 = 0;

        canvas = gameObject;

        OpenTutorial();

	}
	
	// Update is called once per frame
	void Update () {

        if (playerTime)
        {
            
            interactions[counter3]();

        }

    }

    // My methods //

    void OpenTutorial()
    {

        textHead = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();     // initialize texts and buttons
        textHead.text = head[0];

        textMess = canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        textMess.text = message[0];

        okButton = canvas.transform.GetChild(0).transform.GetChild(2).gameObject;
        okButton.GetComponent<Button>().onClick.AddListener(CloseWindow);

        skipButton = canvas.transform.GetChild(0).transform.GetChild(3).gameObject;
        skipButton.GetComponent<Button>().onClick.AddListener(CloseTutorial);

        tutorialStarted = true;
                
    }

    void CloseTutorial()                        // TUTORIAL IS CLOSED BY ITSELF!! SHOULD IT BE CLOSED FROM THE UI MANAGER???
    {

        Destroy(canvas.gameObject);
        tutorialEnded = true;

    }

    void Handle()       // determines whether a window or interactive part has to run, then calls it
    {
        if (counter1 > handle.Count-1)  // close tutorial when last element of the list handle has been trespassed
        {
            CloseTutorial();
        }
        else
        {
            kind = handle[counter1];

            if (kind == "window")
            {
                OpenWindow();
            }
            else
            {
                playerTime = true;
            }
        }
    }

    void OpenWindow()       // re-activate the pop up window with new content when Handle() asks for that
    {

        textHead = canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();     
        textHead.text = head[counter2];

        textMess = canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        textMess.text = message[counter2];

        canvas.transform.GetChild(0).gameObject.SetActive(true);

    }

    void CloseWindow()      // runs when the player clicks the OK button
    {

        canvas.transform.GetChild(0).gameObject.SetActive(false);

        counter1++;
        counter2++;

        Handle();

    }

    // Interactive phases managers

    void TryCamera()
    {

        xInput = Input.GetAxis("Horizontal");               // STILL MISSING THE PANNING!!!
        zInput = Input.GetAxis("Vertical");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (xInput > 0)
        {
            checkList[0] = true;
        }
        else if (xInput < 0)
        {
            checkList[1] = true;
        }

        if (zInput > 0)
        {
            checkList[2] = true;
        }
        else if (zInput < 0)
        {
            checkList[3] = true;
        }

        if (scrollInput > 0)
        {
            checkList[4] = true;
        }
        else if (scrollInput < 0)
        {
            checkList[5] = true;
        }

        if (checkList[0] & checkList[1] & checkList[2] & checkList[3] & checkList[4] & checkList[5])
        {
            playerTime = false;         // always perform these actions when the tasks are accomplished
            counter1++;
            counter3++;
            checkList = new List<bool>(new bool[6]);
            Handle();
        }

    }

    void Build1()
    {

        // maybe look for the menu as a child of UI?
        // much better: access to global variables keeping track of the buildings/objects in the world

        playerTime = false;         
        counter1++;
        counter3++;
        Handle();
    }

    void Pole()
    {
        playerTime = false;
        counter1++;
        counter3++;
        Handle();
    }

    void Rose()
    {
        playerTime = false;
        counter1++;
        counter3++;
        Handle();
    }

    void Build2()
    {
        playerTime = false;
        counter1++;
        counter3++;
        Handle();
    }

    void Turbine()
    {
        playerTime = false;
        counter1++;
        counter3++;
        Handle();
    }

    void Info()
    {
        playerTime = false;
        counter1++;
        counter3++;
        Handle();
    }

}
