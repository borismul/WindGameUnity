using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

public class TileInfomationMenu : MonoBehaviour {

    public Text biome;
    public Text occupant;
    public Text position;
    public Button closeMenu;
    public Slider slider;
    public Toggle toggle;
    public Text sliderValue;

    GridTile tile;
    string[] biomes = new string[4] { "Forest", "Sand", "Rock", "Grass" };

    TerrainController terrain;

    // Use this for initialization
    void Start ()
    {
        closeMenu.onClick.AddListener(CloseMenu);
        slider.onValueChanged.AddListener(delegate { ChangeWindHeight(slider.value); });
        toggle.onValueChanged.AddListener(delegate { ChangeSeeWind(toggle.isOn); });
    }


    void OnDisable()
    {
        terrain = TerrainController.thisTerrainController;
        if (terrain != null)
            WindVisualizer.instance.StopVisualizeWind();

        toggle.isOn = false;
    }
	
    void ChangeWindHeight(float value)
    {
        WindVisualizer.instance.height = value;
        sliderValue.text = Mathf.RoundToInt(value).ToString();
    }

    void ChangeSeeWind(bool showWind)
    {
        if (showWind)
        {
            WindVisualizer.instance.VisualizeWind();
        }
        else
        {
            WindVisualizer.instance.StopVisualizeWind();
        }
    }

    public void setTile(GridTile til)
    {
        tile = til;

        WindController.GetWindAtTile(til, 0);
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
            
            } else if (tile.occupant.obj.name.Equals("CactusEmpty(Clone)"))
            {
                occupant.text = "A cactus";
            }
            else
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
