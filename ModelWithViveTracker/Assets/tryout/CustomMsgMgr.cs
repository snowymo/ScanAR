using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;

public class CustomMsgMgr : MonoBehaviour {

  public Transform modelObj;
  public Transform cameraObj;

  public enum Category { Hololens, HTCVive};

  public Category category;

  CustomMessages customMessages;
//  [SerializeField]
//  CustomMessages.TestMessageID id;

  // Use this for initialization
  void Start () {
    customMessages = CustomMessages.Instance;
    //id = customMessages.generateMID();
    customMessages.MessageHandlers[CustomMessages.TestMessageID.StageTransform] = this.receiveModelTransform;
    customMessages.MessageHandlers[CustomMessages.TestMessageID.Camera] = this.receiveCameraTransform;
  }
	
	// Update is called once per frame
	void Update () {
    if(category == Category.Hololens)
    {
      sendTransform(CustomMessages.TestMessageID.Camera);
    } else {
      sendTransform();
    }
    
	}

  void sendTransform(CustomMessages.TestMessageID id)
  {
    CustomMessages.Instance.SendCustomTransform(id, cameraObj.position, cameraObj.rotation);
    print(cameraObj.position);
  }

  void sendTransform() {
    CustomMessages.Instance.SendStageTransform(modelObj.position, modelObj.rotation);
  }

  void receiveModelTransform(NetworkInMessage msg)
  {
    // Parse the message
    long userID = msg.ReadInt64();

    modelObj.position = customMessages.ReadVector3(msg);

    modelObj.rotation = customMessages.ReadQuaternion(msg);
    
  }

  void receiveCameraTransform(NetworkInMessage msg) {
    // Parse the message
    long userID = msg.ReadInt64();

    cameraObj.position = customMessages.ReadVector3(msg);

    cameraObj.rotation = customMessages.ReadQuaternion(msg);

  }
}
