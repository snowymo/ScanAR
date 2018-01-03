using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMappingCtrl : MonoBehaviour {

    // Use this for initialization
    void Start() {
        //disableSpatialMapping();
    }

    // Update is called once per frame
    void Update() {

    }

    public void disableSpatialMapping()
    {
        //HoloToolkit.Unity.SpatialMapping.SpatialMappingManager;
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.StopObserver();
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.CastShadows = false;
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.DrawVisualMeshes = false;
    }

    public void enableSpatialMapping()
    {
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.StartObserver();
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.CastShadows = true;
        Academy.HoloToolkit.Unity.SpatialMappingManager.Instance.DrawVisualMeshes = true;
    }

    public void disablePositionTracking()
    {
        //XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
    }
}
