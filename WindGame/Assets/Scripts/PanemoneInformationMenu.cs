using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanemoneInformationMenu : MonoBehaviour {

    TurbineController turbine;

    public Text turbineName;
    public Text health;
    public Text avgPowerProduction;
    public Text curPowerProduction;
    public Text tipSpeedRatio;
    public Text bladePitch;
    public Text repairCosts;
    public Text destroyRefund;
    public Button destroyButton;
    public Button repairButton;
    public Button closeButton;

	// Use this for initialization
	void Start ()
    {
        repairButton.onClick.AddListener(RepairTurbine);
        destroyButton.onClick.AddListener(DestroyTurbine);
        closeButton.onClick.AddListener(CloseMenu);
    }

    // Update is called once per frame
    void Update ()
    {
        if (turbine == null) return;

        //turbineName.text = turbine.turbineName;
        //health.text = (turbine.health * 100).ToString("0") + "%";
        //avgPowerProduction.text = turbine.avgPower.ToString("0") + " W";
        //curPowerProduction.text = turbine.power.ToString("0") + " W";
        //tipSpeedRatio.text = turbine.TSR.ToString("0");
        //bladePitch.text = turbine.bladePitch.ToString("0") + "°";
        //repairCosts.text = ((1-turbine.health) * 2500).ToString("0");
        //destroyRefund.text = (2500 - (1-turbine.health) * 2500).ToString("0");
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
        GameResources.removeWealth((float)turbine.health * 2500);
    }

    void DestroyTurbine()
    {
        GameResources.removeWealth(-(2500 - (1 - (float)turbine.health) * 2500));
        TurbineManager.GetInstance().RemoveTurbine(turbine.gameObject);
        CloseMenu();
    }

    void CloseMenu()
    {
        UIScript.GetInstance().CloseTurbineMenu();
    }
}
