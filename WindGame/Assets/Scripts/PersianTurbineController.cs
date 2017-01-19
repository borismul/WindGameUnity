using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersianTurbineController :TurbineController {
    
    public PersianTurbineSpecificsController specificProperties;

    public override void SpecificProperties()
    {
        foreach (FloatProperty prop in specificProperties.properties.floatProperty)
        {
            if (prop.powerFunction != null)
                prop.powerFunction.Invoke(prop.callObject, new object[] { });
        }
        foreach (IntProperty prop in specificProperties.properties.intProperty)
        {
            if (prop.powerFunction != null)
                prop.powerFunction.Invoke(prop.callObject, new object[] { });
        }
        foreach (BoolProperty prop in specificProperties.properties.boolProperty)
        {
            if (prop.powerFunction != null)
                prop.powerFunction.Invoke(prop.callObject, new object[] { });
        }
        foreach (MinMaxFloatProperty prop in specificProperties.properties.minMaxProperty)
        {
            if (prop.maxPowerFunction != null)
                prop.maxPowerFunction.Invoke(prop.callObject, new object[] { });
            if (prop.minPowerFunction != null)
                prop.minPowerFunction.Invoke(prop.callObject, new object[] { });
        }
    }

    public override void startChild()
    {
        specificProperties = GetComponent<PersianTurbineSpecificsController>();
        SpecificProperties();
    }

    public override ObjectProperties getSpecificProperties()
    {
        return specificProperties.properties;
    }
}
