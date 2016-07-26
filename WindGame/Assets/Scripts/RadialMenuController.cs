﻿using UnityEngine;
using System.Collections;

public class RadialMenuController : MonoBehaviour {

    public GameObject radMenu;
    bool justOpened;

    static GameObject radMenuInst;

    public static GameObject buildMenu;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        OnCloseClick();
        OnOpenClick();
    }

    void OnOpenClick()
    {
        if (Input.GetMouseButtonUp(1))
            justOpened = false;

        if (!Input.GetMouseButtonDown(1) || radMenuInst != null || buildMenu != null)
            return;

        Vector3 position = Input.mousePosition;

        radMenuInst = (GameObject)Instantiate(radMenu, Vector3.zero, Quaternion.identity);
        radMenuInst.transform.GetChild(0).position = position;
        radMenuInst.GetComponent<RectTransform>().localScale = Vector3.one;
        justOpened = true;
    }

    void OnCloseClick()
    {
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && radMenuInst != null && !justOpened)
            Destroy(radMenuInst);
    }
}