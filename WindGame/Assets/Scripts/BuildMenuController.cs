using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


/**
    The goal of this controller is to create the build menu.
    The build menu provides an interface to the user to select turbines and 
    characteristics for said turbines. 
    When the user is satisfied, he confirms his choices through the 'Build' button.
    The task of this controller is then to !!!!INFORM!!!! the WORLD that a new turbine 
    should be built with the given parameters.
    After completion of this task, the build menu is destroyed.
**/

public class BuildMenuController : MonoBehaviour
{
    // Some required class parameters
    public GameObject[] turbines;
    public GameObject[] turbinePreviews;
    public GameObject[] others;
    public Text[] turbineText;
    public Text[] othersText;
    public Button turbineButton;
    public GameObject overviewPanel;
    public Button cancelButton;
    public Button buildButton;
    public GameObject instantHere;
    public Camera infoCamera;
    public Text nameText;
    public Button loadTurbinesButton;
    public Button loadOthersButton;
    public Text infoText;
    public float cutOffRadius;
    public GameObject buildingFeatures;
    public LayerMask buildMask;
    public Slider TSRSlider;
    public Slider bladePitchSlider;
    public FadableText errorText;
    public Text buildPrice;
    public float rotateSpeed = 10;

    bool isTurbine;

    List<Button> menuButtons = new List<Button>();
    GameObject curSelected;
    GameObject previewSelected;

    GameObject curInstantiated;

    bool canCancel;

    WorldController world;

    float mouseX;

    void OnEnable()
    {
        GetComponentInChildren<CanvasGroup>().alpha = 1;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
        canCancel = true;
        infoCamera.enabled = true;
        buildingFeatures.SetActive(false);
    }
    // The start method gets called first
    void Start()
    {
        // Subscribe methods buttons
        cancelButton.onClick.AddListener(Cancel);
        buildButton.onClick.AddListener(BuildButton);
        LoadTurbines();

        canCancel = true;

        world = WorldController.GetInstance();

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    void Update()
    {
        // Check if the user is clicking outside the build menu
        ClickOutside();

        BuildPriceColorUpdate();
        if (UIScript.GetInstance().GetInBuildMode())
        {
            UpdateSelectedPosition();
        }


    }
    void ClickOutside()
    {
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !EventSystem.current.IsPointerOverGameObject())
        {
            Cancel();
        }
    }

    void Cancel()
    {
        if (canCancel)
        {
            gameObject.transform.parent.gameObject.SetActive(false);
            Destroy(curInstantiated);
            infoCamera.enabled = false;
        }
    }

    void LoadTurbines()
    {
        DeleteMenuButtons(); // What am I doing here?

        for (int i = 0; i < turbines.Length; i++) // turbines array is filled through the prefab, wouldn't it be better if built through a config file?
        {
            //For each turbine, load a button for the user to select one.
            int index = i;
            Button turbBut = Instantiate(turbineButton); // Creates the button
            turbBut.transform.SetParent(overviewPanel.transform);
            turbBut.gameObject.GetComponentInChildren<Text>().text = turbines[i].name;
            turbBut.transform.localScale = Vector3.one;
            turbBut.onClick.AddListener(delegate { LoadTurbineButton(index); }); // Subscribes a method to the button for when a turbine is selected
            menuButtons.Add(turbBut); 
        }

    }

    void LoadTurbineButton(int index)
    {
        buildingFeatures.SetActive(true);
        isTurbine = true;   // Okay, some value is now true.
        if (curInstantiated != null)
            Destroy(curInstantiated); // If we have an object on curInstantiated, destroy it

        // Grab some information from the selected turbine
        nameText.text = turbines[index].name;
        infoText.text = turbineText[index].text;
        curSelected = turbines[index];
        previewSelected = turbinePreviews[index];

        // Instantiate this turbine (for the preview window?)
        curInstantiated = (GameObject)Instantiate(previewSelected);
        curInstantiated.transform.position = instantHere.transform.position +  curInstantiated.transform.position;
        curInstantiated.transform.SetParent(instantHere.transform);
        curInstantiated.tag = "Respawn"; // To confirm that this turbine is created for the preview window
        buildPrice.text = curSelected.GetComponent<TurbineController>().price.ToString();

    }

    // Does the same thing as LoadTurbines(), but for other buildings
    void LoadOthers()
    {
        DeleteMenuButtons();
        for (int i = 0; i < others.Length; i++)
        {
            int index = i;
            Button turbBut = Instantiate(turbineButton);
            turbBut.transform.SetParent(overviewPanel.transform);
            turbBut.gameObject.GetComponentInChildren<Text>().text = others[i].name;
            turbBut.transform.localScale = Vector3.one;
            turbBut.onClick.AddListener(delegate { LoadOthersButton(index); });
            menuButtons.Add(turbBut);
        }
    }

    void DeleteMenuButtons()
    {
        // I suppose this function deletes all the selectable objects when a user switches back and forth between Turbines and Others
        foreach (Button menBut in menuButtons)
        {
            Destroy(menBut.gameObject);
        }
        menuButtons = new List<Button>();
    }

