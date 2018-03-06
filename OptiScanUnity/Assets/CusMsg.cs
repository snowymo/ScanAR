using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;
using System;

public class CusMsg : MonoBehaviour {

    CustomMessages customMessages;

    public Transform modelObj;

    public bool isHololens;

    [SerializeField]
    bool currentPositionTracking;

    // Use this for initialization
    void Start () {
        customMessages = CustomMessages.Instance;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Transform] = this.receiveTransform;

        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        currentPositionTracking = UnityEngine.XR.InputTracking.disablePositionalTracking;
    }
	
	// Update is called once per frame
	void Update () {
        //switch (UnityEngine.XR.XRSettings.loadedDeviceName)
       if(!isHololens)
            sendTransform(CustomMessages.TestMessageID.Transform);
               
	}

    private void sendTransform(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendTransform(id, modelObj.position, modelObj.rotation);
    }

    void receiveTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        modelObj.position = customMessages.ReadVector3(msg);
        modelObj.rotation = customMessages.ReadQuaternion(msg);
    }
}
