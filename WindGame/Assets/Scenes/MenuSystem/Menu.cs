// Menu base class, used to inherit from by the specific menus.
// Each menu has therefore it's own type, and is handled here to facilitate basic functions
// such as opening and closing, and what behaviour is required upon those actions.

using UnityEngine;

public abstract class Menu<T> : Menu where T : Menu<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    protected static void Open()
    {
        if (Instance == null)
            MenuManager.Instance.CreateInstance<T>();
        else
            Instance.gameObject.SetActive(true);

        MenuManager.Instance.OpenMenu(Instance);
    }

    protected static void Close()
    {
        if (Instance == null)
        {
            Debug.LogErrorFormat("Trying to close menu, but instance is null");
            return;
        }

        MenuManager.Instance.CloseMenu(Instance);
    }

    public override void OnBackPressed()
    {
        Close();
    }


}

public abstract class Menu : MonoBehaviour
{
    [Tooltip("Destroy the GO when menu is closed")]
    public bool DestroyWhenClosed = true;

    [Tooltip("Disable menus underneath the stack")]
    public bool DisableMenusUnderneath = true;

    public abstract void OnBackPressed();
}