    // Same as LoadTurbinesButton() but for other buildings
    void LoadOthersButton(int index)
    {
        isTurbine = false;
        if (curInstantiated != null)
            Destroy(curInstantiated);

        nameText.text = others[index].name;
        infoText.text = othersText[index].text;
        curSelected = others[index];

        curInstantiated = (GameObject)Instantiate(curSelected, instantHere.transform.position, Quaternion.identity);
        curInstantiated.transform.SetParent(instantHere.transform);
        curInstantiated.transform.localScale = Vector3.one * 10;

    }

    // This function is called when the user clicks on the 'Build' button
    void BuildButton()
    {
        if (curSelected == null)
            return;

        if (!GameResources.CanIBuy(curSelected.GetComponent<TurbineController>().price))
        {
            if (errorText.enabled)
            {
                errorText.gameObject.SetActive(true);
                errorText.GetComponent<Text>().text = "Insufficient funds to buy a turbine.";
            }
            else
            {
                errorText.ReEnable();
            }

            return;
        }

        Destroy(curInstantiated);
        curInstantiated = null;

        canCancel = false;
        GetComponentInChildren<CanvasGroup>().alpha = 0;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
        infoCamera.enabled = false;
        UIScript.GetInstance().SetInBuildMode(true);
    }

    void BuildPriceColorUpdate()
    {
        if (curSelected == null)
            return;

        if (GameResources.CanIBuy(curSelected.GetComponent<TurbineController>().price))
            buildPrice.color = Color.green;
        else
            buildPrice.color = Color.red;
    }

    // Updates the turbine to the mouse cursor until placement is confirmed
    void UpdateSelectedPosition()
    {
        Vector3 plantPos = Vector3.zero; // Get zero vector
        GridTile plantGrid = null;  
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Raycast to find where the mouse is pointing at
        RaycastHit hit;
        bool canBuild = false;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildMask))
        {
            plantGrid = GridTile.FindClosestGridTile(hit.point); // Grab the grid where we're hitting
            plantPos = plantGrid.position; // What is the x,y,z coords?
            if (curInstantiated == null) // If we haven't already created a preview turbine
            {
                //Create a turbine and place it on the ground where the mouse cursor points to
                curInstantiated = (GameObject)Instantiate(curSelected);
                curInstantiated.transform.position = plantPos;
            }
            else
            {
                if(!curInstantiated.GetComponent<TurbineController>().canRotateAtBuild || (curInstantiated.GetComponent<TurbineController>().canRotateAtBuild && !Input.GetMouseButton(1)))
                    curInstantiated.transform.position = plantPos; // We already have a preview turbine, just update it's position to follow the mouse

                if (mouseX * rotateSpeed >= 1 || mouseX * rotateSpeed <= -1)
                {
                    curInstantiated.transform.rotation = Quaternion.Euler(curInstantiated.transform.rotation.eulerAngles.x, Mathf.RoundToInt(curInstantiated.transform.rotation.eulerAngles.y + mouseX * rotateSpeed), curInstantiated.transform.rotation.eulerAngles.z);
                    mouseX = 0;
                }
            }

            if (!curInstantiated.GetComponent<TurbineController>().canRotateAtBuild || (curInstantiated.GetComponent<TurbineController>().canRotateAtBuild && !Input.GetMouseButton(1)))
            {
                if (world.CanBuild(plantPos, 50, true)) // If we can build here, make the color greenish
                {
                    foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
                    {
                        foreach (Material mat in ren.materials)
                        {
                            mat.shader = Shader.Find("Transparent/Diffuse");
                            mat.color = new Color(0, 0.8f, 1, 0.5f);
                        }
                    }
                    canBuild = true;
                }
                else // We can't build here, make the color reddish
                {
                    foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
                    {
                        foreach (Material mat in ren.materials)
                        {
                            mat.shader = Shader.Find("Transparent/Diffuse");
                            mat.color = new Color(1, 0, 0, 0.5f);
                        }
                    }
                    canBuild = false;
                }
            }
        }

        if (curInstantiated.GetComponent<TurbineController>().canRotateAtBuild && Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * Time.deltaTime;
        }
        else if(curInstantiated.GetComponent<TurbineController>().canRotateAtBuild && Input.GetMouseButtonUp(1))
        {
            mouseX = 0;
        }
        if (Input.GetMouseButtonDown(0) && canBuild) // The user clicks and we can build here
        {
            Destroy(curInstantiated); // Destroy the preview turbine
            BuildNow(plantGrid, plantPos); // Run the build function
            gameObject.transform.parent.gameObject.SetActive(false);
            UIScript.GetInstance().SetInBuildMode(false);
        }
    }

    void BuildNow(GridTile plantGrid, Vector3 plantPos)
    {
        if (isTurbine) // If we want to build a turbine...
        {
            GameResources.BuyTurbine(curInstantiated.GetComponent<TurbineController>().price);
            world.AddTurbine(curSelected, plantPos, curInstantiated.transform.rotation, 1, GridTileOccupant.OccupantType.Turbine, TurbineManager.GetInstance().transform, TSRSlider.value, bladePitchSlider.value); // Let the world controller know we want to build this thing
        }
    }
}
