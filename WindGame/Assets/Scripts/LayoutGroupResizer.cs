using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LayoutGroupResizer : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        //Rect parentRect = GetComponent<RectTransform>().rect;
        //int childCount = transform.childCount;
        //if (childCount == 0)
        //    return;

        //float childRectHeight = transform.GetChild(0).GetComponent<RectTransform>().rect.height;
        //float paddingUp = GetComponent<VerticalLayoutGroup>().padding.top;
        //float paddingDown = GetComponent<VerticalLayoutGroup>().padding.bottom;
        //float spacing = GetComponent<VerticalLayoutGroup>().spacing;

        //print(paddingUp + " " + paddingDown + " " + (childCount - 1) * spacing + " " + childRectHeight * childCount);

        //parentRect.height = paddingUp + paddingDown + (childCount - 1) * spacing + childRectHeight * childCount;
        //print(parentRect.height);
	}
	
    void Update()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
            return;

        float childRectHeight = transform.GetChild(0).GetComponent<RectTransform>().rect.height;
        float paddingUp = GetComponent<VerticalLayoutGroup>().padding.top;
        float paddingDown = GetComponent<VerticalLayoutGroup>().padding.bottom;
        float spacing = GetComponent<VerticalLayoutGroup>().spacing;

        GetComponent<RectTransform>().sizeDelta = new Vector2(0, paddingUp + paddingDown + (childCount - 1) * spacing + childRectHeight * childCount + 1f);
    }
}
