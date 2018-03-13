using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrate : MonoBehaviour {

    public Transform optiMarkers101;
    public Transform floatingMarkers;
    [SerializeField]
    Matrix4x4 calibMtx;
    public TextMesh vLog;
    public Transform[] optiMarkers, outputMarkers;

    // Use this for initialization
    void Awake () {
        calibMtx = Matrix4x4.identity;

    }
	
	// Update is called once per frame
	void Update () {
//         if (Input.touchCount > 0)
//             OnTouch();

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
            vLog.text = "RCVING";
        }
    }

    public void OnTouch()
    {
        // might disable the CALIB

        vLog.text = "TOUCHED";
        Matrix4x4 groundtruth = Matrix4x4.TRS(optiMarkers101.position, optiMarkers101.rotation, new Vector3(1, 1, 1));
        Matrix4x4 ini = Matrix4x4.TRS(floatingMarkers.position, floatingMarkers.rotation, new Vector3(1, 1, 1));
        calibMtx = ini * groundtruth.inverse;

        //
        GetComponent<SyncMtx>().SetHost(true);
    }

    public void SetMatrix(float[] m)
    {
        calibMtx.m00 = m[0];
        calibMtx.m01 = m[1];
        calibMtx.m02 = m[2];
        calibMtx.m03 = m[3];
        calibMtx.m10 = m[4];
        calibMtx.m11 = m[5];
        calibMtx.m12 = m[6];
        calibMtx.m13 = m[7];
        calibMtx.m20 = m[8];
        calibMtx.m21 = m[9];
        calibMtx.m22 = m[10];
        calibMtx.m23 = m[11];
        calibMtx.m30 = m[12];
        calibMtx.m31 = m[13];
        calibMtx.m32 = m[14];
        calibMtx.m33 = m[15];

    }

    public float[] GetMatrix()
    {
        float[] m = new float[16];
        m[0] = calibMtx.m00;
        m[1] = calibMtx.m01;
        m[2] = calibMtx.m02;
        m[3] = calibMtx.m03;
        m[4] = calibMtx.m10;
        m[5] = calibMtx.m11;
        m[6] = calibMtx.m12;
        m[7] = calibMtx.m13;
        m[8] = calibMtx.m20;
        m[9] = calibMtx.m21;
        m[10] = calibMtx.m22;
        m[11] = calibMtx.m23;
        m[12] = calibMtx.m30;
        m[13] = calibMtx.m31;
        m[14] = calibMtx.m32;
        m[15] = calibMtx.m33;
        return m;
    }
}
