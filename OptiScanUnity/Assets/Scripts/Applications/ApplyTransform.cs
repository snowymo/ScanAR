using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransform : MonoBehaviour {

    public Calibrate104 curCalib;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Matrix4x4 curMatrix = curCalib.curOffset.inverse * curCalib.Mheadset.inverse * curCalib.Mpobj;
        transform.localPosition = curMatrix.GetColumn(3);
        transform.localRotation = curMatrix.rotation;

    }
}
