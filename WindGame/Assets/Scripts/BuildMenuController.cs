using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildMenuController : MonoBehaviour
{
    public GameObject[] turbines;
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

    public Text infoText;

    GameObject curSelected;

    GameObject curInstantiated;

    bool canCancel;

    public float cutOffRadius;

    void Start()
    {
        cancelButton.onClick.AddListener(Cancel);
        buildButton.onClick.AddListener(BuildButton);
        LoadTurbines();
        canCancel = true;
    }

    void Update()
    {
        ClickOutside();
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
            Destroy(RadialMenuController.buildMenu);
    }

    void LoadTurbines()
    {
        for (int i = 0; i < turbines.Length; i++)
        {
            int index = i;
            Button turbBut = Instantiate(turbineButton);
            turbBut.transform.SetParent(overviewPanel.transform);
            turbBut.gameObject.GetComponentInChildren<Text>().text = turbines[i].name;
            turbBut.transform.localScale = Vector3.one;
            turbBut.onClick.AddListener(delegate { LoadTurbineButton(index); });
        }
        
    }

    void LoadTurbineButton(int index)
    {
        if (curInstantiated != null)
            Destroy(curInstantiated);

        nameText.text = turbines[index].name;
        infoText.text = turbineText[index].text;
        curSelected = turbines[index];
        curInstantiated = (GameObject)Instantiate(curSelected, instantHere.transform.position, Quaternion.identity);
        curInstantiated.transform.SetParent(instantHere.transform);
    }

    void LoadOthers()
    {
        for (int i = 0; i < others.Length; i++)
        {
            int index = i;
            Button turbBut = Instantiate(turbineButton);
            turbBut.transform.SetParent(overviewPanel.transform);
            turbBut.gameObject.GetComponentInChildren<Text>().text = turbines[i].name;
            turbBut.transform.localScale = Vector3.one;
            print(i);
            turbBut.onClick.AddListener(delegate { LoadOthersButton(index); });
        }
    }

    void LoadOthersButton(int index)
    {
        if (curInstantiated != null)
            Destroy(curInstantiated);

        nameText.text = others[index].name;
        infoText.text = othersText[index].text;
        curSelected = others[index];
        curInstantiated = (GameObject)Instantiate(curSelected, instantHere.transform.position, Quaternion.identity);
        curInstantiated.transform.SetParent(instantHere.transform);
    }

    void BuildButton()
    {
        if (curSelected == null)
            return;

        Destroy(curInstantiated);

        canCancel = false;
        GetComponentInChildren<CanvasGroup>().alpha = 0;
        GetComponentInChildren<CanvasGroup>().blocksRaycasts = false;
        infoCamera.enabled = false;
        InvokeRepeating("UpdateSelectedPosition", 0, 1/60f);
    }

    void UpdateSelectedPosition()
    {
        Vector3 plantPos = Vector3.zero;
        GridTile plantGrid = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            plantGrid = Grid.FindClosestGridTile(hit.point);
            plantPos = plantGrid.position;
            if (curInstantiated == null)
            {
                curInstantiated = (GameObject)Instantiate(curSelected, plantPos, Quaternion.identity);

            }
            else
                curInstantiated.transform.position = plantPos;

            if (plantGrid.canBuild && plantGrid.occupant == null)
            {
                foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
                {
                    ren.material.shader = Shader.Find("Transparent/Diffuse");
                    ren.material.color = new Color(0, 0.8f, 1, 0.5f);
                }
            }
            else
            {
                foreach (Renderer ren in curInstantiated.GetComponentsInChildren<Renderer>())
                {
                    ren.material.shader = Shader.Find("Transparent/Diffuse");
                    ren.material.color = new Color(1, 0, 0, 0.5f);
                }
            }
        }
        if (Input.GetMouseButtonDown(0) && plantGrid.canBuild && plantGrid.occupant == null)
        {
            Destroy(curInstantiated);
            BuildNow(plantGrid, plantPos);
            curInstantiated = null;
            Destroy(gameObject.transform.parent.gameObject);
            CancelInvoke("UpdateSelectedPosition");
        }
    }

    void BuildNow(GridTile plantGrid, Vector3 plantPos)
    {
        curInstantiated = (GameObject)Instantiate(curSelected, plantPos, Quaternion.identity);
        Grid.SetOccupant(plantGrid, curInstantiated, cutOffRadius);
    }



    
}
