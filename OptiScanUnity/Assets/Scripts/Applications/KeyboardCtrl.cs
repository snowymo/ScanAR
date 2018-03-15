using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyboardCtrl : MonoBehaviour {

    public enum MODE { NONE, TUNER, CALIB};
    public MODE cur_mode;

    public RectTransform tunerPanel, calibPanel;

	// Use this for initialization
	void Start () {
        cur_mode = MODE.NONE;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ++cur_mode;
            if (cur_mode > MODE.CALIB)
                cur_mode -= (MODE.CALIB + 1);
        }

        switch (cur_mode)
        {
            case MODE.NONE:
                tunerPanel.gameObject.SetActive( false);
                calibPanel.gameObject.SetActive(false);
                break;
            case MODE.CALIB:
                tunerPanel.gameObject.SetActive(false);
                calibPanel.gameObject.SetActive( true);
                break;
            case MODE.TUNER:
                calibPanel.gameObject.SetActive(false);
                tunerPanel.gameObject.SetActive(true);
                break;
            default:
                break;
        }
	}

}
