using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMatrix : Holojam.Tools.SynchronizableTrackable
{

    public Transform TrackerEye;
    public Matrix4x4 mtxEye;
    Matrix4x4 mtxTrackerEye;

    [SerializeField] string label = "CameraMatrix102";
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

    // Add the scale vector to Trackable, which by default only contains position/rotation
    public override void ResetData()
    {
        initial();
        data = new Holojam.Network.Flake(1,1);
       
    }

    bool isWrote = false;
    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        

        // m = ET
        mtxTrackerEye = Matrix4x4.TRS(TrackerEye.position, TrackerEye.rotation, Vector3.one);
        Matrix4x4 curM = mtxEye * mtxTrackerEye;
        transform.position = curM.GetColumn(3);
        transform.rotation = curM.rotation;

        if (Sending)
        {
            base.Sync();
        }
        else
        {
            if(Tracked)
                base.Sync();
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag

        
        base.Update();
    }

    // Use this for initialization
    void initial() {
        mtxEye = Matrix4x4.identity;
        mtxTrackerEye = Matrix4x4.identity;

        mtxEye[0, 0] = -0.29322964f;
        mtxEye[1, 0] = 0.84660488f;
        mtxEye[2, 0] = -0.44415826f;

        mtxEye[0, 1] = -0.46558685f;
        mtxEye[1, 1] = 0.27931203f;
        mtxEye[2, 1] = 0.83977002f;

        mtxEye[0, 2] = 0.83501213f;
        mtxEye[1, 2] = 0.45303971f;
        mtxEye[2, 2] = 0.31226553f;

        mtxEye[0, 3] = 1.40892117f;
        mtxEye[1, 3] = -0.53082655f;
        mtxEye[2, 3] = -0.45353763f;
    }

    public void SetMtxEYE(float[] f)
    {
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                mtxEye[i, j] = f[i + j * 4];
            }
        }
    }
}
