using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour {

    public Button buildButton;
    public Button infoButton;
    public GameObject buildMenu;
    GameObject instBuildMenu;

    GridTile activeTile;

    // Use this for initialization
    void Start ()
    {
        buildButton.onClick.AddListener(BuildMenu);
        infoButton.onClick.AddListener(InfoButton);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    //Sets the target that the mouse was pointed at when user right clicked
    public void setTarget(GridTile tile)
    {
        activeTile = tile;
    }

    void BuildMenu()
    {
        instBuildMenu = (GameObject)Instantiate(buildMenu);
        RadialMenuController.buildMenu = instBuildMenu;
    }

    void InfoButton()
    {
        if (activeTile == null) return;

        UIScript.GetInstance().OpenTileMenu(activeTile);
        
    }

}
