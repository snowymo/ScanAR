using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;

public class CustomMsgMgr : MonoBehaviour
{

     Transform modelObj;
     Transform cameraObj;
    public Mesh mesh;

    public MeshRender mr;

    public enum Category { Hololens, Scanner };

    public Category category;

    CustomMessages customMessages;
    //  [SerializeField]
    //  CustomMessages.TestMessageID id;

    // Use this for initialization
    void Start()
    {
        customMessages = CustomMessages.Instance;
        //id = customMessages.generateMID();
        customMessages.MessageHandlers[CustomMessages.TestMessageID.Mesh] = this.receiveMesh;
        //customMessages.MessageHandlers[CustomMessages.TestMessageID.Camera] = this.receiveCameraTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (category == Category.Hololens)
        {
            //sendTransform(CustomMessages.TestMessageID.Camera);
        }
        else
        {
            sendMesh(CustomMessages.TestMessageID.Mesh);
        }

    }

    void sendTransform(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendCustomTransform(id, cameraObj.position, cameraObj.rotation);
        print(cameraObj.position);
    }

    void sendMesh(CustomMessages.TestMessageID id)
    {
        CustomMessages.Instance.SendMesh(mesh);
        
    }

    void sendTransform()
    {
        CustomMessages.Instance.SendStageTransform(modelObj.position, modelObj.rotation);
    }

    void receiveModelTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        modelObj.position = customMessages.ReadVector3(msg);

        modelObj.rotation = customMessages.ReadQuaternion(msg);

    }

    void receiveMesh(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        int triangleLen = msg.ReadInt32();
        int[] triangles = new int[triangleLen];
        for (int i = 0; i < triangleLen; i++)
            triangles[i] = msg.ReadInt32();

        int verticeLen = msg.ReadInt32();
        Vector3[] vertices = new Vector3[verticeLen];
        for (int i = 0; i < verticeLen; i++)
            vertices[i] = customMessages.ReadVector3(msg);

        mr.updateMesh(triangles, vertices);
    }

    void receiveCameraTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        cameraObj.position = customMessages.ReadVector3(msg);

        cameraObj.rotation = customMessages.ReadQuaternion(msg);

    }
}
