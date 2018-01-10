using UnityEngine;

public class UIMenu : SimpleMenu<UIMenu> {

	public void OnMenuPressed()
    {
        Debug.Log("Menu button is pressed");
    }

    public void OnBuildPressed()
    {
        Debug.Log("Build button is pressed");
        BuildMenu.Show();
    }

    public void OnFFPressed()
    {
        Debug.Log("Fast Forward (FF) is pressed");
    }

    public void OnPausePressed()
    {
        Debug.Log("Pause button is pressed");
    }

    public void OnInfoPressed()
    {
        Debug.Log("Info button is pressed");
    }

    public void OnHelpPressed()
    {
        Debug.Log("Help button is pressed");
    }
}
