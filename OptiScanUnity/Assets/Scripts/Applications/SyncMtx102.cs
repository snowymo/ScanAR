using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class SyncMtx102 : Holojam.Tools.SynchronizableTrackable
{

    // As an example, expose all the Synchronizable properties in the inspector.
    // In practice, you probably want to control some or all of these manually in code.

    [SerializeField] string label = "SyncMtx102";
    [SerializeField] string scope = "";

    [SerializeField] bool host = false;
    [SerializeField] bool autoHost = false;

    // As an example, allow all the Synchronizable properties to be publicly settable
    // In practice, you probably want to control some or all of these manually in code.

    public void SetLabel(string label) { this.label = label; }
    public void SetScope(string scope) { this.scope = scope; }

    public void SetHost(bool host) { this.host = host; }
    public void SetAutoHost(bool autoHost) { this.autoHost = autoHost; }

    // Point the property overrides to the public inspector fields

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    public override bool Host { get { return host; } }
    public override bool AutoHost { get { return autoHost; } }

    [DllImport("svd", EntryPoint = "Calib")]
    private static extern void Calib(float[] a, float[] b, int len, float[] transform);

    public CameraMatrix cm;

    // Add the scale vector to Trackable, which by default only contains position/rotation
    public override void ResetData()
    {
        data = new Holojam.Network.Flake(8);
        //data.floats[0] = data.floats[5] = data.floats[10] = data.floats[15] = 1;
    }

    public bool isWrote = false;
    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        if (Sending)
        {
            data.vector3s = GetComponent<Calibrate102>().GetPointPairs();
        }
        else
        {
            //GetComponent<Calibrate102>().SetMatrix(data.floats);
            // write down
            if (Tracked && Application.platform == RuntimePlatform.WindowsEditor && !isWrote)
            {
                isWrote = true;
                string path = "Assets/Resources/pointsets.txt";
                StreamWriter writer = new StreamWriter(path, true);
                for(int i = 0; i < data.vector3s.Length; i++)
                    writer.WriteLine(data.vector3s[i].ToString("F4"));
                writer.Close();
                print("isWrote");
                print(data.vector3s);

                // call c++ dll to calculate svd
                float[] pa = new float[4 * 3];
                for(int i = 0; i < pa.Length / 3; i++)
                {
                    pa[i * 3] = data.vector3s[i][0];
                    pa[i * 3 + 1] = data.vector3s[i][1];
                    pa[i * 3 + 2] = data.vector3s[i][2];
                }

                float[] pb = new float[4 * 3];
                for (int i = 0; i < pb.Length / 3; i++)
                {
                    pb[i * 3] = data.vector3s[i+4][0];
                    pb[i * 3 + 1] = data.vector3s[i+4][1];
                    pb[i * 3 + 2] = data.vector3s[i+4][2];
                }
                float[] result = new float[16];
                for (int i = 0; i < result.Length; i++)
                    result[i] = 1.0f;
                Calib(pa,pb,pa.Length, result);
                for (int i = 0; i < result.Length; i++)
                    print(result[i]);
                // assign to CameraMatrix.mtxEye
                cm.SetMtxEYE(result);
            }
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }
}
