using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class TestDll : MonoBehaviour
{

    public float[] holoPointSet, vivePointSet;
    [SerializeField]
    float[] result;
    // The imported function
    [DllImport("AffinedTransformation", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    [DllImport("AffinedTransformation", EntryPoint = "EstimateAffTransTT")]
    public static extern void EstimateAffTransTT(IntPtr holopoints, IntPtr vivepoints, IntPtr transformation);

    [DllImport("AffinedTransformation", EntryPoint = "EstimateAffTransSF")]
    public static extern void EstimateAffTransSF(IntPtr holopoints, IntPtr vivepoints, IntPtr transformation);

    public int[] a;

    void Start()
    {
        result = new float[12];

        TestSort(a, a.Length);
        
        int size = Marshal.SizeOf(holoPointSet[0]) * holoPointSet.Length;

        IntPtr holopoints = Marshal.AllocHGlobal(Marshal.SizeOf(holoPointSet[0]) * holoPointSet.Length);
        IntPtr vivepoints = Marshal.AllocHGlobal(Marshal.SizeOf(vivePointSet[0]) * vivePointSet.Length);
        IntPtr transform = Marshal.AllocHGlobal(Marshal.SizeOf(result[0]) * result.Length);

        try
        {
            // Copy the array to unmanaged memory.
            Marshal.Copy(holoPointSet, 0, holopoints, holoPointSet.Length);
            Marshal.Copy(vivePointSet, 0, vivepoints, vivePointSet.Length);

            EstimateAffTransTT(holopoints, vivepoints, transform);

            Marshal.Copy(transform, result, 0, 3 * 4);
            Marshal.Copy(holopoints, holoPointSet, 0, 3 * 4);
            Marshal.Copy(vivepoints, vivePointSet, 0, 3 * 4);
        }
        finally
        {
            // Free the unmanaged memory.
            Marshal.FreeHGlobal(holopoints);
            Marshal.FreeHGlobal(vivepoints);
            Marshal.FreeHGlobal(transform);
        }
    }
}