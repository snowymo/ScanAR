using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggle : MonoBehaviour {
    public GameObject arcmr;
	// Use this for initialization
	void Awake () {
		if(GetComponent<CustomMsgMgr>().category == CustomMsgMgr.Category.Scanner)
        {
            arcmr.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
