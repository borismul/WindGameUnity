using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanemoneInformationMenu : MonoBehaviour {

    TurbineController turbine;

    public Text numberBlades;
    public Text powerDelivered;
    public Text costOfMaintenance;
    public Text health;
    public Text costOfRepair;
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

        numberBlades.text = turbine.nrBlades.ToString("0");
        powerDelivered.text = turbine.power.ToString("0");
        costOfMaintenance.text = turbine.costOfMaintenance.ToString("0");
        health.text = turbine.health.ToString("0");
        costOfRepair.text = turbine.repairCosts.ToString("0");
    }

    void onEnable ()
    {
        
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
        
    }

    void DestroyTurbine()
    {

    }

    void CloseMenu()
    {
        UIScript.GetInstance().CloseTurbineMenu();
    }
}
