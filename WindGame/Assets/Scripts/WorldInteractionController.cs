using UnityEngine;
using System.Collections;

public class WorldInteractionController : MonoBehaviour
{

    int terrainLayer = 1 << 8;

    GameObject previousChunk;
    IEnumerator CheckSelectedTile()
    {
        TerrainController terrain = TerrainController.thisTerrainController;
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, float.MaxValue, terrainLayer))
        {
            GameObject hitObj = hit.collider.gameObject;
            Vector3 hitPoint = hit.point;
            Vector3 localPos = hitPoint - hitObj.transform.position;
            int iHighLight = Mathf.RoundToInt((localPos.x - terrain.tileSize / 2)/ terrain.tileSize);
            int kHighLight = Mathf.RoundToInt((localPos.z - terrain.tileSize / 2)/ terrain.tileSize);

            hitObj.GetComponent<Chunk>().HighLightTile(iHighLight, kHighLight);
            if (previousChunk != null && previousChunk != hitObj)
            {
                yield return null;
                previousChunk.GetComponent<Chunk>().DeHighLight();
            }
            previousChunk = hitObj;
        }
    }

    void Update()
    {
        if (!TerrainController.thisTerrainController.levelLoaded)
            return;

        StartCoroutine(CheckSelectedTile());
    }

}
