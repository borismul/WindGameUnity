using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SavedTurbineProperties {

    static Dictionary<string, float> savedFloatProperties = new Dictionary<string, float>();
    static Dictionary<string, int> savedIntProperties = new Dictionary<string, int>();
    static Dictionary<string, bool> savedBoolProperties = new Dictionary<string, bool>();

    public static bool GetSavedValue(string nameId, out float value)
    {
        return savedFloatProperties.TryGetValue(nameId, out value);
    }

    public static bool GetSavedValue(string nameId, out int value)
    {
        return savedIntProperties.TryGetValue(nameId, out value);
    }

    public static bool GetSavedValue(string nameId, out bool value)
    {
        return savedBoolProperties.TryGetValue(nameId, out value);
    }


    public static void SaveValue(string nameId, float value)
    {
        if (!savedFloatProperties.ContainsKey(nameId))
            savedFloatProperties.Add(nameId, value);
        else
        {
            savedFloatProperties.Remove(nameId);
            savedFloatProperties.Add(nameId, value);
        }
    }

    public static void SaveValue(string nameId, int value)
    {
        if (!savedIntProperties.ContainsKey(nameId))
            savedIntProperties.Add(nameId, value);
        else
        {
            savedIntProperties.Remove(nameId);
            savedIntProperties.Add(nameId, value);
        }
    }

    public static void SaveValue(string nameId, bool value)
    {
        if (!savedBoolProperties.ContainsKey(nameId))
            savedBoolProperties.Add(nameId, value);
        else
        {
            savedBoolProperties.Remove(nameId);
            savedBoolProperties.Add(nameId, value);
        }
    }

}
