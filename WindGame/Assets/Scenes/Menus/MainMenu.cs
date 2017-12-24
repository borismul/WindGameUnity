// The main menu specifics. Here buttons are linked to a game-specific function.
// This includes e.g. opening other menu's and closing the application.

using UnityEngine;

public class MainMenu : SimpleMenu<MainMenu>
{
    public void OnPlayPressed()
    {
        Debug.Log("Play is pressed");
        MissionMenu.Show();
    }

    public void OnOptionsPressed()
    {
        Debug.Log("Options is pressed");
    }

    public override void OnBackPressed()
    {
        Debug.Log("Application will close");
        Application.Quit();
    }
}
