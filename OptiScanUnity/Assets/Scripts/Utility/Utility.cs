using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public static class Utility {

    [DllImport("svd", EntryPoint = "Calib")]
    private static extern void Calib(float[] a, float[] b, int len, float[] transform);

    [DllImport("svd", EntryPoint = "SVD")]
    private static extern void SVD(float[] a, int len, float[] k);

    [DllImport("svd", EntryPoint = "CalIntersection")]
    private static extern void CalIntersection(float[] cens, float[] ks, int len, float[] intersection);

    [DllImport("svd", EntryPoint = "CalRotation")]
    private static extern void CalRotation(float[] tOffset, float[] pHeadsets, float[] pCmrs, int lineAmt, int pPerLine, float[] rotation);

    public static void FitLine(Vector3[] points/*pHeadset*/, int startIdx, int endIdx, ref Vector3 pCenter, ref Vector3 k) 
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
            fH_mean[i*3] = (float)Math.Round(pH_mean.x, 4);
            fH_mean[i * 3 + 1] = (float)Math.Round(pH_mean.y,4);
            fH_mean[i * 3 + 2] = (float)Math.Round(pH_mean.z,4);
        }

        //do svd
        float[] fk = new float[3];
        SVD(fH_mean, endIdx - startIdx, fk);
        k = new Vector3(fk[0], fk[1], fk[2]);
        pCenter = pMean;
    }

    public static void CalIntersectionPoint(Vector3[] vKs, Vector3[] vCenters, ref Vector3 pIntersection)
    {
        // prepare floats
        int len = vKs.Length;
        float[] cens = new float[len * 3];
        float[] ks = new float[len * 3];
        for (int i = 0; i < len; i++)
        {
            cens[3 * i] = (float)Math.Round(vCenters[i].x,4);
            cens[3 * i + 1] = (float)Math.Round(vCenters[i].y,4);
            cens[3 * i + 2] = (float)Math.Round(vCenters[i].z,4);
            ks[3 * i] = (float)Math.Round(vKs[i].x,4);
            ks[3 * i + 1] = (float)Math.Round(vKs[i].y,4);
            ks[3 * i + 2] = (float)Math.Round(vKs[i].z,4);
        }
        float[] intersection = new float[3];
        CalIntersection(cens, ks, len, intersection);
        pIntersection = new Vector3(intersection[0], intersection[1], intersection[2]);
    }

    public static void CalRotationOffset(Vector3 vOffset, Vector3[] vHeadsets, Vector3[] vCmrs, ref Quaternion rotationOffset)
    {
        // prepare floats
        float[] tOffset = new float[3];
        tOffset[0] = vOffset.x;
        tOffset[1] = vOffset.y;
        tOffset[2] = vOffset.z;

        float[] pHeadsets = new float[vHeadsets.Length * 3];
        for(int i = 0; i < vHeadsets.Length; i++)
        {
            pHeadsets[i * 3] = vHeadsets[i].x;
            pHeadsets[i * 3 + 1] = vHeadsets[i].y;
            pHeadsets[i * 3 + 2] = vHeadsets[i].z;
        }

        float[] pCmrs = new float[vCmrs.Length * 3];
        for (int i = 0; i < vCmrs.Length; i++)
        {
            pCmrs[i * 3] = vCmrs[i].x;
            pCmrs[i * 3 + 1] = vCmrs[i].y;
            pCmrs[i * 3 + 2] = vCmrs[i].z;
        }
        float[] rotation = new float[9];
        CalRotation(tOffset, pHeadsets, pCmrs, vCmrs.Length, vHeadsets.Length/vCmrs.Length, rotation);
        Matrix4x4 m = Matrix4x4.identity;
        for(int i = 0; i < 9; i++)
            m[i/3, i%3] = rotation[i];
        rotationOffset = m.rotation;
    }
}
