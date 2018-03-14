using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;
using System;
//using System.IO;


public class CusMsg : MonoBehaviour
{

    CustomMessages customMessages;

    public Transform modelObj;

    public bool isHololens;

    [SerializeField]
    bool currentPositionTracking;

    public Transform[] markers4;
    public Transform cameraObj, hllcameraObj;

    public Matrix4x4 calibMatrix;

    // Use this for initialization
    void Start()
    {
        customMessages = CustomMessages.Instance;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Transform] = this.receiveTransform;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.MARKERS4] = this.receiveMarkers4;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.CAMERA] = this.receiveCamera;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.CALIB] = this.receiveCalib;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.HLLCMR] = this.receiveHLLCmr;

        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        currentPositionTracking = UnityEngine.XR.InputTracking.disablePositionalTracking;

        calibMatrix = Matrix4x4.identity;
    }

    // Update is called once per frame
    void Update()
    {
        //switch (UnityEngine.XR.XRSettings.loadedDeviceName)
        if (!isHololens)
        {
            sendTransform(CustomMessages.TestMessageID.Transform);
            sendMarkers4(CustomMessages.TestMessageID.MARKERS4);
            sendCamera(CustomMessages.TestMessageID.CAMERA);
        }
        else
        {
            sendHololensCmr(CustomMessages.TestMessageID.HLLCMR);
        }


    }

    private void sendTransform(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendTransform(id, modelObj.position, modelObj.rotation);
    }

    private void sendMarkers4(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendMarkers4(id, markers4);
    }

    private void sendCamera(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendOptiCmr(id, cameraObj.position, cameraObj.rotation);
    }

    public void SendMatrix(Matrix4x4 m)
    {
        CustomMessages.Instance.SendMatrix(CustomMessages.TestMessageID.CALIB, m);
    }

    private void sendHololensCmr(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendHLLCmr(id, Camera.main.transform.position, Camera.main.transform.rotation);
    }

    void receiveTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        modelObj.position = customMessages.ReadVector3(msg);
        modelObj.rotation = customMessages.ReadQuaternion(msg);
    }

    void receiveMarkers4(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        for (int i = 0; i < markers4.Length; i++)
        {
            markers4[i].position = customMessages.ReadVector3(msg);
        }
    }

    void receiveCamera(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 vec3 = customMessages.ReadVector3(msg);
        //vec3.x *= -1f;
        cameraObj.position = vec3;
        Quaternion rot = customMessages.ReadQuaternion(msg);
        //rot.x *= -1;
        //rot.w *= -1;
        cameraObj.rotation = rot * Quaternion.Inverse(Camera.main.transform.localRotation);
    }

    void receiveCalib(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        // matrix
        calibMatrix = customMessages.ReadMatrix(msg);

//         if (!isHololens)
//         {
//             // write down
//             string path = "Assets/Resources/calib.txt";
// 
//             //Write some text to the test.txt file
//             StreamWriter writer = new StreamWriter(path, true);
//             writer.WriteLine(calibMatrix);
//             writer.Close();
//         }
    }

    void receiveHLLCmr(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 vec3 = customMessages.ReadVector3(msg);
        //vec3.x *= -1f;
        hllcameraObj.position = vec3;
        Quaternion rot = customMessages.ReadQuaternion(msg);
        //rot.x *= -1;
        //rot.w *= -1;
        hllcameraObj.rotation = rot;
    }

}
