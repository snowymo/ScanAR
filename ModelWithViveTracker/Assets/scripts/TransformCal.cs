using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCal : MonoBehaviour {

    public Transform modelRelatedMatrix;
    public Transform headsetRelatedMatrix;
    public Transform modelTransform;

    bool bCalibrated;

    Matrix4x4 lastModel, curModel, lastHeadset, curHeadset;

	// Use this for initialization
	void Start () {
        bCalibrated = false;
        lastModel = Matrix4x4.identity;
        lastHeadset = Matrix4x4.identity;

    }

    void calibrate()
    {
        // when key space is pressed
        // do the calibration to calculate the missing matrix
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bCalibrated = true;
        }

        if (bCalibrated)
        {
            // apply the difference of vivetracker1.m
            // apply the inverse of vivetracker2.m
            curModel.SetTRS(modelRelatedMatrix.position, modelRelatedMatrix.rotation, modelRelatedMatrix.localScale);
            if (!lastModel.isIdentity)
            {
                Matrix4x4 modelbtw = curModel * lastModel.inverse ;
                Matrix4x4 modelMatrix = Matrix4x4.identity;
                modelMatrix.SetTRS(modelTransform.position, modelTransform.rotation, modelTransform.localScale);
                modelMatrix = modelbtw * modelMatrix;
                modelTransform.position = modelMatrix.GetPosition();
                modelTransform.rotation = modelMatrix.GetRotation();
            }
            lastModel = curModel;
        }
    }
}
