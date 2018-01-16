using UnityEngine.UI;
using UnityEngine;

public class BuildMenu : SimpleMenu<BuildMenu> {

    public BuildableObject[] buildableObjects;
    public Button genericBuildableObjectButton;
    public RectTransform buttonContentHolder;

    private void Start()
    {
        for (int i=0; i<buildableObjects.Length; i++)
        {
            // Instantiate buttons for all the Buildable objects, as a child of the content holder of the vertical scrollrect
            Button buildbutton = Instantiate(genericBuildableObjectButton, buttonContentHolder.transform);  
            // Set the text component equal to the short name provided in the buildableObject
            buildbutton.GetComponentInChildren<Text>().text = buildableObjects[i].shortName;
        }
    }

    public override void OnBackPressed()
    {
        Hide();
    }
}
