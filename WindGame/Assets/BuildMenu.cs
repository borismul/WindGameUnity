using UnityEngine;

public class BuildMenu : SimpleMenu<BuildMenu> {

	public override void OnBackPressed()
    {
        Hide();
    }
}
