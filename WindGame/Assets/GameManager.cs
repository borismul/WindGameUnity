using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject UIManagerPrefab;
    public GameObject mission1ManagerPrefab;
    public GameObject mainCameraPrefab;

    private static GameManager instance;

	void Awake ()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
	}

    // Create the singletone for the GameManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("GameManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the GameManager
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(UIManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(mission1ManagerPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(mainCameraPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public static GameManager GetInstance()
    {
        return instance;
    }
}
