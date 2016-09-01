using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingMenuController : MonoBehaviour
{
    public Text loadingText;

    int count = 0;

    string string0 = "Loading";
    string string1 = "Loading.";
    string string2 = "Loading..";
    string string3 = "Loading...";

    string[] loadingTexts;

    void Start()
    {
        loadingTexts = new string[] { string0, string1, string2, string3 };
        InvokeRepeating("DotDotDot", 0, .7f);
    }

    void DotDotDot()
    {
        if (count == 4)
            count = 0;

        loadingText.text = loadingTexts[count];

        count++;
    }
}
