using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibCollection : MonoBehaviour {

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

    public SyncCalib syncCalib;

    bool isSending;

    // TODO: reset, so that we can redo calibration several times

	// Use this for initialization
	void Start () {
        collectedData = new List<Vector3>();
        overlayPos = new List<Vector3>();
        headsetRBPos = new List<Vector3>();
        curLineCount = 0;
        curPointCount = 0;
        isSending = false;
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
        if (isSending)
            return;

        // add to list
        collectedData.Add(objMarker.position);
        headsetRBPos.Add(headsetRB.position);
        
        //TODO: // transfer collectedData to Pheadset based on points and headset
        // use headsetRB's pos and rot to create a matrix and get the inverse and apply to obj.pos to get Pheadset and collect only that

        //print("collect one, now " + collectedData.Count + " points");
        ++curPointCount;
        if(curPointCount == calibPointPerLine)
        {
            curPointCount = 0;
            ++curLineCount;
            if (curLineCount >= calibLineAmount)
            {
                print("already collected " + collectedData.Count + " points");
                syncCollectData();
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
        print("generate new one: " + predefinedPoint.localPosition);
    }

    void syncCollectData()
    {
        if (!isSending)
        {
            //
            syncCalib.SetCollectedData(collectedData, headsetRBPos, overlayPos);
            isSending = true;
        }
    }
}
