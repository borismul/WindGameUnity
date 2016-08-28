using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadableText : MonoBehaviour {

    Text text;
    public float fadeStartTime;
    public float fadeDuration;
    float dFade;
    public float repeatRate = 1f / 30f;

    void OnEnable()
    {
        text = GetComponent<Text>();

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        dFade = 1 * repeatRate / fadeDuration;
        InvokeRepeating("FadeText", fadeStartTime, repeatRate);
    }
	
    void FadeText()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - dFade);
        if(text.color.a < 0.01f)
        {
            CancelInvoke("FadeText");
            gameObject.SetActive(false);
        }
    }

    public void ReEnable()
    {
        CancelInvoke("FadeText");
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        dFade = 1 * repeatRate / fadeDuration;
        text = GetComponent<Text>();
        InvokeRepeating("FadeText", fadeStartTime, repeatRate);

    }
}
