using UnityEngine;

[CreateAssetMenu]
public class BooleanVariable : ScriptableObject {

#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    public bool State;

    public void SetBool(bool state)
    {
        State = state;
    }

    public void SetBool(BooleanVariable state)
    {
        State = state.State;
    }
}
