using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildingManager : MonoBehaviour {

    public CityObject cityObject;
    int currentTier = 0;
    public GameObject curObject;

    public void Upgrade()
    {
        currentTier++;
        if (currentTier >= cityObject.prefabs.Length)
            return;

        Vector3 position = curObject.transform.position;
        Quaternion rotation = curObject.transform.rotation;
        Destroy(curObject);
        GameObject obj = Instantiate(cityObject.prefabs[currentTier], position, rotation);
        curObject = obj;
        obj.transform.localScale = new Vector3(cityObject.scale, cityObject.scale, cityObject.scale);

    }
}
