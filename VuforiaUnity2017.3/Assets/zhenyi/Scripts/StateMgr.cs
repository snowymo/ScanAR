using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour {

    public int method = 0;// 0 means observer method and 1 means opencv method

    public enum STATE { aligning, manipulation };
    
    [SerializeField]
    public STATE current_state;

	// Use this for initialization
	void Start () {
        current_state = STATE.aligning;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ControllerClicked(Transform tf)
    {
        switch (current_state)
        {
            case STATE.aligning:
                if(method == 0)
                {
                    GetComponent<AlignmentMgr>().AssignAlignmentResult(tf);
                    current_state++;
                }
                else
                {
                    GetComponent<CalibrateMgr>().addVTPoint(tf.localPosition);
                    GetComponent<CustomMsgVive>().sendCalib();
                }
                //                 
                //                 GetComponent<TrackingCtrl>().AssignVTWhenAligning(GetComponent<CustomMsgVive>().vivetracker.transform);
                //current_state++;
                
                break;
            case STATE.manipulation:
                break;
            default:
                break;
        }
    }

    public void ControllerGripped()
    {
       GetComponent<CalibrateMgr>().reset();
        current_state = STATE.aligning;
    }
}
