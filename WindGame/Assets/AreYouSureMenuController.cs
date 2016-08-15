using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
public class AreYouSureMenuController : MonoBehaviour {

    public Button yesButton;
    public Button cancelButton;

    public UnityAction yesCall;

    public static AreYouSureMenuController singleton;

	// Use this for initialization
	void Start ()
    {
        singleton = this;

        yesCall += CancelButton;
        yesButton.onClick.AddListener(yesCall);
        cancelButton.onClick.AddListener(CancelButton);
    }

    void CancelButton()
    {
        Destroy(gameObject);
    }
}
