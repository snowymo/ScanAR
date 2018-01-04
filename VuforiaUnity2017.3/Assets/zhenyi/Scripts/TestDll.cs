using UnityEngine;
using System.Runtime.InteropServices;

public class TestDll : MonoBehaviour
{
    // The imported function
    [DllImport("AffinedTransformation", EntryPoint = "TestSort")]
    public static extern void TestSort(int[] a, int length);

    public int[] a;

    void Start()
    {
        TestSort(a, a.Length);
    }
}