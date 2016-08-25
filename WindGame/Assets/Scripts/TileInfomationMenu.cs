using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TileInfomationMenu : MonoBehaviour {

    public Text biome;
    public Text occupant;
    public Text position;

    GridTile tile;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void onEnable()
    {
        biome.text = tile.biome.ToString();
        occupant.text = tile.occupant.ToString();
        position.text = tile.position.ToString();
    }

    public void setTile(GridTile til)
    {
        tile = til;
    }

    public void clearTile()
    {
        tile = null;
    }
}
