// This script is the main handler for all function concerning menu's.
// It allows the dynamic interaction with prefabs, and thereby eases the addition of new menu's.

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PersianMenuManager : MonoBehaviour
{
    // Public Menu Prefabs are listed here:
    public UIMainMenuManager pauseMenu;
    public BuildMenu buildMenu;
    public LoadingMenuController loadMenu;
    public TileInfomationMenu tileMenu;
    public UIResourcesManager mainUI;

    // Stack of menus that are open at any given time
    private Stack<Menu> menuStack = new Stack<Menu>();

    public static PersianMenuManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        LoadingMenuController.Show();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void CreateInstance<T>() where T : Menu
    {
        var prefab = GetPrefab<T>();
        Instantiate(prefab, transform);
    }

    private T GetPrefab<T>() where T : Menu
    {
        // Get prefab by type dynamically. Possible because of the abstract types
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            var prefab = field.GetValue(this) as T;
            if (prefab != null)
            {
                return prefab;
            }
        }

        throw new MissingReferenceException("Prefab not found for type " + typeof(T));
    }

    public void OpenMenu(Menu instance)
    {
        // De-activate top menu
        if (menuStack.Count > 0)
        {
            if (instance.DisableMenusUnderneath)
            {
                foreach (var menu in menuStack)
                {
                    menu.gameObject.SetActive(false);

                    if (menu.DisableMenusUnderneath)
                        break;
                }
            }

            var topCanvas = instance.GetComponent<Canvas>();
            var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        // Add new menu to the top of the stack
        menuStack.Push(instance);

        //Debug.Log("Menu is opened");
    }

    public void CloseMenu(Menu menu)
    {
        if (menuStack.Count == 0)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because te menu stack is empty", menu.GetType());
            return;
        }

        if (menuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of the stack", menu.GetType());
            return;
        }

        CloseTopMenu();

        //Debug.Log("Menu is closed");
    }

    public void CloseTopMenu()
    {
        var instance = menuStack.Pop();

        if (instance.DestroyWhenClosed)
            Destroy(instance.gameObject);
        else
            instance.gameObject.SetActive(false);

        // reactivate top-menu
        // If that menu has a false DisableMenusUnderneath flag, it needs to enable the menu beneath as well
        foreach (var menu in menuStack)
        {
            menu.gameObject.SetActive(true);

            if (menu.DisableMenusUnderneath)
                break;
        }

    }

    private void Update()
    {

    }
}