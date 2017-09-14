using UnityEngine;

public class InputMgr : MonoBehaviour
{
    private SteamVR_TrackedController _controller;
    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

    public TransformCal tf;

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += HandleTriggerClicked;
        //_controller.PadClicked += HandlePadClicked;
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        //_controller.PadClicked -= HandlePadClicked;
    }

    #region Primitive Spawning
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        tf.bCalibrated = true;
        //SpawnCurrentPrimitiveAtController();
    }

    private void SpawnCurrentPrimitiveAtController()
    {
        var spawnedPrimitive = GameObject.CreatePrimitive(_currentPrimitiveType);
        spawnedPrimitive.transform.position = transform.position;
        spawnedPrimitive.transform.rotation = transform.rotation;

        spawnedPrimitive.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (_currentPrimitiveType == PrimitiveType.Plane)
            spawnedPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    #endregion
    
}