using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Plot : MonoBehaviour {

    GameObject linePrefab;

    GameObject plotPanel;
    List<Color> color;
    List<float> lineThickness;
    List<List<float>> xData;
    List<List<float>> yData;
    float plotWidth;
    float plotHeight;

    Vector2 stdMarginLeftBottom;
    Vector2 equalMargin;

    List<GameObject> currentPlotItems;

    public Plot(GameObject plotPanel, List<List<float>> xData, List<List<float>> yData, Color color, float lineThickness, GameObject linePrefab)
    {
        currentPlotItems = new List<GameObject>();

        this.plotPanel = plotPanel;
        this.xData = xData;
        this.yData = yData;
        this.color = new List<Color>();
        this.lineThickness = new List<float>();
        this.color.Add(color);
        this.lineThickness.Add(lineThickness);
        this.linePrefab = linePrefab;

        plotWidth = plotPanel.GetComponent<RectTransform>().rect.width;
        plotHeight = plotPanel.GetComponent<RectTransform>().rect.height;

        stdMarginLeftBottom = new Vector2(plotWidth * 0.1f, plotHeight * 0.08f);
        equalMargin = new Vector2(plotWidth * 0.05f, plotHeight * 0.05f);

        PlotAxis();
        for (int i = 0; i < xData.Count; i++)
        {
            PlotGraph(xData[i], yData[i], color, lineThickness);
        }
    }

    public Plot(GameObject plotPanel, List<float> xData, List<float> yData, Color color, float lineThickness, GameObject linePrefab)
    {
        currentPlotItems = new List<GameObject>();

        this.plotPanel = plotPanel;

        this.xData = new List<List<float>>();
        this.yData = new List<List<float>>();

        this.xData.Add(xData);
        this.yData.Add(yData);

        this.color = new List<Color>();
        this.lineThickness = new List<float>();
        this.color.Add(color);
        this.lineThickness.Add(lineThickness);
        this.linePrefab = linePrefab;

        plotWidth = plotPanel.GetComponent<RectTransform>().rect.width;
        plotHeight = plotPanel.GetComponent<RectTransform>().rect.height;

        stdMarginLeftBottom = new Vector2(plotWidth * 0.1f, plotHeight * 0.08f);
        equalMargin = new Vector2(plotWidth * 0.05f, plotHeight * 0.05f);

        PlotAxis();
        PlotGraph(xData, yData, color, lineThickness);
    }

    void AddLinePiece(Vector2 point1, Vector2 point2, Color color, float thickness)
    {
        GameObject line = (GameObject)Instantiate(linePrefab, plotPanel.transform);
        line.GetComponent<Image>().color = color;

        RectTransform lineRect = line.GetComponent<RectTransform>();

        float lineLength = Vector2.Distance(point1, point2);
        double linerotation = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x);

        lineRect.localScale = Vector2.one;
        lineRect.anchoredPosition = point1;
        lineRect.sizeDelta = new Vector2(lineLength, thickness);
        lineRect.localRotation = Quaternion.Euler(0, 0, (float)(linerotation * Mathf.Rad2Deg));
    }

    void PlotGraph(List<float> xData, List<float> yData, Color color, float thickness)
    {
        float maxX = 0;
        float maxY = 0;
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;

        for (int i = 0; i < xData.Count; i++)
        {
            if (xData[i] > maxX)
                maxX = xData[i];

            if (yData[i] > maxY)
                maxY = yData[i];

            if (xData[i] < minX)
                minX = xData[i];

            if (yData[i] < minY)
                minY = yData[i];
        }

        float scaleX = 1f / ((maxX - minX)) * (plotWidth - (stdMarginLeftBottom.x + equalMargin.x));
        float scaleY = 1f / ((maxY - minY)) * (plotHeight - (stdMarginLeftBottom.y + equalMargin.y));

        for (int i = 0; i < xData.Count - 1; i++)
        {
            float currentX1 = xData[i] - minX;
            float currentX2 = xData[i + 1] - minX;
            float currentY1 = yData[i] - minY;
            float currentY2 = yData[i + 1] - minY;

            Vector2 point1 = new Vector2(currentX1 * scaleX + stdMarginLeftBottom.x + equalMargin.x / 2, currentY1 * scaleY + stdMarginLeftBottom.y + equalMargin.y / 2);
            Vector2 point2 = new Vector2(currentX2 * scaleX + stdMarginLeftBottom.x + equalMargin.x / 2, currentY2 * scaleY + stdMarginLeftBottom.y + equalMargin.y / 2);

            AddLinePiece(point1, point2, color, thickness);
        }
    }

    void PlotAxis()
    {
        Vector2 origin = new Vector2(stdMarginLeftBottom.x + equalMargin.x / 2, stdMarginLeftBottom.y + equalMargin.y / 2);
        Vector2 xAxisPoint2 = new Vector2(plotWidth - equalMargin.x / 2, stdMarginLeftBottom.y + equalMargin.y / 2);
        Vector2 yAxisPoint2 = new Vector2(stdMarginLeftBottom.x + equalMargin.x / 2, plotHeight - equalMargin.y / 2);

        AddLinePiece(origin, xAxisPoint2, Color.black, 0.5f);
        AddLinePiece(origin, yAxisPoint2, Color.black, 0.5f);
    }
}
