using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TileInfomationMenu : MonoBehaviour {

    public Text biome;
    public Text occupant;
    public Text position;
    public Button closeMenu;

    GridTile tile;
    string[] biomes = new string[4] { "Forest", "Sand", "Rock", "Grass" };

	// Use this for initialization
	void Start ()
    { 

    }
	
	// Update is called once per frame
	void Update ()
    {
        closeMenu.onClick.AddListener(CloseMenu);
    }

    public void setTile(GridTile til)
    {
        tile = til;

        biome.text = biomes[tile.biome];
        if (tile.occupant == null)
        {
            occupant.text = "None";
        }
        else
        {
            if (tile.occupant.obj.name.Equals("RockEmpty(Clone)") || tile.occupant.obj.name.Equals("StoneCubeEmpty(Clone)"))
            {
                occupant.text = "A rock";
            } else if (tile.occupant.obj.name.Equals("House Colored"))
            {
                occupant.text = "A house";
            } else if(tile.occupant.obj.name.Equals("NewTreeEmpty(Clone)"))
            {
                occupant.text = "A tree";
            } else 
            {
                occupant.text = "Wind Turbine";
            }
        }
        position.text = tile.position.ToString("F0");
    }

    public void clearTile()
    {
        tile = null;
    }

    void CloseMenu()
    {
        UIScript.GetInstance().CloseTileMenu();
        WorldInteractionController.GetInstance().SetInInfoMode(false);
    }
}
