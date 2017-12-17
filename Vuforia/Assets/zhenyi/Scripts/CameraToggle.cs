using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggle : MonoBehaviour {
    public GameObject arcmr;
    public GameObject vivecmr;
	// Use this for initialization
	void Start () {
		if(GetComponent<CustomMsgMgr>().category == CustomMsgMgr.Category.Scanner)
        {
            arcmr.SetActive(false);
            vivecmr.SetActive(true);
        }else if(GetComponent<CustomMsgMgr>().category == CustomMsgMgr.Category.Hololens)
        {
            arcmr.SetActive(true);
            vivecmr.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
