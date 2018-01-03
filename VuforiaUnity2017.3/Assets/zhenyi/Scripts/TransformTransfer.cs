using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTransfer : MonoBehaviour {

    public Transform refer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition= refer.position;
        transform.localRotation= refer.rotation;
    }
}
