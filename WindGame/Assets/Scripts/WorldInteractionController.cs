using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WorldInteractionController : MonoBehaviour
{
    int terrainLayer = 1 << 8;

    Mesh highlighter;
    int[] tri;
    Vector2[] uv;
    Vector3[] vert = new Vector3[4];

    bool inInfoMode = false;

    static WorldInteractionController instance;

    void Start()
    {
        CreateSingleton();
        highlighter = new Mesh();
        tri = new int[6] {0, 2, 1, 0, 3, 2};
        uv = new Vector2[4] { new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f) };
    }

    // Create the singletone for the WorldInteractionController. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("WorldInteractionController already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Get the singleton instance
    public static WorldInteractionController GetInstance()
    {
        return instance;
    }

    void CheckSelectedTile()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, float.MaxValue, terrainLayer))
        {
            highlighter.Clear();
            GridTile tile = GridTile.FindClosestGridTile(hit.point);
            if (tile == null)
                return;

            for (int i = 0; i < 4; i++)
                vert[i] = tile.vert[i] + new Vector3(0, 0.3f, 0);

            highlighter.vertices = vert;
            highlighter.triangles = tri;
            highlighter.uv = uv;

            UIScript.GetInstance().SetActiveTile(tile);

        }
        highlighter.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = highlighter;
    }

    void CheckLeftClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject hitObject = hit.collider.gameObject;

            TurbineController tubineController = hit.transform.GetComponentInParent<TurbineController>();
            if (tubineController != null)
                UIScript.GetInstance().OpenTurbineMenu(tubineController);

            WindVaneController windVaneController = hit.transform.GetComponentInParent<WindVaneController>();

            if (windVaneController != null)
                UIScript.GetInstance().OpenWindVaneMenu(windVaneController);
        }

    }

    void Update()
    {
        if (!TerrainController.thisTerrainController.levelLoaded)
            return;

        if (inInfoMode)
            CheckSelectedTile();

        if (!UIScript.GetInstance().GetInBuildMode() || PointerInfo.overUIElement)
        {
            CheckLeftClick();
            GetComponent<MeshFilter>().mesh = highlighter;
        }
    }

    public void SetInInfoMode(bool mode)
    {
        if (inInfoMode && !mode)
        {
            highlighter.Clear();

        }
        inInfoMode = mode;
    }



}
