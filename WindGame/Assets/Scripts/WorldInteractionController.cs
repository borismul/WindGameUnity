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

    void Start()
    {
        highlighter = new Mesh();
        tri = new int[6] {0, 2, 1, 0, 3, 2};
        uv = new Vector2[4] { new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f) };
    }

    void CheckSelectedTile()
    {
        TerrainController terrain = TerrainController.thisTerrainController;
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, float.MaxValue, terrainLayer))
        {
            highlighter.Clear();
            GameObject hitObj = hit.collider.gameObject;
            GridTile tile = GridTile.FindClosestGridTile(hit.point);
            for (int i = 0; i < 4; i++)
                vert[i] = tile.vert[i] + new Vector3(0, 0.3f, 0);

            highlighter.vertices = vert;
            highlighter.triangles = tri;
            highlighter.uv = uv;
        }
        highlighter.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = highlighter;
    }

    void Update()
    {
        if (!TerrainController.thisTerrainController.levelLoaded)
            return;

       CheckSelectedTile();
    }

}
