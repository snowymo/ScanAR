using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHandler : MonoBehaviour {


    public StateMgr stateManager;

    private SteamVR_TrackedController controller;
    public SteamVR_TrackedObject vivetracker;

    void Start()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        if (!controller)
        {
            Debug.LogError("Controller must have SteamVR_TrackedController behavior.");
            return;
        }

        controller.TriggerClicked += Controller_TriggerClicked;
        controller.Gripped += Controller_Gripped;
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        //if (stateManager.CurrentlyAligning)
        //{
        if (stateManager.method == 0)
        {
            stateManager.ControllerClicked(controller.transform);
        }
        else
        {
            stateManager.ControllerClicked(vivetracker.transform);
        }
            
        //}
    }

    private void Controller_Gripped(object sender, ClickedEventArgs e)
    {
        // reset
        stateManager.ControllerGripped();
    }
}
