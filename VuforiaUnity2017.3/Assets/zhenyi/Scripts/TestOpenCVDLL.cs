﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class TestOpenCVDLL : MonoBehaviour {

    public double[] holoPointSet, vivePointSet;
    [SerializeField]
    double[] result;
    // The imported function
    [DllImport("AffinedTransformation", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    [DllImport("AffinedTransformation", EntryPoint = "EstimateAffTransTT")]
    public static extern void EstimateAffTransTT(IntPtr holopoints, IntPtr vivepoints, IntPtr transformation, int[] typesize);

    [DllImport("AffinedTransformation", EntryPoint = "EstimateAffTransSF")]
    public static extern void EstimateAffTransSF(IntPtr holopoints, IntPtr vivepoints, IntPtr transformation);

    public int[] a;

    // Use this for initialization
    void Start () {
        result = new double[12];

        TestSort(a, a.Length);

        int size = Marshal.SizeOf(holoPointSet[0]) * holoPointSet.Length;
        int[] debuglength = new int[1];
        debuglength[0] = -1;

        IntPtr holopoints = Marshal.AllocHGlobal(Marshal.SizeOf(holoPointSet[0]) * holoPointSet.Length);
        IntPtr vivepoints = Marshal.AllocHGlobal(Marshal.SizeOf(vivePointSet[0]) * vivePointSet.Length);
        IntPtr transform = Marshal.AllocHGlobal(Marshal.SizeOf(result[0]) * result.Length);
        //IntPtr typesize = Marshal.AllocHGlobal(Marshal.SizeOf(debuglength[0]));

        try
        {
            // Copy the array to unmanaged memory.
            Marshal.Copy(holoPointSet, 0, holopoints, holoPointSet.Length);
            Marshal.Copy(vivePointSet, 0, vivepoints, vivePointSet.Length);
            //Marshal.Copy(debuglength, 0, typesize, 1);

            EstimateAffTransSF(holopoints, vivepoints, transform);

            Marshal.Copy(transform, result, 0, 3 * 4);
            Marshal.Copy(holopoints, holoPointSet, 0, 3 * 4);
            Marshal.Copy(vivepoints, vivePointSet, 0, 3 * 4);
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
