using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    float centerX;
    float centerY;
    float centerZ;
    public float length;
    public float width;
    public float height;
    public float gridResolution;
    public bool drawAnalyseZone;
    public bool debugGrid;
    public GameObject gridPoint;

    static List<GridTile> grid = new List<GridTile>();

	void Start ()
    {
        centerX = TerrainController.statLength / 2;
        centerZ = TerrainController.statLength / 2;

        GenGrid();
	}

    void OnDrawGizmos()
    {
        if (drawAnalyseZone)
        {
            Gizmos.color = new Color(0, 0.8f, 1, 0.3f);
            Gizmos.DrawCube(new Vector3(centerX, centerY, centerZ), new Vector3(length, height, width));
            Gizmos.color = new Color(1, 1, 1, 0.7f);
            Gizmos.DrawWireCube(new Vector3(centerX, centerY, centerZ), new Vector3(length, height, width));
        }
    }

    void GenGrid()
    {
        for (int i = 0; i <= length / gridResolution; i++)
        {
            for (int j = 0; j <= width / gridResolution; j++)
            {
                float xPos = (i * gridResolution + centerX - length / 2);
                float yPos = height/2 + centerY;
                float zPos = (j * gridResolution + centerZ - width / 2);

                Vector3 pos = new Vector3(xPos, yPos, zPos);
                Vector3 direc = Vector3.down;
                Ray ray = new Ray(pos, direc);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, height))
                {
                    if (debugGrid)
                        grid.Add(new GridTile(hit.point, 1, true, null, (GameObject)Instantiate(gridPoint, hit.point, Quaternion.identity)));
                    else
                        grid.Add(new GridTile(hit.point, 1, true, null));

                }
            }
        }
    }

    public static void ShowGrid()
    {
        for (int i = 0; i < grid.Count - 1; i++)
            Debug.DrawLine(grid[i].position, grid[i + 1].position, Color.blue, Mathf.Infinity);
    }

    public static GridTile FindClosestGridTile(Vector3 pos)
    {
        float dist = Mathf.Infinity;
        GridTile closest = null;

        for (int i = 0; i < grid.Count; i++)
        {
            float curDist = Vector3.Distance(pos, grid[i].position);
            if (curDist < dist)
            {
                dist = curDist;
                closest = grid[i];
            }
        }
        return closest;
    }

    public static void SetOccupant(GridTile gridTile, GameObject occupant, float radius)
    {
        foreach (GridTile curGridTile in grid)
        {
            float dist = Vector3.Distance(gridTile.position, curGridTile.position);
            if (dist < radius)
            {
                curGridTile.occupant = occupant;
                if(curGridTile.gridPointDebug != null)
                    curGridTile.gridPointDebug.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }
}
