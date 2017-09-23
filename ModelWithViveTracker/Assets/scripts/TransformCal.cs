using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCal : MonoBehaviour {

    public Transform modelRelatedMatrix;
    public Transform headsetRelatedMatrix;
    public Transform modelTransform;

    public bool bCalibrated;
    public bool bHide;

    Matrix4x4 lastModel, curModel, lastHeadset, curHeadset;

	// Use this for initialization
	void Start () {
        bCalibrated = false;
        bHide = false;
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
            curHeadset.SetTRS(headsetRelatedMatrix.position, headsetRelatedMatrix.rotation, headsetRelatedMatrix.localScale);
            if (!lastModel.isIdentity/* && !lastHeadset.isIdentity*/)
            {
                Matrix4x4 modelMatrix = Matrix4x4.identity;
                if (bHide)
                    modelTransform.position -= new Vector3(1, 1, 1);
                modelMatrix.SetTRS(modelTransform.position, modelTransform.rotation, modelTransform.localScale);

                //Matrix4x4 headsetbtw = curHeadset * lastHeadset.inverse;
                //modelMatrix = headsetbtw.inverse * modelMatrix;

                Matrix4x4 modelbtw = curModel * lastModel.inverse;
                modelMatrix = modelbtw * modelMatrix;

                modelTransform.position = modelMatrix.GetPosition();
                if (bHide)
                    modelTransform.position += new Vector3(1, 1, 1);
                modelTransform.rotation = modelMatrix.GetRotation();
            }
            lastModel = curModel;
            lastHeadset = curHeadset;
        }

        if (bHide)
            modelTransform.position = new Vector3(100, 100, 100);
    }
}
