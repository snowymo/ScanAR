using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour {

    public CusMsg cusMessage;

    public Transform marker101;
    public Transform iniMarker;
    public Matrix4x4 calibMtx;

    public Transform[] optiMarkers;
    public Transform[] outputMarkers;

    // Use this for initialization
    void Start () {
		calibMtx = Matrix4x4.identity;
    }
	
	// Update is called once per frame
	void Update () {
        // apply transformation to all optiMarkers
        if (calibMtx == Matrix4x4.identity)
            return;

        for(int i = 0; i < optiMarkers.Length; i++)
        {
            Matrix4x4 output = Matrix4x4.TRS(outputMarkers[i].position, outputMarkers[i].rotation, new Vector3(1, 1, 1));
            Matrix4x4 input = Matrix4x4.TRS(optiMarkers[i].position, optiMarkers[i].rotation, new Vector3(1, 1, 1));
            output = calibMtx * input;
            outputMarkers[i].position = output.GetColumn(3);
            outputMarkers[i].rotation = output.rotation;
        }
	}

    void OnSelect()
    {
        // send back the matrix and documented that, which could be used for next frame and next time
        Matrix4x4 groundtruth = Matrix4x4.TRS(marker101.position, marker101.rotation, new Vector3(1, 1, 1));
        Matrix4x4 ini = Matrix4x4.TRS(iniMarker.position, iniMarker.rotation, new Vector3(1, 1, 1));
        calibMtx = ini * groundtruth.inverse;
        cusMessage.SendMatrix(calibMtx);
    }
}
