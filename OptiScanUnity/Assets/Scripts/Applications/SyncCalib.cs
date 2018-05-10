using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCalib : Holojam.Tools.SynchronizableTrackable
{

    public TextMesh tm;
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

    public CalibCollection calibCtrl;

    public bool isReady;

    public CalibrateRay cr;

    public override void ResetData()
    {
        data = new Holojam.Network.Flake(calibCtrl.calibLineAmount_x * calibCtrl.calibLineAmount_y * calibCtrl.calibPointPerLine + calibCtrl.calibLineAmount_x * calibCtrl.calibLineAmount_y, 0, 0, 1);
        host = true;
        host = (Application.platform == RuntimePlatform.Android);
        //tm.text = Application.platform.ToString();
        //tm.text += "\thello\n" + host;
        isReady = false;
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
            if(data.ints[0] == 1)
            {
                cr.CalibrateRays(data.vector3s, calibCtrl.calibLineAmount, calibCtrl.calibPointPerLine);
                data.ints[0] = 0;
            }
                
        }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }

    public void SetCollectedData(List<Vector3> objPoints, List<Vector3> overlayPoints)
    {
        print("set collected data for sync");
        int index = 0;
        for (int i = 0; i < objPoints.Count; i++, index++)
        {
            data.vector3s[index] = objPoints[i];
        }
        for (int i = 0; i < overlayPoints.Count; i++, index++)
        {
            data.vector3s[index] = overlayPoints[i];
        }
        isReady = true;
        data.ints[0] = 1;
    }
}
