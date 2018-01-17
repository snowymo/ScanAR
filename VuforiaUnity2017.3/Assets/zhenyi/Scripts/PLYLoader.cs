using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class PLYLoader : MonoBehaviour {

    // The imported function
    [DllImport("PLYDecoder", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    [DllImport("PLYDecoder", EntryPoint = "LoadPLY")]
    public static extern void LoadPLY(IntPtr F, IntPtr V, IntPtr N, int[] meshSize);

    [DllImport("PLYDecoder", EntryPoint = "LoadPLY2")]
    public static extern void LoadPLY2(out IntPtr F, out IntPtr V, out IntPtr N, int[] meshSize);

    [DllImport("PLYDecoder", EntryPoint = "LoadPLY3")]
    public static extern void LoadPLY3([MarshalAs(UnmanagedType.LPStr)] string path, IntPtr[] F, IntPtr[] V, int[] meshSize);

    public int[] a;

    public string scanPath;

    [SerializeField]
    private int[] cnt;
    [SerializeField]
    private float[] V, N;
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
            V = new float[cnt[1]];
            Marshal.Copy(pV, V, 0, cnt[1]);
            N = new float[cnt[2]];
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

    public void loadPLY2()
    {
        cnt = new int[3];
        IntPtr pF = new IntPtr();
        IntPtr pV = new IntPtr();
        IntPtr pN = new IntPtr();
        //IntPtr pCnt = Marshal.AllocHGlobal(sizeof(int) * 3);
        //IntPtr typesize = Marshal.AllocHGlobal(Marshal.SizeOf(debuglength[0]));
        //int pCnt = 4;
        try
        {
            // Copy the array to unmanaged memory.
            //Marshal.Copy(holoPointSet, 0, holopoints, holoPointSet.Length);
            //Marshal.Copy(vivePointSet, 0, vivepoints, vivePointSet.Length);
            //Marshal.Copy(debuglength, 0, typesize, 1);

            LoadPLY2(out pF, out pV, out pN, cnt);

            F = new int[cnt[0]];
            Marshal.Copy(pF, F, 0, cnt[0]);
            V = new float[cnt[1]];
            Marshal.Copy(pV, V, 0, cnt[1]);
            N = new float[cnt[2]];
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

    public void loadPLY3()
    {
        cnt = new int[2];
        //IntPtr[] pFVN = new IntPtr[2];
        IntPtr[] pF = new IntPtr[1];
        pF[0] = new IntPtr(0);
        IntPtr[] pV = new IntPtr[1];
        pV[0] = new IntPtr(0);
        //         for (int i = 0; i < pFVN.Length; i++)
        //         {
        //             pFVN[i] = new IntPtr();
        //             cnt[i] = 0;
        //         }

        //         IntPtr pF = new IntPtr();
        //         IntPtr pV = new IntPtr();
        //         IntPtr pN = new IntPtr();
        try
        {
            //StringBuilder sb = new StringBuilder(scanPath);
            LoadPLY3(scanPath, pF, pV, cnt);

            if(cnt[0] > 0)
            {
                F = new int[cnt[0]];
                Marshal.Copy(pF[0], F, 0, cnt[0]);
            }
            if(cnt[1] > 0)
            {
                V = new float[cnt[1]];
                Marshal.Copy(pV[0], V, 0, cnt[1]);
            }
//             if(cnt[2] > 0)
//             {
//                 N = new double[cnt[2]];
//                 Marshal.Copy(pFVN[2], N, 0, cnt[2]);
//             }
        }
        finally
        {
            // Free the unmanaged memory.
            //foreach(IntPtr ip in pFVN)
                Marshal.FreeHGlobal(pF[0]);
            Marshal.FreeHGlobal(pV[0]);
            //Marshal.FreeHGlobal(typesize);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
