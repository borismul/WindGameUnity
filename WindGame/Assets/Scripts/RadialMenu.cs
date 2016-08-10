using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour {

    public Button buildButton;
    public GameObject buildMenu;
    GameObject instBuildMenu;

    // Use this for initialization
    void Start ()
    {
        buildButton.onClick.AddListener(BuildMenu);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void BuildMenu()
    {
        instBuildMenu = (GameObject)Instantiate(buildMenu);
        RadialMenuController.buildMenu = instBuildMenu;
    }

}
