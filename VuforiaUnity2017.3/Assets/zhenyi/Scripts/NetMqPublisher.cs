﻿using System.Diagnostics;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class NetMqPublisher
{
    private readonly Thread _listenerWorker;

    private bool _listenerCancelled;

    public delegate string MessageDelegate(byte[] message);

    public delegate string MessageStrDelegate(string message);

    private readonly MessageDelegate _messageDelegate;

    private readonly MessageStrDelegate _messageStrDelegate;

    private readonly Stopwatch _contactWatch;

    private const long ContactThreshold = 1000;

    public bool Connected;

    private void ListenerWork()
    {
        AsyncIO.ForceDotNet.Force();
        using (var server = new ResponseSocket())
        {
            server.Bind("tcp://*:5555");

            while (!_listenerCancelled)
            {
                Connected = _contactWatch.ElapsedMilliseconds < ContactThreshold;

//                 byte[] message;
//                 if (!server.TryReceiveFrameBytes(out message)) continue;

                string msgStr;
                if (!server.TryReceiveFrameString(out msgStr)) continue;

                //_contactWatch.Restart();
                _contactWatch.Stop();
                _contactWatch.Start();

                //var response = _messageDelegate(message);
                var response = _messageStrDelegate(msgStr);

                server.SendFrame(response);
                
            }
        }
        NetMQConfig.Cleanup();
    }

    public NetMqPublisher(MessageDelegate messageDelegate)
    {
        _messageDelegate = messageDelegate;
        _contactWatch = new Stopwatch();
        _contactWatch.Start();
        _listenerWorker = new Thread(ListenerWork);
    }

    public NetMqPublisher(MessageStrDelegate messageDelegate)
    {
        _messageStrDelegate = messageDelegate;
        _contactWatch = new Stopwatch();
        _contactWatch.Start();
        _listenerWorker = new Thread(ListenerWork);
    }


    public void Start()
    {
        _listenerCancelled = false;
        _listenerWorker.Start();
    }

    public void Stop()
    {
        _listenerCancelled = true;
        _listenerWorker.Join();
    }
}