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
    // Turbine prefabs, turbine preview prefabs and their texts
    GameObject[] turbines;
    Text[] turbineText;

    public GameObject[] others;
    public Text[] othersText;

    // Buttons on the build Menu
    public Button turbineButton;        // Loads a turbine preview on the right side of the menu
    public Button cancelButton;         // cancels the build menu
    public Button buildButton;          // builds the current selected turbine

    // Text fields on the build menu
    public Text nameText;               // Name textfield of the current selected turbine
    public Text infoText;               // Info textfield of the current selected turbine
    public Text buildPrice;             // Buildprice of the turbine in the current settings

    public GameObject overviewPanel;

    // preview camera and the location where the preview should be spawned
    public GameObject instantHere;
    public Camera infoCamera;

    // Features panel of the current selected turbine
    public GameObject buildingFeatures;

    // Raycast mask that only sees the terrain. Used to instantiate the preview of the turbine in blue or red.
    public LayerMask buildMask;

    // Error textfield that can be faded. Used to say that the player does not have enough money.
    public FadableText errorText;

    // Rotation speed of the preview turbine.
    public float rotateSpeed = 10;

    // Turbine properties menu
    public GameObject turbineProperties;
    public GameObject floatSliderPrefab;
    public GameObject intSliderPrefab;
    public GameObject boolPropertyPrefab;
    public GameObject minMaxPropertyPrefab;

    public ScrollRect propertiesScroller;

    public Material blueBuildMaterial;
    public Material redBuildMaterial;

    List<Color> originalMaterial = new List<Color>();

    List<FloatPropertyController> floatProperties = new List<FloatPropertyController>();
    List<IntPropertyController> intProperties = new List<IntPropertyController>();
    List<BoolPropertyController> boolProperties = new List<BoolPropertyController>();
    List<MinMaxController> minMaxProperties = new List<MinMaxController>();

    // Used to check wheter a turbine is selected.
    bool isTurbine;

    List<Button> menuButtons = new List<Button>();
    GameObject curSelected;

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

        turbines = TurbineManager.GetInstance().turbinePrefabs;
        turbineText = TurbineManager.GetInstance().turbineText;
        LoadTurbines();
        LoadOthers();
        canCancel = true;

        world = WorldController.GetInstance();

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
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
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !EventSystem.current.IsPointerOverGameObject())
        {
            Cancel();
        }
    }

    void Cancel()
    {
        if (canCancel)
        {
            Destroy(curInstantiated);
            infoCamera.enabled = false;
            curSelected = null;
            UIScript.GetInstance().SetInBuildMode(false);
            Camera.main.GetComponent<CameraController>().SetHaveControl(true);
            instantHere.SetActive(false);
            UIScript.GetInstance().CloseBuildMenu();
        }
    }

    void LoadTurbines()
    {
        for (int i = 0; i < TurbineManager.GetInstance().turbinePrefabs.Length; i++) // turbines array is filled through the prefab, wouldn't it be better if built through a config file?
        {
            //For each turbine, load a button for the user to select one.
            int index = i;
            Button turbBut = Instantiate(turbineButton); // Creates the button
            turbBut.transform.SetParent(overviewPanel.transform);
            turbBut.gameObject.GetComponentInChildren<Text>().text = TurbineManager.GetInstance().turbinePrefabs[i].name;
            turbBut.transform.localScale = Vector3.one;
            turbBut.onClick.AddListener(delegate { LoadTurbineButton(index); }); // Subscribes a method to the button for when a turbine is selected
            menuButtons.Add(turbBut); 
        }

    }

    void LoadOthers()
    {
        for (int i = 0; i < others.Length; i++) // turbines array is filled through the prefab, wouldn't it be better if built through a config file?
        {
            //For each turbine, load a button for the user to select one.
            int index = i;
            Button otherBut = Instantiate(turbineButton); // Creates the button
            otherBut.transform.SetParent(overviewPanel.transform);
            otherBut.gameObject.GetComponentInChildren<Text>().text = others[i].name;
            otherBut.transform.localScale = Vector3.one;
            otherBut.onClick.AddListener(delegate { LoadOtherButton(index); }); // Subscribes a method to the button for when a turbine is selected
            menuButtons.Add(otherBut);
        }
    }

    void LoadTurbineButton(int index)
    {
        if (curSelected != null && curSelected.GetInstanceID() == turbines[index].GetInstanceID())
            return;

        buildingFeatures.SetActive(true);
        isTurbine = true;   // Okay, some value is now true.
        if (curInstantiated != null)
            Destroy(curInstantiated); // If we have an object on curInstantiated, destroy it

        // Grab some information from the selected turbine
        nameText.text = turbines[index].GetComponent<TurbineController>().turbineName;
        infoText.text = turbineText[index].text;
        curSelected = turbines[index];

        // Instantiate this turbine in preview window
        curInstantiated = (GameObject)Instantiate(curSelected);
        curInstantiated.transform.position = instantHere.transform.position;
        curInstantiated.transform.SetParent(instantHere.transform);
        DestroyProperties();
        CreateProperties();
        Canvas.ForceUpdateCanvases();
        propertiesScroller.verticalScrollbar.value = 1;
        propertiesScroller.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();
        instantHere.SetActive(true);
    }

    void LoadOtherButton(int index)
    {
        if (curSelected != null && curSelected.GetInstanceID() == others[index].GetInstanceID())
            return;

        buildingFeatures.SetActive(true);
        isTurbine = false; 
        if (curInstantiated != null)
            Destroy(curInstantiated); // If we have an object on curInstantiated, destroy it

        // Grab some information from the selected turbine
        nameText.text = others[index].name;
        infoText.text = othersText[index].text;
        curSelected = others[index];

        // Instantiate this turbine in preview window
        curInstantiated = (GameObject)Instantiate(curSelected);
        curInstantiated.transform.position = instantHere.transform.position;
        curInstantiated.transform.SetParent(instantHere.transform);
        DestroyProperties();
        CreateProperties();
        Canvas.ForceUpdateCanvases();
        propertiesScroller.verticalScrollbar.value = 1;
        propertiesScroller.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();
        instantHere.SetActive(true);
    }

    void DestroyProperties()
    {
        foreach (FloatPropertyController controller in floatProperties)
            Destroy(controller.gameObject);

        foreach (IntPropertyController controller in intProperties)
            Destroy(controller.gameObject);

        foreach (BoolPropertyController controller in boolProperties)
            Destroy(controller.gameObject);

        foreach(MinMaxController controller in minMaxProperties)
            Destroy(controller.gameObject);


        floatProperties.Clear();
        intProperties.Clear();
        boolProperties.Clear();
        minMaxProperties.Clear();
    }

    void CreateProperties()
    {
        foreach (FloatProperty floatProperty in curInstantiated.GetComponent<PropertiesContainer>().properties.floatProperty)
        {
            GameObject floatSlider = (GameObject)Instantiate(floatSliderPrefab);
            floatSlider.transform.SetParent(turbineProperties.transform, false);
            floatSlider.GetComponent<FloatPropertyController>().floatProperty = floatProperty;
            floatProperties.Add(floatSlider.GetComponent<FloatPropertyController>());
        }

        foreach (IntProperty intProperty in curInstantiated.GetComponent<PropertiesContainer>().properties.intProperty)
        {
            GameObject intSlider = (GameObject)Instantiate(intSliderPrefab);
            intSlider.transform.SetParent(turbineProperties.transform, false);
            intSlider.GetComponent<IntPropertyController>().intProperty = intProperty;
            intProperties.Add(intSlider.GetComponent<IntPropertyController>());
        }

        foreach (MinMaxFloatProperty minMaxProperty in curInstantiated.GetComponent<PropertiesContainer>().properties.minMaxProperty)
        {
            GameObject minMaxSlider = (GameObject)Instantiate(minMaxPropertyPrefab);
            minMaxSlider.transform.SetParent(turbineProperties.transform, false);
            minMaxSlider.GetComponent<MinMaxController>().minMaxFloatProperty = minMaxProperty;
            minMaxProperties.Add(minMaxSlider.GetComponent<MinMaxController>());
        }

        foreach (BoolProperty boolProperty in curInstantiated.GetComponent<PropertiesContainer>().properties.boolProperty)
        {
            GameObject boolSlider = (GameObject)Instantiate(boolPropertyPrefab);
            boolSlider.transform.SetParent(turbineProperties.transform, false);
            boolSlider.GetComponent<BoolPropertyController>().boolProperty = boolProperty;
            boolProperties.Add(boolSlider.GetComponent<BoolPropertyController>());
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

    // This function is called when the user clicks on the 'Build' button
    void BuildButton()
    {
        if (curSelected == null)
            return;

        if (!GameResources.CanIBuy(curSelected.GetComponent<PriceController>().price))
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
        BuildMenu2BuildMode();
    }

    void BuildMenu2BuildMode()
    {
        canCancel = false;
        GetComponentInChildren<CanvasGroup>().alpha = 0;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
        infoCamera.enabled = false;
        UIScript.GetInstance().SetInBuildMode(true);

        originalMaterial.Clear();
        foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in ren.materials)
            {
                Color col = mat.color;
                originalMaterial.Add(col);
            }
        }

        curInstantiated.transform.SetParent(null);
    }

    void BuildMode2BuildMenu()
    {
        Destroy(curInstantiated);
        canCancel = true;
        GetComponentInChildren<CanvasGroup>().alpha = 1;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts = true;
        infoCamera.enabled = true;
        UIScript.GetInstance().SetInBuildMode(false);
        curInstantiated = (GameObject)Instantiate(curSelected);
        curInstantiated.transform.position = instantHere.transform.position;
        curInstantiated.transform.SetParent(instantHere.transform);
        DestroyProperties();
        CreateProperties();
        Canvas.ForceUpdateCanvases();
        propertiesScroller.verticalScrollbar.value = 1;
        propertiesScroller.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();

    }

    void BuildPriceColorUpdate()
    {
        if (curSelected == null)
            return;

        curInstantiated.GetComponent<PriceController>().CalculateCost();
        buildPrice.text = curInstantiated.GetComponent<PriceController>().price.ToString();

        if (GameResources.CanIBuy(curInstantiated.GetComponent<PriceController>().price))
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
        Camera.main.GetComponent<CameraController>().SetHaveControl(true);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildMask))
        {
            plantGrid = GridTile.FindClosestGridTile(hit.point); // Grab the grid where we're hitting
            plantPos = plantGrid.position; // What is the x,y,z coords?

            if((!curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild || (curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild && !Input.GetMouseButton(1))))
                curInstantiated.transform.position = plantPos; // We already have a preview turbine, just update it's position to follow the mouse

            if (mouseX * rotateSpeed >= 1 || mouseX * rotateSpeed <= -1)
            {
                curInstantiated.transform.rotation = Quaternion.Euler(curInstantiated.transform.rotation.eulerAngles.x, Mathf.RoundToInt(curInstantiated.transform.rotation.eulerAngles.y + mouseX * rotateSpeed), curInstantiated.transform.rotation.eulerAngles.z);
                mouseX = 0;
            }
            

            if ((!curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild || (curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild && !Input.GetMouseButton(1))))
            {
                if (world.CanBuild(plantPos, curInstantiated.GetComponent<SizeController>().diameter * curInstantiated.GetComponent<SizeController>().desiredScale, true)) // If we can build here, make the color greenish
                {
                    foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
                    {
                        foreach (Material mat in ren.materials)
                        {
                            mat.shader = blueBuildMaterial.shader;
                            mat.color = blueBuildMaterial.color;
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
                            mat.shader = redBuildMaterial.shader;
                            mat.color = redBuildMaterial.color;
                        }
                    }
                    canBuild = false;
                }
            }
        }

        if (curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild && Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * Time.deltaTime;
        }
        else if(curInstantiated.GetComponent<BuildAttributes>().canRotateAtBuild && Input.GetMouseButtonUp(1))
        {
            mouseX = 0;
        }
        if (Input.GetMouseButtonDown(0) && canBuild) // The user clicks and we can build here
        {
            BuildNow(plantGrid, plantPos); // Run the build function
            infoCamera.enabled = false;
            curSelected = null;
            UIScript.GetInstance().SetInBuildMode(false);
            instantHere.SetActive(false);
            gameObject.transform.parent.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BuildMode2BuildMenu();
        }
    }

    void BuildNow(GridTile plantGrid, Vector3 plantPos)
    {
        if (isTurbine) // If we want to build a turbine...
        {
            curInstantiated.GetComponent<TurbineController>().enabled = true;
            world.AddTurbine(curInstantiated, plantPos, curInstantiated.transform.rotation, curInstantiated.GetComponent<SizeController>().desiredScale, GridTileOccupant.OccupantType.Turbine, TurbineManager.GetInstance().transform); // Let the world controller know we want to build this thing
        }
        else
        {
            world.AddOther(curInstantiated, plantPos, curInstantiated.transform.rotation, curInstantiated.GetComponent<SizeController>().desiredScale, GridTileOccupant.OccupantType.Other, TerrainController.thisTerrainController.transform);
        }
        GameResources.Buy(curInstantiated.GetComponent<PriceController>().price);
        Renderer[] rens = curInstantiated.GetComponentsInChildren<Renderer>();
            Renderer[] rensPrefab = curSelected.GetComponentsInChildren<Renderer>();
            int count = 0;
            foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
            {
                foreach (Material mat in ren.materials)
                {
                    mat.shader = Shader.Find("Standard");
                    mat.color = originalMaterial[count];
                    count++;

                }
            }
            curInstantiated = null;
        
    }

    public void OnMouseEnter()
    {
        PointerInfo.inScrollableArea = true;
        PointerInfo.overUIElement = true;
    }

    public void OnMouseExit()
    {
        PointerInfo.inScrollableArea = false;
        PointerInfo.overUIElement = false;
    }
}
