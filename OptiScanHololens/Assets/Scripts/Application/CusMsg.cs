using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;
using System;

public class CusMsg : MonoBehaviour
{

    CustomMessages customMessages;

    public Transform modelObj;

    public bool isHololens;

    [SerializeField]
    bool currentPositionTracking;

    public Transform[] markers4;
    public Transform cameraObj;

    // Use this for initialization
    void Start()
    {
        customMessages = CustomMessages.Instance;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Transform] = this.receiveTransform;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.MARKERS4] = this.receiveMarkers4;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.CAMERA] = this.receiveCamera;

        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        currentPositionTracking = UnityEngine.XR.InputTracking.disablePositionalTracking;
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


    }

    private void sendTransform(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendTransform(id, modelObj.position, modelObj.rotation);
    }

    private void sendMarkers4(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendMarkers(id, markers4);
    }

    private void sendCamera(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendMarkers(id, cameraObj.position, cameraObj.rotation);
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

        cameraObj.position = customMessages.ReadVector3(msg);
        cameraObj.rotation = customMessages.ReadQuaternion(msg);
    }
}
