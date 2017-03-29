using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Plot : MonoBehaviour {

    GameObject plotPanel;
    List<List<float>> xData;
    List<List<float>> yData;
    float plotWidth;
    float plotHeight;    

    public Plot(GameObject plotPanel, List<List<float>> xData, List<List<float>> yData)
    {
        this.plotPanel = plotPanel;
        this.xData = xData;
        this.yData = yData;

        plotWidth = plotPanel.GetComponent<RectTransform>().rect.width;
        plotHeight = plotPanel.GetComponent<RectTransform>().rect.height;
    }

    public Plot(GameObject plotPanel, List<float> xData, List<float> yData)
    {
        this.plotPanel = plotPanel;

        this.xData = new List<List<float>>();
        this.yData = new List<List<float>>();

        this.xData.Add(xData);
        this.yData.Add(yData);
    }



}
