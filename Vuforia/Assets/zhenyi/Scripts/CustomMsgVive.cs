using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;
using System;

public class CustomMsgVive : MonoBehaviour
{

    Transform modelObj;
    Transform cameraObj;
    public Mesh mesh;

    public MeshRender[] mrs;

    public enum Category { Hololens, Scanner };

    [SerializeField]
    public Category category;

    CustomMessages customMessages;

    public KeyboardHandler tuner;

    public Transform holoController, holoController2;
    public SteamVR_TrackedObject vivecontroller;

    [SerializeField]
    float roundtrip, startTime, midTime, delta1;

    [SerializeField]
    int meshId;
    //  [SerializeField]
    //  CustomMessages.TestMessageID id;

    // Use this for initialization
    void Awake()
    {
        customMessages = CustomMessages.Instance;
        //id = customMessages.generateMID();
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Mesh] = this.receiveMeshWithTime;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Time] = this.receiveTime;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Tuner] = this.receiveTuner;
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Alignment] = this.receiveAlignTransform;
/*        customMessages.MessageHandlers[CustomMessages.TestMessageID.Alignment2] = this.receiveAlignTransform2;*/

        switch (UnityEngine.VR.VRSettings.loadedDeviceName)
        {
            case "HoloLens":
                category = Category.Hololens;
                break;
            default:
                category = Category.Scanner;
                break;
        }
        if (category == Category.Hololens)
            meshId = -1;
        else
            meshId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (category == Category.Scanner)
        {
            sendTuner(CustomMessages.TestMessageID.Tuner);
            sendAlignTransform(CustomMessages.TestMessageID.Alignment);
/*            sendAlignTransform2(CustomMessages.TestMessageID.Alignment2);*/
        }
    }
    

    public void PassMesh()
    {
        startTime = Time.time;
        sendMesh(CustomMessages.TestMessageID.Mesh);
        meshId++;
    }

    void sendTransform(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendCustomTransform(id, cameraObj.position, cameraObj.rotation);
        print(cameraObj.position);
    }

    void sendMesh(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendMeshWithTime(meshId, mesh, startTime);

    }

    void sendTransform()
    {
        CustomMessages.Instance.SendStageTransform(modelObj.position, modelObj.rotation);
    }
    
    void sendTuner(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendTuner(id, tuner.tunerTranslation, tuner.tunerEulerAngle, tuner.tunerScale);
    }

    void receiveTime(NetworkInMessage msg)
    {

        long userId = msg.ReadInt64();

        float midTime = msg.ReadFloat();
        print("receive time:" + midTime);
        roundtrip = Time.time - startTime;

        delta1 = midTime - startTime;
    }

    void receiveModelTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        modelObj.position = customMessages.ReadVector3(msg);

        modelObj.rotation = customMessages.ReadQuaternion(msg);

    }

    void receiveAlignTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        holoController.localPosition = customMessages.ReadVector3(msg);
        holoController.localRotation = customMessages.ReadQuaternion(msg);
    }

//     void receiveAlignTransform2(NetworkInMessage msg)
//     {
//         // Parse the message
//         long userID = msg.ReadInt64();
// 
//         holoController2.localPosition = customMessages.ReadVector3(msg);
//         holoController2.localRotation = customMessages.ReadQuaternion(msg);
//     }

    void receiveMesh(NetworkInMessage msg)
    {
        print("receive msg size:" + msg.GetSize());
        // Parse the message
        long userID = msg.ReadInt64();

        int mid = msg.ReadInt32();
        if (mid == meshId)
            return;

        meshId = mid;

        int triangleLen = msg.ReadInt32();
        int[] triangles = new int[triangleLen];
        for (int i = 0; i < triangleLen; i++)
            triangles[i] = msg.ReadInt32();

        int verticeLen = msg.ReadInt32();
        Vector3[] vertices = new Vector3[verticeLen];
        for (int i = 0; i < verticeLen; i++)
            vertices[i] = customMessages.ReadVector3(msg);
        foreach (MeshRender mr in mrs)
            mr.updateMesh(triangles, vertices);
    }

    void receiveMeshWithTime(NetworkInMessage msg)
    {
        print("receive msg size:" + msg.GetSize());
        // Parse the message
        long userID = msg.ReadInt64();

        int mid = msg.ReadInt32();
        if (mid == meshId)
            return;

        meshId = mid;

        midTime = msg.ReadFloat(); // start time

        int triangleLen = msg.ReadInt32();
        int[] triangles = new int[triangleLen];
        for (int i = 0; i < triangleLen; i++)
            triangles[i] = msg.ReadInt32();

        int verticeLen = msg.ReadInt32();
        Vector3[] vertices = new Vector3[verticeLen];
        for (int i = 0; i < verticeLen; i++)
            vertices[i] = customMessages.ReadVector3(msg);
        foreach (MeshRender mr in mrs)
            mr.updateMesh(triangles, vertices);

        CustomMessages.Instance.SendTime(CustomMessages.TestMessageID.Time, Time.time);
    }


    void receiveTuner(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();
        tuner.tunerTranslation = customMessages.ReadVector3(msg);
        tuner.tunerEulerAngle = customMessages.ReadVector3(msg);
        tuner.tunerScale = msg.ReadFloat();
    }

    private void sendAlignTransform(CustomMessages.TestMessageID id)
    {
        if(GetComponent<StateMgr>().current_state == StateMgr.STATE.manipulation)
        {
            // pass vive controller's information here and apply transformation based on alignment data and send out
            GetComponent<AlignmentMgr>().CalculateTransform(vivecontroller.gameObject.transform, ref holoController);
            CustomMessages.Instance.SendAlignedTransform(id, holoController.localPosition, holoController.localRotation, holoController.localScale[0]);
            //GetComponent<AlignmentMgr>().CalculateTransform2(vivecontroller.gameObject.transform, ref holoController2);
        }
        
    }

//     private void sendAlignTransform2(CustomMessages.TestMessageID id)
//     {
//         if (GetComponent<StateMgr>().current_state == StateMgr.STATE.manipulation)
//         {
//             // pass vive controller's information here and apply transformation based on alignment data and send out
//             GetComponent<AlignmentMgr>().CalculateTransform2(vivecontroller.gameObject.transform, ref holoController2);
//             CustomMessages.Instance.SendAlignedTransform(id, holoController2.localPosition, holoController2.localRotation, holoController2.localScale[0]);
//             //GetComponent<AlignmentMgr>().CalculateTransform2(vivecontroller.gameObject.transform, ref holoController2);
//         }
// 
//     }

}
