using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class zmqServer : MonoBehaviour {

    public bool Connected;
    private NetMqPublisher _netMqPublisher;
    [SerializeField]
    private string _response;

    public float[] matrix1, matrix2;

    private void Start()
    {
        _netMqPublisher = new NetMqPublisher(HandleMessage);
        _netMqPublisher.Start();

        matrix1 = new float[16 * 4];
        matrix2 = new float[16 * 4];
    }

    private void Update()
    {
        //var position = transform.position;
        //_response = "position:" + position.x + "," + position.y + "," + position.z + "\n";//$"{position.x} {position.y} {position.z}";
        Connected = _netMqPublisher.Connected;
    }

    private string HandleMessage(byte[] message)
    {
        // Not on main thread
        // handle received msg
        print("handle message");
        print(message);
        // buffer copy
        Buffer.BlockCopy(message, 0, matrix1, 0, 16 * 4 * 4);
        // bitconverter
        for(int i = 0; i < matrix2.Length; i++)
            matrix2[i] = System.BitConverter.ToSingle(message, i * 4);
        return _response;
    }

    private string HandleMessage2(string message)
    {
        print("handle message");
        print(message);
        _response = message;
        return "ok";
    }

    private void OnDestroy()
    {
        _netMqPublisher.Stop();
    }
}
