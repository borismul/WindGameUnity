using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SavePanelController : MonoBehaviour {

    public Button saveButton;
    public Button cancelButton;
    public InputField saveInput;
    public Text errText;

    public static SavePanelController thisSavePanel;

    Coroutine coroutine;

    void Awake ()
    {
        thisSavePanel = this;
	}

    void Start()
    {
        saveButton.onClick.AddListener(Save);
        cancelButton.onClick.AddListener(Cancel);
    }

    void Cancel()
    {
        Destroy(this.gameObject);
    }

    void Save()
    {
        if (saveInput.text != "")
            TerrainController.thisTerrainController.Save(saveInput.text + ".bytes");
        else
        {
            errText.text = "Enter a save name.";
            errText.color = new Color(1, 0, 0, 1);
            if(coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Fade());
        }
        Cancel();
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(3);
        while (errText.color.a != 0)
        {
            Color color = errText.color;
            color.a -= 0.01f;
            errText.color = color;
            yield return null;
        }
    }
}
