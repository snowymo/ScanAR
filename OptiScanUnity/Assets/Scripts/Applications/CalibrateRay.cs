using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateRay : MonoBehaviour {

    [SerializeField]
    Vector3 pIntersection;

    [SerializeField]
    Quaternion rotationOffset;

    [SerializeField]
    Vector3[] ks;

    [SerializeField]
    Vector3[] pCens;

    // Use this for initialization
    void Start () {
        pIntersection = Vector3.zero;
        rotationOffset = Quaternion.identity;
        
}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getResult(ref Vector3 p, ref Quaternion q)
    {
        p = pIntersection;
        q = rotationOffset;
    }

    public void CalibrateRays(Vector3[] collectedData, int lineAmount, int pointPerLine)
    {
        print("collect data:");
        for(int i = 0;i < collectedData.Length; i++)
            print(collectedData[i]);
        // split headsets data and screen point data from collected data
        Vector3[] vHeadsets = new Vector3[lineAmount * pointPerLine];
        Vector3[] vCmrs = new Vector3[lineAmount];

        for(int i = 0; i < lineAmount; i++)
        {
            for(int j = 0; j < pointPerLine; j++)
            {
                vHeadsets[i * pointPerLine + j] = collectedData[i * pointPerLine + j];
            }
            vCmrs[i] = collectedData[lineAmount * pointPerLine + i];
        }

        // fit a line
        ks = new Vector3[lineAmount];
        pCens = new Vector3[lineAmount];
        for (int i = 0; i < lineAmount; i++)
        {
            Utility.FitLine(collectedData, i * (pointPerLine), (i + 1) * (pointPerLine), ref pCens[i], ref ks[i]);
            print("k:\t" + ks[i]);
            //print("pCens:\t" + pCens[i]);
        }
        
        // calculate intersection points
        Utility.CalIntersectionPoint(ks, pCens, ref pIntersection);
        print("pIntersection:" + pIntersection);

        // calculate rotation offset
        Utility.CalRotationOffset(pIntersection, vHeadsets, vCmrs, ref rotationOffset);
        print("rotationOffset:" + rotationOffset);
    }
}
