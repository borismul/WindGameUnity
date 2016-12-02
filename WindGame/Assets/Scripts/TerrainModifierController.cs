using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerrainModifierController : MonoBehaviour {

    public static TerrainModifierController singleton;

    public Button raiseButton;
    public Button lowerButton;
    public Button biomeButton;

    public Slider sizeSlider;
    public Slider strengthSlider;

    public Text sizeText;
    public Text strengthText;

    public Color activeColor;
    public Color inActiveColor;
    public Color highLightColor;
    public Color pressedColor;

    public LayerMask mask;

    public GameObject selectionMesh;

    int currentActive = -1;
    int currentSize = 0;
    int currentStrength = 0;

    Button currentActiveButton;

    ColorBlock activeBlock;
    ColorBlock inActiveBlock;

    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    TerrainController terrainController;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateSelectionMesh();
    }

    void Initialize()
    {
        mesh = new Mesh();

        activeBlock = new ColorBlock();
        activeBlock.normalColor = activeColor;
        activeBlock.highlightedColor = highLightColor;
        activeBlock.pressedColor = pressedColor;
        activeBlock.colorMultiplier = 1;

        inActiveBlock = new ColorBlock();
        inActiveBlock.normalColor = inActiveColor;
        inActiveBlock.highlightedColor = highLightColor;
        inActiveBlock.pressedColor = pressedColor;
        inActiveBlock.colorMultiplier = 1;

        raiseButton.onClick.AddListener(delegate { ButtonSelect(raiseButton, 0); });
        lowerButton.onClick.AddListener(delegate { ButtonSelect(lowerButton, 1); });
        biomeButton.onClick.AddListener(delegate { ButtonSelect(biomeButton, 2); });

        sizeText.text = sizeSlider.value.ToString();
        strengthText.text = strengthSlider.value.ToString();

        sizeSlider.onValueChanged.AddListener(delegate { SetSliderText(sizeSlider.value, sizeText, 0); });
        strengthSlider.onValueChanged.AddListener(delegate { SetSliderText(strengthSlider.value, strengthText, 1); });

        terrainController = TerrainController.thisTerrainController;
    }

    void ButtonSelect(Button button, int type)
    {
        if (currentActiveButton != null)
            ButtonDeSelect();

        currentActiveButton = button;
        currentActive = type;

        currentActiveButton.colors = activeBlock;
    }

    void ButtonDeSelect()
    {
        currentActiveButton.colors = inActiveBlock;
    }

    void SetSliderText(float value, Text text, int type)
    {
        text.text = value.ToString();

        if (type == 0)
            currentSize = (int)value;

        if (type == 1)
            currentStrength = (int)value;
    }

    void UpdateSelectionMesh()
    {
        int tileSize = terrainController.tileSize;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            vertices.Clear();
            triangles.Clear();

            Vector3 hitPoint = hit.point;

            for (int i = -currentSize; i < currentSize; i++)
            {
                for (int j = -currentSize; j < currentSize; j++)
                {
                    Vector3 currentPos = new Vector3(hitPoint.x + i * tileSize, 0, hitPoint.z + j * tileSize);
                    GridTile curGridTile = GridTile.FindClosestGridTile(currentPos);

                    if (curGridTile == null || Vector3.Distance(hit.point, curGridTile.position) > currentSize * tileSize)
                        continue;

                    CreatePlane(curGridTile.gridNodes.ToArray());
                }
            }

            SetMesh();
        }
    }

    void CreatePlane(GridNode[] vertPos)
    {
        int index = vertices.Count;
        Vector3 offset = new Vector3(0, 2f, 0);
        triangles.Add(index);
        triangles.Add(index + 2);
        triangles.Add(index + 1);

        triangles.Add(index);
        triangles.Add(index + 3);
        triangles.Add(index + 2);

        vertices.Add(vertPos[0].position + offset);
        vertices.Add(vertPos[1].position + offset);
        vertices.Add(vertPos[2].position + offset);
        vertices.Add(vertPos[3].position + offset);
    }

    void SetMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        MeshFilter filter = selectionMesh.GetComponent<MeshFilter>();
        if (filter == null)
        {
            selectionMesh.AddComponent<MeshRenderer>();
            filter = selectionMesh.AddComponent<MeshFilter>();
        }

        filter.mesh = mesh;
    }
}

