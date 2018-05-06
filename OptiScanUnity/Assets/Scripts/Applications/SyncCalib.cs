using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCalib : Holojam.Tools.SynchronizableTrackable
{
    [SerializeField] string label = "SyncCalib";
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

    public OverlapCalib calibCtrl;

    public override void ResetData()
    {
        data = new Holojam.Network.Flake(2 * calibCtrl.calibLineAmount * calibCtrl.calibPointPerLine + calibCtrl.calibLineAmount);
        host = true;
        host = (Application.platform == RuntimePlatform.Android);
        
    }

    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        //base.Sync();

        if (Sending)
        {
            // sync all the vector3s as an android phone
        }
        else
        {
            // receive all the vectors as a windows calculater
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }

    public void SetCollectedData(List<Vector3> objPoints, List<Vector3> headsetPoints, List<Vector3> overlayPoints)
    {
        print("set collected data for sync");
        int index = 0;
        for(int i = 0; i < objPoints.Count; i++, index++)
        {
            data.vector3s[index] = objPoints[i];
        }
        for (int i = 0; i < headsetPoints.Count; i++, index++)
        {
            data.vector3s[index] = headsetPoints[i];
        }
        for (int i = 0; i < overlayPoints.Count; i++, index++)
        {
            data.vector3s[index] = overlayPoints[i];
        }
    }
}
