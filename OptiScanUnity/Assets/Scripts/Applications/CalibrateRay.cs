using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateRay : MonoBehaviour {

    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CalibrateRays(Vector3[] collectedData, int lineAmount, int pointPerLine)
    {
        Vector3[] ks = new Vector3[lineAmount];
        for (int i = 0; i < lineAmount; i++)
        {
            ks[i] = Utility.FitLine(collectedData, i * (pointPerLine), (i + 1) * (pointPerLine));
        }
        
    }
}
