using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class CalibrateMgr : MonoBehaviour {

    int curPointPair;
    int curHoloPoint, curVTPoint;

    public int requiredPairAmount;
    [SerializeField]
    double[] holoPointSets;
    [SerializeField]
    double[] vtPointSets;

    [SerializeField]
    double[] calibResult;
    [SerializeField]
    Matrix4x4 calibResultMat;

    public bool bCalib;

    [DllImport("AffinedTransformation", EntryPoint = "EstimateAffTransSF")]
    public static extern void EstimateAffTransSF(IntPtr holopoints, IntPtr vivepoints, IntPtr transformation, int pCnt);

    // Use this for initialization
    void Start() {

        curPointPair = 0;
        curHoloPoint = 0;
        curVTPoint = 0;
        calibResult = new double[12];
        holoPointSets = new double[requiredPairAmount * 3];
        vtPointSets = new double[requiredPairAmount * 3];
        bCalib = false;
        calibResultMat = Matrix4x4.identity;
    }

    // Update is called once per frame
    void Update() {

    }

    public void addPointPair(Vector3 holoCmrPoint, Vector3 vivetrackerPoint)
    {
        if(curPointPair < requiredPairAmount)
        {
            holoPointSets[curPointPair * 3] = holoCmrPoint[0];
            holoPointSets[curPointPair * 3 + 1] = holoCmrPoint[1];
            holoPointSets[curPointPair * 3 + 2] = holoCmrPoint[2];

            vtPointSets[curPointPair * 3] = vivetrackerPoint[0];
            vtPointSets[curPointPair * 3 + 1] = vivetrackerPoint[1];
            vtPointSets[curPointPair * 3 + 2] = vivetrackerPoint[2];

            curPointPair++;
        }
    }

    public void addVTPoint(Vector3 vivetrackerPoint)
    {
        if (curVTPoint < requiredPairAmount)
        {
            vtPointSets[curVTPoint * 3] = vivetrackerPoint[0];
            vtPointSets[curVTPoint * 3 + 1] = vivetrackerPoint[1];
            vtPointSets[curVTPoint * 3 + 2] = vivetrackerPoint[2];

            curVTPoint++;

            if (curVTPoint == curHoloPoint)
                curPointPair = curVTPoint;

            print("[hehe] addVTPoint:" + curVTPoint);

            calibrate();
        }
    }

    public void addHoloPoint(Vector3 holoCmrPoint)
    {
        if (curHoloPoint < requiredPairAmount)
        {
            holoPointSets[curHoloPoint * 3] = holoCmrPoint[0];
            holoPointSets[curHoloPoint * 3 + 1] = holoCmrPoint[1];
            holoPointSets[curHoloPoint * 3 + 2] = holoCmrPoint[2];

            curHoloPoint++;

            if (curVTPoint == curHoloPoint)
                curPointPair = curVTPoint;

            print("[hehe] addHoloPoint:" + curHoloPoint);

            calibrate();
        }
    }


    public void calibrate()
    {
        if ((curPointPair) != requiredPairAmount)
            return;
        

        int size = Marshal.SizeOf(holoPointSets[0]) * holoPointSets.Length;

        IntPtr holopoints = Marshal.AllocHGlobal(Marshal.SizeOf(holoPointSets[0]) * holoPointSets.Length);
        IntPtr vivepoints = Marshal.AllocHGlobal(Marshal.SizeOf(vtPointSets[0]) * vtPointSets.Length);
        IntPtr transform = Marshal.AllocHGlobal(Marshal.SizeOf(calibResult[0]) * calibResult.Length);
        //IntPtr typesize = Marshal.AllocHGlobal(Marshal.SizeOf(debuglength[0]));

        try
        {
            // Copy the array to unmanaged memory.
            Marshal.Copy(holoPointSets, 0, holopoints, holoPointSets.Length);
            Marshal.Copy(vtPointSets, 0, vivepoints, vtPointSets.Length);
            //Marshal.Copy(debuglength, 0, typesize, 1);

            EstimateAffTransSF(holopoints, vivepoints, transform, requiredPairAmount);

            Marshal.Copy(transform, calibResult, 0, 3 * 4);
            //Marshal.Copy(holopoints, holoPointSet, 0, 3 * 4);
            //Marshal.Copy(vivepoints, vivePointSet, 0, 3 * 4);
            //Marshal.Copy(typesize, debuglength, 0, 1);
            //print(debuglength[0]);
        }
        finally
        {
            // Free the unmanaged memory.
            Marshal.FreeHGlobal(holopoints);
            Marshal.FreeHGlobal(vivepoints);
            Marshal.FreeHGlobal(transform);
            //Marshal.FreeHGlobal(typesize);
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                calibResultMat[i + j * 4] = (float)calibResult[i * 4 + j];
            }
        }

        print("calib: " + calibResult);

        bCalib = true;
        GetComponent<StateMgr>().current_state = StateMgr.STATE.manipulation;
    }

    public void TransformFromVT2Holo(Vector3 vt, ref Vector3 holo)
    {
        holo = calibResultMat.MultiplyPoint3x4(vt);
    }

    public void reset()
    {
        curPointPair = 0;
        curHoloPoint = 0;
        curVTPoint = 0;
        calibResult = new double[12];
        holoPointSets = new double[requiredPairAmount * 3];
        vtPointSets = new double[requiredPairAmount * 3];
        bCalib = false;
        calibResultMat = Matrix4x4.identity;
    }
}
