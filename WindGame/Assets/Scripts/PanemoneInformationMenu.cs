using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanemoneInformationMenu : MonoBehaviour {

    TurbineController turbine;

    public GameObject detailsPanel;
    public Text turbineName;
    public Text repairCosts;
    public Text destroyRefund;
    public Button destroyButton;
    public Button repairButton;
    public Button closeButton;
    public ScrollRect propertiesScroller;

    public GameObject infoElementPrefab;
    public GameObject separatorPrefab;

    List<GameObject> turbineSpecificElements = new List<GameObject>();
    List<GameObject> globalElements = new List<GameObject>();
    List<GameObject> separators = new List<GameObject>();


    void Start()
    {
        repairButton.onClick.AddListener(RepairTurbine);
        destroyButton.onClick.AddListener(DestroyTurbine);
        closeButton.onClick.AddListener(CloseMenu);
    }
    // Use this for initialization
    void OnEnable()
    {
        if (turbine == null)
            return;

        CreateGlobalInfo();
        GenerateInfoElements();

        Canvas.ForceUpdateCanvases();
        propertiesScroller.verticalScrollbar.value = 1;
        propertiesScroller.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();
    }

    void OnDisable()
    {
        DestroyAllElements();
    }

    // Update is called once per frame
    void Update ()
    {
        if (turbine == null) return;

        turbineName.text = turbine.turbineName;
        repairCosts.text = ((1 - turbine.health) * turbine.price).ToString("0");
        destroyRefund.text = (turbine.price/2 - (1 - turbine.health) * turbine.price/2).ToString("0");

        UpdateInfoElements();
        UpdateGlobalInfo();
    }

    void DestroyAllElements()
    {
        foreach (GameObject obj in turbineSpecificElements)
            Destroy(obj);

        foreach (GameObject obj in globalElements)
            Destroy(obj);

        turbineSpecificElements.Clear();
        globalElements.Clear();
    }

    void CreateGlobalInfo()
    {
        CreateSeparator("Production Info:");
        GameObject infoElement = CreateProperty("Efficiency", turbine.efficiency.ToString("F2"), "");
        infoElement.transform.GetChild(1).GetComponent<Text>().color = RYGInterpolation(1- turbine.efficiency);
        globalElements.Add(infoElement);

        GameObject infoElement2 = CreateProperty("Current Power Production", turbine.power.ToString("F2"), "W");
        globalElements.Add(infoElement2);

        GameObject infoElement3 = CreateProperty("Average Power Production", turbine.power.ToString("F2"), "W");
        globalElements.Add(infoElement3);

        GameObject infoElement4 = CreateProperty("Health", turbine.health.ToString("F2"), "");
        globalElements.Add(infoElement4);
    }

    void UpdateGlobalInfo()
    {
        GameObject infoElement = globalElements[0];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.efficiency.ToString("F2");
        infoElement.transform.GetChild(1).GetComponent<Text>().color = RYGInterpolation(1 - turbine.efficiency);

        infoElement = globalElements[1];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.power.ToString("F2");

        infoElement = globalElements[2];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.avgPower.ToString("F2");

        infoElement = globalElements[3];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.health.ToString("F2");
    }

    public void GenerateInfoElements()
    {
        CreateSeparator("User Variables:");

        foreach (FloatProperty prop in turbine.turbineProperties.floatProperty)
        {
            GameObject infoElement = CreateProperty(prop.propertyName, prop.property.ToString("F2"), prop.unit);
            turbineSpecificElements.Add(infoElement);
        }
        foreach (IntProperty prop in turbine.turbineProperties.intProperty)
        {
            GameObject infoElement = CreateProperty(prop.propertyName, prop.property.ToString("0"), prop.unit);
            turbineSpecificElements.Add(infoElement);

        }
        foreach (BoolProperty prop in turbine.turbineProperties.boolProperty)
        {
            GameObject infoElement;
            if (prop.property)
                infoElement = CreateProperty(prop.propertyName, "Yes", "");
            else
            {
                infoElement = CreateProperty(prop.propertyName, "No", "");
            }
            turbineSpecificElements.Add(infoElement);
        }
    }

    GameObject CreateProperty(string name, string value, string unit)
    {
        GameObject infoElement = (GameObject)Instantiate(infoElementPrefab);
        infoElement.transform.SetParent(detailsPanel.transform, false);
        infoElement.transform.GetChild(0).GetComponent<Text>().text = name + ":";
        infoElement.transform.GetChild(1).GetComponent<Text>().text = value;
        infoElement.transform.GetChild(2).GetComponent<Text>().text = unit;

        return infoElement;
    }

    GameObject CreateSeparator(string name)
    {
        GameObject separator = (GameObject)Instantiate(separatorPrefab);
        separator.transform.SetParent(detailsPanel.transform, false);
        separator.GetComponentInChildren<Text>().text = name;

        return separator;
    }

    public void UpdateInfoElements()
    {
        int count = 0;
        foreach (FloatProperty prop in turbine.turbineProperties.floatProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];
            infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.property.ToString("F2");
            count++;
        }
        foreach (IntProperty prop in turbine.turbineProperties.intProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];
            infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.property.ToString("0");
            count++;

        }
        foreach (BoolProperty prop in turbine.turbineProperties.boolProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];

            if (prop.property)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = "Yes";
            else
            {
                infoElement.transform.GetChild(1).GetComponent<Text>().text = "No";
            }
            count++;
        }
    }

    Color RYGInterpolation(float efficiency)
    {
        Color myColor = new Color(2.0f * efficiency, 2.0f * (1 - efficiency), 0);
        return myColor;
    }

    public void SetTurbine(TurbineController tur)
    {
        turbine = tur;
    }

    public void ClearTurbine()
    {
        turbine = null;
    }

    void RepairTurbine()
    {
        turbine.health = 1;
        GameResources.removeWealth((float)turbine.health * turbine.price);
    }

    void DestroyTurbine()
    {
        GameResources.removeWealth(-(turbine.price - (1 - (float)turbine.health) * turbine.price/2));
        TurbineManager.GetInstance().RemoveTurbine(turbine.gameObject);
        CloseMenu();
    }

    void CloseMenu()
    {
        UIScript.GetInstance().CloseTurbineMenu();
    }

    public void OnMouseOver()
    {
        PointerInfo.inScrollableArea = true;
    }

    public void OnMouseExit()
    {
        PointerInfo.inScrollableArea = false;
    }
}
