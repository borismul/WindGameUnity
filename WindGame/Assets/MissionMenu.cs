using UnityEngine.SceneManagement;
using UnityEngine;

public class MissionMenu : SimpleMenu<MissionMenu>
{

    public void OnPersianPressed()
    {
        Debug.Log("Persian Mission is being Loaded");
        SceneManager.LoadScene(1);
    }

    public void OnNASAPressed()
    {
        Debug.Log("NASA Mission is being Loaded");
    }

    public void OnNorthSeaPressed()
    {
        Debug.Log("NorthSea Mission is being Loaded");
    }

    public override void OnBackPressed()
    {
        Debug.Log("Back to main menu");
        Hide();
    }
}
