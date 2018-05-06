using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public static class Utility {

    [DllImport("svd", EntryPoint = "Calib")]
    private static extern void Calib(float[] a, float[] b, int len, float[] transform);

    [DllImport("svd", EntryPoint = "SVD")]
    private static extern void SVD(float[] a, int len, float[] k);

    public static Vector3 FitLine(Vector3[] points/*pHeadset*/, int startIdx, int endIdx) 
    {
        // calculate mean
        Vector3 pMean = Vector3.zero;
        for(int i = startIdx; i < endIdx; i++)
        {
            pMean += points[i];
        }
        pMean /= (endIdx - startIdx);
        float[] fH_mean = new float[(endIdx - startIdx) * 3];
        for (int i = 0; i < endIdx- startIdx; i++)
        {
            Vector3 pH_mean = points[i+ startIdx] - pMean;
            fH_mean[i*3] = pH_mean.x;
            fH_mean[i * 3 + 1] = pH_mean.y;
            fH_mean[i * 3 + 2] = pH_mean.z;
        }

        //do svd
        float[] k = new float[3];
        SVD(fH_mean, endIdx - startIdx, k);
        Vector3 ret = new Vector3(k[0], k[1], k[2]);
        return ret;
    }
}
