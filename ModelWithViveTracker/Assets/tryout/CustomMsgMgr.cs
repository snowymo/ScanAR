using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;

public class CustomMsgMgr : MonoBehaviour {

  public Transform testObj;

  public enum Category { sender, receiver};

  public Category category;

  CustomMessages customMessages;

  // Use this for initialization
  void Start () {
    customMessages = CustomMessages.Instance;
    customMessages.MessageHandlers[CustomMessages.TestMessageID.StageTransform] = this.receiveTransform;
  }
	
	// Update is called once per frame
	void Update () {
    if(category == Category.sender)
    {
      sendTransform();
    }
    
	}

  void sendTransform()
  {
    CustomMessages.Instance.SendStageTransform(testObj.position, testObj.rotation);
    print(testObj.position);
  }

  void receiveTransform(NetworkInMessage msg)
  {
    // Parse the message
    long userID = msg.ReadInt64();

    testObj.position = customMessages.ReadVector3(msg);

    testObj.rotation = customMessages.ReadQuaternion(msg);
    
  }
}
