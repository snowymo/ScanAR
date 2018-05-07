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

    public GameObject cube;

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
            // during testing, I randomize / or manually move marker's position
            collectData();
        }

	}

    void testRelatedPositionFunc(Vector3 headsetRelative)
    {
        // create a debug obj to see if it is correct
        GameObject go = GameObject.Instantiate(cube, headsetRB);
        go.transform.localPosition = headsetRelative;
        go.name = "see";
    }

    void collectData()
    {
        if (isSending)
            return;

        // use headsetRB's pos and rot to create a matrix and get the inverse and apply to obj.pos to get Pheadset and collect only that
        Vector3 headsetRelative = headsetRB.InverseTransformPoint(objMarker.position);
        //testRelatedPositionFunc(headsetRelative);
        
        // add to list
        collectedData.Add(headsetRelative);
        //headsetRBPos.Add(headsetRB.position);

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
        Vector3 randomEulerAngle = new Vector3(Random.Range(0, 15), Random.Range(0, 8),0);
        predefinedPoint.localPosition = Quaternion.Euler(randomEulerAngle) * Vector3.forward * 0.5f;
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
            syncCalib.SetCollectedData(collectedData, overlayPos);
            isSending = true;
        }
    }
}
