using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LayoutGroupResizer : MonoBehaviour {

    float startHeight;

    void Start()
    {
        startHeight = GetComponent<RectTransform>().rect.height;
    }

    // Use this for initialization
    void Update ()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
            return;
        float paddingUp = GetComponent<VerticalLayoutGroup>().padding.top;
        float paddingDown = GetComponent<VerticalLayoutGroup>().padding.bottom;
        float spacing = GetComponent<VerticalLayoutGroup>().spacing;
        float dy = paddingUp + paddingDown - spacing;

        for (int i = 0; i < transform.childCount; i++)
        {
            float childRectHeight = transform.GetChild(i).GetComponent<RectTransform>().rect.height;
            dy += spacing + childRectHeight;
        }

        if (dy > startHeight && dy != GetComponent<RectTransform>().rect.height)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, dy);
            ScrollRect propertiesScroller = GetComponentInParent<ScrollRect>();

            Canvas.ForceUpdateCanvases();
            propertiesScroller.verticalScrollbar.value = 1;
            propertiesScroller.verticalNormalizedPosition = 1;
            Canvas.ForceUpdateCanvases();

        }
        else if(dy <= startHeight && dy == GetComponent<RectTransform>().rect.height)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, startHeight);
        }

    }
}
