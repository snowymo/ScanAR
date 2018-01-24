using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class zmqClient : MonoBehaviour {

    public string msg;
    public float[] cmd,fm;

    private NetMqListener _netMqListener;

    public string topic;

    public CustomMsgVive cmv;

    private void HandleMessage(string message)
    {
        //         var splittedStrings = message.Split(' ');
        //         if (splittedStrings.Length != 3) return;
        //         var x = float.Parse(splittedStrings[0]);
        //         var y = float.Parse(splittedStrings[1]);
        //         var z = float.Parse(splittedStrings[2]);
        //         transform.position = new Vector3(x, y, z);
        msg = message;
    }

    private void HandleFMessage(byte[] b)
    {
        //         var splittedStrings = message.Split(' ');
        //         if (splittedStrings.Length != 3) return;
        //         var x = float.Parse(splittedStrings[0]);
        //         var y = float.Parse(splittedStrings[1]);
        //         var z = float.Parse(splittedStrings[2]);
        //         transform.position = new Vector3(x, y, z);
        if (msg.Equals("s01"))
        {
            Buffer.BlockCopy(b, 0, cmd, 0, 4);
        }
        else if(msg.Equals("m64"))
        {
            Buffer.BlockCopy(b, 0, fm, 0, 256);
            // call mgr to set matrix
            cmv.davidMeshMgr.setMatrix(fm);

        }
        else if (msg.Equals("m04"))
        {
            Buffer.BlockCopy(b, 0, fm, 0, 4 * 4);

        }
        //fm = f;
    }

    private void Start()
    {
        _netMqListener = new NetMqListener(HandleMessage, HandleFMessage, topic);
        _netMqListener.Start();

        cmd = new float[1];
        fm = new float[64];
    }

    private void Update()
    {
        _netMqListener.Update();
    }

    private void OnDestroy()
    {
        _netMqListener.Stop();
    }
}
