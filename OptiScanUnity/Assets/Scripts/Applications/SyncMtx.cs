using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMtx : Holojam.Tools.SynchronizableTrackable
{

    // As an example, expose all the Synchronizable properties in the inspector.
    // In practice, you probably want to control some or all of these manually in code.

    [SerializeField] string label = "CalibMatrix";
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
        data = new Holojam.Network.Flake(0,0,16);
        data.floats[0] = data.floats[5] = data.floats[10] = data.floats[15] = 1;
    }

    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        if (Sending)
        {
            data.floats = GetComponent<Calibrate>().GetMatrix();
        }
        else
        {
            GetComponent<Calibrate>().SetMatrix(data.floats);
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }
}
