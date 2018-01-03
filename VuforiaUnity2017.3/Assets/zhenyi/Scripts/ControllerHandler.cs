using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHandler : MonoBehaviour {

    public StateMgr stateManager;

    private SteamVR_TrackedController controller;

    void Start()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        if (!controller)
        {
            Debug.LogError("Controller must have SteamVR_TrackedController behavior.");
            return;
        }

        controller.TriggerClicked += Controller_TriggerClicked;
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        //if (stateManager.CurrentlyAligning)
        //{
            stateManager.ControllerClicked(controller.transform);
        //}
    }
}
