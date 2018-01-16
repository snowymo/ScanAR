using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class PLYLoader : MonoBehaviour {

    // The imported function
    [DllImport("PLYDecoder", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    [DllImport("PLYDecoder", EntryPoint = "LoadPLY")]
    public static extern void LoadPLY(IntPtr F, IntPtr V, IntPtr N, int[] meshSize);

    public int[] a;

    [SerializeField]
    private int[] cnt;
    [SerializeField]
    private double[] V, N;
    [SerializeField]
    private int[] F;
    // Use this for initialization
    void Start () {
        TestSort(a, a.Length);
    }

    public void loadply()
    {
        cnt = new int[3];
        IntPtr pF = Marshal.AllocHGlobal(sizeof(int) * 3);
        IntPtr pV = Marshal.AllocHGlobal(sizeof(double) * 1);
        IntPtr pN = Marshal.AllocHGlobal(sizeof(double) * 1);
        //IntPtr pCnt = Marshal.AllocHGlobal(sizeof(int) * 3);
        //IntPtr typesize = Marshal.AllocHGlobal(Marshal.SizeOf(debuglength[0]));
        //int pCnt = 4;
        try
        {
            // Copy the array to unmanaged memory.
            //Marshal.Copy(holoPointSet, 0, holopoints, holoPointSet.Length);
            //Marshal.Copy(vivePointSet, 0, vivepoints, vivePointSet.Length);
            //Marshal.Copy(debuglength, 0, typesize, 1);

            LoadPLY(pF, pV, pN, cnt);

            F = new int[cnt[0]];
            Marshal.Copy(pF, F, 0, cnt[0]);
            V = new double[cnt[1]];
            Marshal.Copy(pV, V, 0, cnt[1]);
            N = new double[cnt[2]];
            Marshal.Copy(pN, N, 0, cnt[2]);

            //Marshal.Copy(typesize, debuglength, 0, 1);
            //print(debuglength[0]);
        }
        finally
        {
            // Free the unmanaged memory.
            Marshal.FreeHGlobal(pF);
            Marshal.FreeHGlobal(pV);
            Marshal.FreeHGlobal(pN);
            //Marshal.FreeHGlobal(typesize);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
