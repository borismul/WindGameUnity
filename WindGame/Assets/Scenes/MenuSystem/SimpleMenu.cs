// Most simple implementation of a menu controller.
// If required, create a new abstract class that inherits from Menu
// and use that to add on to the Open() methods, to create more complicated/complex menu behaviour

public abstract class SimpleMenu<T> : Menu<T> where T : SimpleMenu<T>
{
    public static void Show()
    {
        Open();
    }

    public static void Hide()
    {
        Close();
    }
}
