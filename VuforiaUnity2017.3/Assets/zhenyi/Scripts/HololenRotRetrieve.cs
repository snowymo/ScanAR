using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololenRotRetrieve : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UnityEngine.XR.InputTracking.disablePositionalTracking = false;
        // rotation is here all the time
    }
}
