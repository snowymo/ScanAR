using Academy.HoloToolkit.Sharing;
using Academy.HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

public class CustomExtendedMsg : CustomMessages {

  TestMessageID maxid = TestMessageID.Max;

  public TestMessageID generateMID() {
    //maxid = maxid + 1;
    return maxid++ - 1;
  }

  void Start() {
    base.InitializeMessageHandlers();
  }

  public void SendCustomTransform(TestMessageID id, Vector3 position, Quaternion rotation) {
    // If we are connected to a session, broadcast our head info
    if (base.serverConnection != null && base.serverConnection.IsConnected()) {
      // Create an outgoing network message to contain all the info we want to send
      NetworkOutMessage msg = base.CreateMessage((byte)id);

      base.AppendTransform(msg, position, rotation);

      // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
      base.serverConnection.Broadcast(
          msg,
          MessagePriority.Immediate,
          MessageReliability.ReliableOrdered,
          MessageChannel.Avatar);
    }
  }

  void OnDestroy() {
    if (base.serverConnection != null) {
      for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)maxid; index++) {
        base.serverConnection.RemoveListener(index, base.connectionAdapter);
      }
      base.connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
    }
  }
}
