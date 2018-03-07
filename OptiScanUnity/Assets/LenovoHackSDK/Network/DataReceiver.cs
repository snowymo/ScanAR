using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System;

public class DataReceiver : MonoBehaviour {

    public int PORT = 20000;
    public string IP = "128.122.215.23";

    int MAX_BUF_SIZE = 65536; // 2^16
    bool active;

    float updateTime;

    UdpClient client;
    Thread pthread;

    IPEndPoint ep;
    byte[] buf;

    struct Tracker
    {
        public string id;
        public Vector3 pos;
        public Quaternion rot;
    }
    Dictionary<string, Tracker> trackers;

	// Use this for initialization
	void Start () {
        trackers = new Dictionary<string, Tracker>();
        active = true;
        ep = new IPEndPoint(IPAddress.Parse(IP), PORT);
        buf = new byte[MAX_BUF_SIZE];

        pthread = new Thread(DataListen);
        pthread.Start();
	}
	
	// Update is called once per frame
	void Update () {
        updateTime = Time.time;
	}

    public Vector3 getPosition(string id)
    {
        if (trackers.ContainsKey(id))
        {
            return trackers[id].pos;
        }
        return Vector3.zero;
    }

    public Quaternion getRotation(string id)
    {
        if (trackers.ContainsKey(id))
        {
            return trackers[id].rot;
        }
        return Quaternion.identity;
    }

    string getStringRemainder(string message, string key)
    {
        if (message.IndexOf(key) == -1)
        {
            return "";
        }
        return message.Substring(message.IndexOf(key) + key.Length);
    }
    void ParseMessage(string message)
    {
        // super hacky, but Unity's JSON loading sucks...
        string sub = getStringRemainder(message, "\"id\": \"");
        while (sub != "")
        {
            string _id = sub.Substring(0, sub.IndexOf("\""));
            if (_id == "")
            {
                return;
            }

            sub = getStringRemainder(sub, "\"x\": ");
            float x = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"y\": ");
            float y = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"z\": ");
            float z = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"qw\": ");
            float qw = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"qx\": ");
            float qx = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"qy\": ");
            float qy = float.Parse(sub.Substring(0, sub.IndexOf(",")));

            sub = getStringRemainder(sub, "\"qz\": ");
            float qz = float.Parse(sub.Substring(0, sub.IndexOf("}")));




            Tracker newTracker = new Tracker
            {
                id = _id,
                pos = new Vector3(-x, y, z),
                rot = new Quaternion(-qx, qy, qz, -qw)
            };
            trackers[_id] = newTracker;

            sub = getStringRemainder(sub, "\"id\": \"");
        }
    }

    private void OnApplicationQuit()
    {
        active = false;
    }

    void DataListen()
    {
        client = new UdpClient(PORT);
        Debug.Log("opening connection...");

        client.Connect(ep);
        

        float last_time = 0;
        byte[] buf = new byte[MAX_BUF_SIZE];
        buf = System.Text.Encoding.UTF8.GetBytes("{\"mode\":\"receiver\",\"type\":\"3d\"}");
        client.Send(buf, buf.Length * sizeof(byte));
      
        client.BeginReceive(new AsyncCallback(ReceiveCallback), null);

        while (active)
        {
            Thread.Sleep(100);
        }
        Debug.Log("closing connection...");

        buf = System.Text.Encoding.UTF8.GetBytes("{\"mode\":\"disconnect\"}");
        client.Send(buf, buf.Length * sizeof(byte));

        client.Close();
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        if (!active)
        {
            return;
        }

        //Debug.Log("Waiting for data...");
        buf = client.EndReceive(ar, ref ep);

        string str_data = System.Text.Encoding.UTF8.GetString(buf);
        //Debug.Log("Received message: " + str_data);
        ParseMessage(str_data);

        client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }
}
