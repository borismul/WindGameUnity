using UnityEngine;
using System.Collections.Generic;

public class TestPlot : MonoBehaviour {

    public GameObject linePrefab;
    void Start()
    {

        GameObject testPanel = gameObject;

        List<float> xData = new List<float>();
        List<float> yData = new List<float>();

        for (float x = -10; x < 10; x += 0.1f)
        {
            xData.Add(x);
            yData.Add(x * x);
        }

        Plot plot = new Plot(testPanel, xData, yData, Color.green, .5f, linePrefab);
    }
}
