using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateMapPanelController : MonoBehaviour {

    public InputField length;
    public InputField width;
    public InputField height;
    public InputField seed;

    public InputField terOctaves;
    public InputField terPersistance;
    public InputField terFrequency;

    public InputField bioOctaves;
    public InputField bioPersistance;
    public InputField bioFrequency;

    public InputField chunkSize;
    public InputField tileSize;
    public InputField tileSlope;

    public InputField waterChunkSize;
    public InputField waterTileSize;

    public InputField waterLevel;
    public InputField waveHeight;

    public InputField waterOctaves;
    public InputField waterPersistance;
    public InputField waterFrequency; 

    public Button backButton;
    public Button buildMap;

    public static CreateMapPanelController createPanelMapController;

    TerrainController terrainController;

    void Awake()
    {
        createPanelMapController = this;
        terrainController = TerrainController.thisTerrainController;
    }

    void Start()
    {
        backButton.onClick.AddListener(Back);
        buildMap.onClick.AddListener(BuildMapButton);
    }

    void BuildMapButton()
    {
        terrainController.length = int.Parse(length.text);
        terrainController.width = int.Parse(width.text);
        terrainController.maxHeight = int.Parse(height.text);
        terrainController.seed = int.Parse(seed.text);

        terrainController.terrainOctaves = int.Parse(terOctaves.text);
        terrainController.terrainPersistance = float.Parse(terPersistance.text);
        terrainController.terrainFrequency = float.Parse(terFrequency.text);

        terrainController.biomeOctaves = int.Parse(bioOctaves.text);
        terrainController.biomePersistance = float.Parse(bioPersistance.text);
        terrainController.biomeFrequency = float.Parse(bioFrequency.text);

        terrainController.chunkSize = int.Parse(chunkSize.text);
        terrainController.tileSize = int.Parse(tileSize.text);
        terrainController.tileSlope = int.Parse(tileSlope.text);

        terrainController.waterChunkSize = int.Parse(waterChunkSize.text);
        terrainController.waterTileSize = int.Parse(waterTileSize.text);
        terrainController.waterLevel = int.Parse(waterLevel.text);
        terrainController.maxWaveHeight = int.Parse(waveHeight.text);

        terrainController.waterOctaves = int.Parse(waterOctaves.text);
        terrainController.waterPersistance = float.Parse(waterPersistance.text);
        terrainController.waterFrequency = float.Parse(waterFrequency.text);

        terrainController.BuildButton();
        Destroy(this.gameObject);
    }

    void Back()
    {
        Destroy(this.gameObject);
    }
}
