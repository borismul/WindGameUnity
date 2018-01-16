using UnityEngine;

// This class is meant to define the base properties of a buildable object.

public class BuildableObject : MonoBehaviour {

#if UNITY_EDITOR
    [Multiline]
    public string developerDescription = "Description of what this is";
#endif

    public string longName = "Buildable Object";    // Name for use in GUI elements
    public string shortName = "Buildable";          // Name for use on buttons (smaller)

    [Multiline] // Background information for use in build menu
    public string backgroundInfo = "Background information that characterises this buildable element";   

    public bool canRotateOnBuild = true;
    public bool canBeBuiltInSea = false;

    public float buildingCost;      // Cost of building the object
    [HideInInspector]
    public Vector3 worldPosition;   // Position in the world
    [HideInInspector]
    public float timeBuilt;         // Time in-game at which the object is built

}
