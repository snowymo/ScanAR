using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapCalib : MonoBehaviour {

    public Transform objMarker;
    public Transform predefinedPoint;
    public Transform headsetRB;


    public int calibLineAmount;
    public int calibPointPerLine;

    int curLineCount;
    int curPointCount;

    [SerializeField]
    List<Vector3> collectedData;
    [SerializeField]
    List<Vector3> overlayPos;
    [SerializeField]
    List<Vector3> headsetRBPos;

    bool isCalibrated;

	// Use this for initialization
	void Start () {
        collectedData = new List<Vector3>();
        overlayPos = new List<Vector3>();
        headsetRBPos = new List<Vector3>();
        curLineCount = 0;
        curPointCount = 0;
        isCalibrated = false;
        generateNewScreenPoint();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            collectData();
        }

	}

    void collectData()
    {
        if (isCalibrated)
            return;

        // add to list
        collectedData.Add(objMarker.position);
        headsetRBPos.Add(headsetRB.position);
        print("collect one, now " + collectedData.Count + " points");
        ++curPointCount;
        if(curPointCount == calibPointPerLine)
        {
            curPointCount = 0;
            ++curLineCount;
            if (curLineCount >= calibLineAmount)
            {
                print("already collected " + collectedData.Count + " points");
                calibrate();
            }
            else
                // generate a new predefine point
                generateNewScreenPoint();
        }

        
    }

    void generateNewScreenPoint()
    {
        Vector3 randomEulerAngle = new Vector3(Random.Range(0, 20), Random.Range(0, 8),0);
        predefinedPoint.localPosition = Quaternion.Euler(randomEulerAngle) * Vector3.forward;
        if (predefinedPoint.localPosition.z < 0)
            predefinedPoint.localPosition *= -1;
        overlayPos.Add(predefinedPoint.localPosition);
    }

    void calibrate()
    {
        if (!isCalibrated)
        {
            isCalibrated = true;
        }
    }
}
