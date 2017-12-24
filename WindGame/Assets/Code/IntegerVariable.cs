using UnityEngine;

[CreateAssetMenu]
public class IntegerVariable : ScriptableObject {

#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    public int Value;

    public void SetValue(int value)
    {
        Value = value;
    }

    public void SetValue(IntegerVariable value)
    {
        Value = value.Value;
    }

    public void ApplyChange(int value)
    {
        Value += value;
    }

    public void ApplyChange(IntegerVariable value)
    {
        Value += value.Value;
    }
}
