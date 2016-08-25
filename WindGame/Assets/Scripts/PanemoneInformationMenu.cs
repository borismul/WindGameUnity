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

	// Use this for initialization
	void Start ()
    {
        repairButton.onClick.AddListener(repairTurbine);
        destroyButton.onClick.AddListener(destroyTurbine);
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    void onEnable ()
    {
        numberBlades.text = turbine.nrBlades.ToString();
        powerDelivered.text = turbine.power.ToString();
        costOfMaintenance.text = turbine.costOfMaintenance.ToString();
        health.text = turbine.health.ToString();
        costOfRepair.text = turbine.repairCosts.ToString();
    }

    public void setTurbine(TurbineController tur)
    {
        turbine = tur;
    }

    public void clearTurbine()
    {
        turbine = null;
    }

    void repairTurbine()
    {
        
    }

    void destroyTurbine()
    {

    }
}
