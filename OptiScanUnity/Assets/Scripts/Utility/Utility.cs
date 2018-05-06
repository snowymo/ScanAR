using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

    public static void FitLine(List<Vector3> points, int startIdx, int endIdx) 
    {
        // calculate mean
        Vector3 pMean = Vector3.zero;
        for(int i = startIdx; i < endIdx; i++)
        {
            pMean += points[i];
        }
        pMean /= (endIdx - startIdx);

        //do svd
    }
}
