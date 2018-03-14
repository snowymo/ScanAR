using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Network;
using Holojam.Tools;

public class SensorFusion : Trackable
{
    [SerializeField] public Vector3 offset;
    [SerializeField] public Vector3 offsetRot;

    const float correctionThreshold = 0.98f;
    //Lower values allow greater deviation without correction
    Quaternion correction = Quaternion.identity;

    const float differenceThreshold = 0.9995f;
    //Lower values allow correction at greater angular speeds
    public float difference = 1;
    public float correctness = 1;
    public float correctY;

    const float timestep = 0.01f;
    float lastTime = 0;
    private Quaternion lastRot = Quaternion.identity;
    public string label = "Viewer";
    public string scope = "Vive";

    private const int MAX_LAG = 100;

    const int IMU_BUFFER_SIZE = 600;
    public Quaternion[] smoothedRotation = new Quaternion[2] { Quaternion.identity, Quaternion.identity };
    public Quaternion[] correctRotation = new Quaternion[2] { Quaternion.identity, Quaternion.identity };
    private float delayTime;
    private Dictionary<float, Quaternion> imuBuffer = new Dictionary<float, Quaternion>();
    private List<float> imuBufferIdx = new List<float>();

    public bool isYawOnlyOn = false;

    public Transform imuObj;
    public bool imuOnly, rotationOnly;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Update()
    {

        base.Update();
    }

    //Update late to catch local space updates
    protected override void UpdateTracking()
    {
        // old version only need IMU
        //		IMUUpdateTracking();
        //updateIMU();
        // new version support sensor fusion
        //ViveTrackerUpdateTracking();
        SensorReceive();
        calculateDiff();
        if (Tracked)
            correctRotation[0] = transform.rotation;
    }

    readonly Holojam.Utility.AccurateSmoother smoother = new Holojam.Utility.AccurateSmoother(1, 2);

    private void SensorReceive()
    {
        // vive tracker's data
        Vector3 sourcePosition = new Vector3(RawPosition.x, RawPosition.y, RawPosition.z);
        Quaternion sourceRotation = new Quaternion(RawRotation.x, RawRotation.y, RawRotation.z, RawRotation.w);
        Quaternion offsetRotationQ = Quaternion.Euler(offsetRot.x, offsetRot.y, offsetRot.z);
        sourceRotation = sourceRotation * offsetRotationQ;
        sourcePosition += sourceRotation * offset;

        bool sourceTracked = Tracked;

        Quaternion imu = imuObj.rotation;

        if (sourceTracked && imu != Quaternion.identity && !imuOnly)
        {
            Quaternion inv = Quaternion.Inverse(imu);
            Quaternion optical = sourceRotation * inv;
            //Quaternion localRotation = transform.rotation;

            Quaternion oldOrientation = this.transform.rotation;

            float yOpt = optical.eulerAngles.y;
            float yOld = oldOrientation.eulerAngles.y;
            float yDiff = Mathf.Abs(yOpt - yOld);
            if (yDiff > 180f)
            {
                if (yOpt < yOld)
                {
                    yOpt += 360f;
                }
                else
                {
                    yOld += 360f;
                }
                yDiff = Mathf.Abs(yOpt - yOld);
            }
            float t = yDiff / 180f;
            t = t * t;
            float yNew = Mathf.LerpAngle(yOld, yOpt, t);
            // zhenyi
            this.transform.rotation = Quaternion.AngleAxis(yNew, Vector3.up);
        }
        else
        {
            transform.rotation = correctRotation[0]; //Transition seamlessly to IMU when untracked
                                                     //print ("not tracked," + transform.rotation);
        }
        if (!rotationOnly)
            transform.position = sourcePosition;
        //imu only version
        if (imuOnly)
            this.transform.rotation = Quaternion.identity;
    }

    private void calculateDiff()
    {
        //Calculate rotation difference since last timestep
        if (Time.time > lastTime + timestep)
        {
            difference = Quaternion.Dot(correctRotation[0], correctRotation[1]);
            correctness = Quaternion.Dot(RawRotation, correctRotation[1]);
        }
    }

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }
}
