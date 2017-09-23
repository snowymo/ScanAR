using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour {

    public Transform hololensCamera;
    Matrix4x4 mHCamera, mHCameraPrev;
    public Transform hololensModel;
    Matrix4x4 mHModel;
    public Transform ViveTrackerModel;
    Matrix4x4 mVTModel, mVTModelPrev;
    public Transform ViveTrackerCamera;
    Matrix4x4 mVTCamera, mVTCameraPrev;
    public bool bCalib;

    Matrix4x4 VTModelPrev;
    Matrix4x4 HModelPrev;
    

	// Use this for initialization
	void Start () {
        hololensModel.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        bCalib = false;
    }
	
	// Update is called once per frame
	void Update () {
        mHCamera = Matrix4x4.TRS(hololensCamera.position, hololensCamera.rotation, new Vector3(1, 1, 1));
        mHModel = Matrix4x4.TRS(hololensModel.position, hololensModel.rotation, new Vector3(1, 1, 1));
        mVTModel = Matrix4x4.TRS(ViveTrackerModel.position, ViveTrackerModel.rotation, new Vector3(1, 1, 1));
        mVTCamera = Matrix4x4.TRS(ViveTrackerCamera.position, ViveTrackerCamera.rotation, new Vector3(1, 1, 1));

        print("mHModel:" + mHModel);
        calibrate();

        VTModelPrev = mVTModel;
        HModelPrev = mHModel;
        mHCameraPrev = mHCamera;
        mVTModelPrev = mVTModel;
        mVTCameraPrev = mVTCamera;

        hololensModel.SetPositionAndRotation(mHModel.GetPosition(), mHModel.GetRotation());
    }

    Matrix4x4 filter(Matrix4x4 m, Matrix4x4 mRef)
    {
        float deltaP = Vector3.Distance(m.GetPosition(), mRef.GetPosition());
        float deltaQ = Quaternion.Angle(m.GetRotation(), mRef.GetRotation());
        Matrix4x4 returnM = m;
        /*print("m:" + m + "\tmRef:" + mRef);*/
        print("deltaP:" + deltaP + "\tdeltaQ:" + deltaQ);

        if (deltaP < 0.001)
            returnM.SetColumn(3, mRef.GetColumn(3));
        if (deltaQ < 1)
        {
            returnM.SetColumn(0, mRef.GetColumn(0));
            returnM.SetColumn(1, mRef.GetColumn(1));
            returnM.SetColumn(2, mRef.GetColumn(2));
        }
        return returnM;
    }

    void calibrate()
    {
        if (bCalib)
        {
            //print("VTModelPrev:" + VTModelPrev);

            mHCamera = filter(mHCamera, mHCameraPrev);
            mVTCamera = filter(mVTCamera, mVTCameraPrev);
            mVTModel = filter(mVTModel, mVTModelPrev);

            print("HModelPrev:" + HModelPrev);

            //H T-1 Tc'(Tc-1 T) H-1 Hc
            Matrix4x4 refMatrix = mHCamera * mVTCamera.inverse * mVTModel * VTModelPrev.inverse * mVTCamera * mHCamera.inverse;
            print("refMatrix: " + refMatrix);
            mHModel = mHCamera * mVTCamera.inverse * mVTModel * VTModelPrev.inverse * mVTCamera * mHCamera.inverse * HModelPrev;
//             print("mHCamera" + mHCamera);
//             print("mVTCamera" + mVTCamera);
//             print("mVTModel" + mVTModel);
// 
//             print("HModelPrev" + HModelPrev);
//             print("VTModelPrev" + VTModelPrev);
        }        
    }
}
