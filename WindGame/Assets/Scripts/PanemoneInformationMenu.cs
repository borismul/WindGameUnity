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
        OnMouseExit();
    }

    // Update is called once per frame
    void Update ()
    {
        if (turbine == null) return;

        float price = turbine.GetComponent<PriceController>().price;

        turbineName.text = turbine.turbineName;
        repairCosts.text = ((1 - turbine.health) * price).ToString("0");
        destroyRefund.text = (price/2 - (1 - turbine.health) * price/2).ToString("0");

        UpdateInfoElements();
        UpdateGlobalInfo();
    }

    void DestroyAllElements()
    {
        foreach (GameObject obj in turbineSpecificElements)
            Destroy(obj);

        foreach (GameObject obj in globalElements)
            Destroy(obj);

        foreach (GameObject obj in separators)
            Destroy(obj);

        turbineSpecificElements.Clear();
        globalElements.Clear();
        separators.Clear();
    }

    void CreateGlobalInfo()
    {
        separators.Add(CreateSeparator("Production Info:"));
        GameObject infoElement = CreateProperty("Efficiency", turbine.efficiency.ToString("F2"), "");
        infoElement.transform.GetChild(1).GetComponent<Text>().color = RYGInterpolation(1- turbine.efficiency);
        globalElements.Add(infoElement);

        GameObject infoElement2 = CreateProperty("Current Power Production", turbine.power.ToString("F0"), "W");
        globalElements.Add(infoElement2);

        GameObject infoElement3 = CreateProperty("Average Power Production", turbine.power.ToString("F0"), "W");
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
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.power.ToString("F0");

        infoElement = globalElements[2];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.avgPower.ToString("F0");

        infoElement = globalElements[3];
        infoElement.transform.GetChild(1).GetComponent<Text>().text = turbine.health.ToString("F2");
    }

    public void GenerateInfoElements()
    {
        separators.Add(CreateSeparator("User Variables:"));
        ObjectProperties properties = turbine.GetComponent<PropertiesController>().uniProperties;
        GenerateElements(properties);
        properties = turbine.GetComponent<PropertiesController>().specificProperties;
        GenerateElements(properties);

    }

    void GenerateElements(ObjectProperties properties)
    {
        foreach (FloatProperty prop in properties.floatProperty)
        {
            GameObject infoElement;
            if (prop.propertyValue < 1)
                infoElement = CreateProperty(prop.propertyName, prop.propertyValue.ToString("F2"), prop.unit);
            else if (prop.propertyValue < 50)
                infoElement = CreateProperty(prop.propertyName, prop.propertyValue.ToString("F1"), prop.unit);
            else
                infoElement = CreateProperty(prop.propertyName, prop.propertyValue.ToString("F0"), prop.unit);

            turbineSpecificElements.Add(infoElement);
        }
        foreach (IntProperty prop in properties.intProperty)
        {
            GameObject infoElement = CreateProperty(prop.propertyName, prop.propertyValue.ToString("0"), prop.unit);
            turbineSpecificElements.Add(infoElement);

        }
        foreach (MinMaxFloatProperty prop in properties.minMaxProperty)
        {

            GameObject infoElement;
            if (prop.minPropertyValue < 1)
                infoElement = CreateProperty(prop.minPropertyName, prop.minPropertyValue.ToString("F2"), prop.unit);
            else if (prop.minPropertyValue < 50)
                infoElement = CreateProperty(prop.minPropertyName, prop.minPropertyValue.ToString("F1"), prop.unit);
            else
                infoElement = CreateProperty(prop.minPropertyName, prop.minPropertyValue.ToString("F0"), prop.unit);

            turbineSpecificElements.Add(infoElement);


            if (prop.maxPropertyValue < 1)
                infoElement = CreateProperty(prop.maxPropertyName, prop.maxPropertyValue.ToString("F2"), prop.unit);
            else if (prop.maxPropertyValue < 50)
                infoElement = CreateProperty(prop.maxPropertyName, prop.maxPropertyValue.ToString("F1"), prop.unit);
            else
                infoElement = CreateProperty(prop.maxPropertyName, prop.maxPropertyValue.ToString("F0"), prop.unit);

            turbineSpecificElements.Add(infoElement);
        }
        foreach (BoolProperty prop in properties.boolProperty)
        {
            GameObject infoElement;
            if (prop.propertyValue)
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
        ObjectProperties properties = turbine.GetComponent<PropertiesController>().uniProperties;
        UpdateElements(properties, count, out count);
        properties = turbine.GetComponent<PropertiesController>().specificProperties;
        UpdateElements(properties, count, out count);

    }

    void UpdateElements(ObjectProperties properties, int count, out int updateCount)
    {
        foreach (FloatProperty prop in properties.floatProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];

            if (prop.propertyValue < 1)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.propertyValue.ToString("F2");
            else if (prop.propertyValue < 50)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.propertyValue.ToString("F1");
            else
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.propertyValue.ToString("F0");
            count++;
        }
        foreach (IntProperty prop in properties.intProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];
            infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.propertyValue.ToString("0");
            count++;
        }
        foreach (MinMaxFloatProperty prop in properties.minMaxProperty)
        {

            GameObject infoElement = turbineSpecificElements[count];
            if (prop.minPropertyValue < 1)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.minPropertyValue.ToString("F2");
            else if (prop.minPropertyValue < 50)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.minPropertyValue.ToString("F1");
            else
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.minPropertyValue.ToString("F0");

            turbineSpecificElements.Add(infoElement);

            count++;
            infoElement = turbineSpecificElements[count];

            if (prop.maxPropertyValue < 1)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.maxPropertyValue.ToString("F2");
            else if (prop.maxPropertyValue < 50)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.maxPropertyValue.ToString("F1");
            else
                infoElement.transform.GetChild(1).GetComponent<Text>().text = prop.maxPropertyValue.ToString("F0");

            turbineSpecificElements.Add(infoElement);
            count++;
        }
        foreach (BoolProperty prop in properties.boolProperty)
        {
            GameObject infoElement = turbineSpecificElements[count];

            if (prop.propertyValue)
                infoElement.transform.GetChild(1).GetComponent<Text>().text = "Yes";
            else
            {
                infoElement.transform.GetChild(1).GetComponent<Text>().text = "No";
            }
            count++;
        }

        updateCount = count;

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
        GameResources.removeWealth((float)turbine.health * turbine.GetComponent<PriceController>().price);
    }

    void DestroyTurbine()
    {
        //float price = GetComponent<PriceController>().price;
        //GameResources.removeWealth(-(price - (1 - (float)turbine.health) * price/2));
        TurbineManager.GetInstance().RemoveTurbine(turbine.gameObject);
        CloseMenu();
    }

    void CloseMenu()
    {
        UIManager.instance.CloseTurbineMenu();
    }

    public void OnMouseOver()
    {
        PointerInfo.overUIElement = true;
        PointerInfo.inScrollableArea = true;

    }

    public void OnMouseExit()
    {
        PointerInfo.overUIElement = false;
        PointerInfo.inScrollableArea = false;

    }
}
