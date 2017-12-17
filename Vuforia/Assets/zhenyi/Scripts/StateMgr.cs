using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour {

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
                GetComponent<AlignmentMgr>().AssignAlignmentResult(tf);
                current_state++;
                break;
            case STATE.manipulation:
                break;
            default:
                break;
        }
    }
}
