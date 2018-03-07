using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrate : MonoBehaviour {

    public Transform optiMarkers101;
    public Transform floatingMarkers;
    Matrix4x4 calibMtx;
    public TextMesh vLog;
    public Transform[] optiMarkers, outputMarkers;

    // Use this for initialization
    void Start () {
        calibMtx = Matrix4x4.identity;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0)
            OnTouch();

        // apply transformation to all optiMarkers
        if (calibMtx == Matrix4x4.identity)
            return;

        for (int i = 0; i < optiMarkers.Length; i++)
        {
            Matrix4x4 output = Matrix4x4.TRS(outputMarkers[i].position, outputMarkers[i].rotation, new Vector3(1, 1, 1));
            Matrix4x4 input = Matrix4x4.TRS(optiMarkers[i].position, optiMarkers[i].rotation, new Vector3(1, 1, 1));
            output = calibMtx * input;
            outputMarkers[i].position = output.GetColumn(3);
            outputMarkers[i].rotation = output.rotation;
        }
    }

    void OnTouch()
    {
        vLog.text = "TOUCHED";
        Matrix4x4 groundtruth = Matrix4x4.TRS(optiMarkers101.position, optiMarkers101.rotation, new Vector3(1, 1, 1));
        Matrix4x4 ini = Matrix4x4.TRS(floatingMarkers.position, floatingMarkers.rotation, new Vector3(1, 1, 1));
        calibMtx = ini * groundtruth.inverse;
    }
}
