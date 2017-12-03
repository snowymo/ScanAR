using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainZero : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
