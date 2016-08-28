using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class JSLibScript : MonoBehaviour {

	// public Button button;

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    void Start() {
    	
    }

    public void aFunction(){
    	Hello();
    }
}
