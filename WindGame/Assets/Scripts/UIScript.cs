using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

    private static UIScript instance;

    [Header("Prefabs")]
    public GameObject eventSystemPrefab;
    public GameObject buildMenuPrefab;
    public GameObject resourcesMenuPrefab;
    public GameObject mainMenuPrefab;
    public GameObject radialMenuPrefab;
    public GameObject turorialPrefab;

    // Use this for initialization
    void Awake ()
    {
        CreateSingleton();
        InstantiateStartPrefabs();
    }

    // Create the singletone for the UIManager. Also checks if there is another present and logs and error.
    void CreateSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("UIManager already exists while it should be instantiated only once.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Instantiate the starting prefabs as the children of the UIScript
    void InstantiateStartPrefabs()
    {
        GameObject obj = Instantiate(eventSystemPrefab);
        obj.transform.SetParent(transform);
        obj = Instantiate(resourcesMenuPrefab);
        obj.transform.SetParent(transform);
    }

    // Get the singleton instance
    public static UIScript GetInstance()
    {
        return instance;
    }

}
