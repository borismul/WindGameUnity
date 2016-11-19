using UnityEngine;
using System.Collections;

public class PriceController : MonoBehaviour {

    public float price;

    public float CalculateCost()
    {
        ObjectProperties properties = GetComponent<PropertiesContainer>().properties;
        float cost = 0;

        foreach (FloatProperty prop in properties.floatProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }
        foreach (IntProperty prop in properties.intProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }
        foreach (BoolProperty prop in properties.boolProperty)
        {
            if (prop.costFunction != null)
            {
                cost += (int)prop.costFunction.Invoke(prop.callObject, new object[] { prop.property });
            }
        }

        price = cost;

        return cost;
    }
}
