using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibratedTrackable : Holojam.Tools.SynchronizableTrackable
{
    [SerializeField] string label = "CalibedObj";
    [SerializeField] string scope = "";

    [SerializeField] bool host = true;
    [SerializeField] bool autoHost = false;

    public void SetLabel(string label) { this.label = label; }
    public void SetScope(string scope) { this.scope = scope; }

    public void SetHost(bool host) { this.host = host; }
    public void SetAutoHost(bool autoHost) { this.autoHost = autoHost; }

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    public override bool Host { get { return host; } }
    public override bool AutoHost { get { return autoHost; } }

    public CalibrateRay cr;

    public Transform oriObj;
    public Transform headsetRB;

    Vector3 tOffset;
    Quaternion rOffset;

    public override void ResetData()
    {
        data = new Holojam.Network.Flake(1);
        host = true;
        host = (Application.platform != RuntimePlatform.Android);
    }

    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        //base.Sync();

        if (Sending)
        {
            // calculate the pos and rot to sync
            cr.getResult(ref tOffset, ref rOffset);
            Vector3 headsetRelative = headsetRB.InverseTransformPoint(oriObj.position);
            Vector3 pShift = headsetRelative - tOffset;
            Vector3 newPos = rOffset * pShift;
            data.vector3s[0] = newPos;
        }
        else
        {
            // receive all the vectors as a windows calculator
            transform.localPosition = data.vector3s[0];
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }
}
