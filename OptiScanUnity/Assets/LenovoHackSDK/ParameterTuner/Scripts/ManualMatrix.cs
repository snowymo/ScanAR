using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMatrix : MonoBehaviour {

    public Vector3 manual_pos;
    public Vector3 manual_euler_angle;
    //public Transform tracker_eye;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//         Quaternion rot = Quaternion.Euler(manual_euler_angle);
//         Matrix4x4 mtx = Matrix4x4.TRS(manual_pos, rot, Vector3.one);
//         Matrix4x4 mtx_trackeye = Matrix4x4.TRS(tracker_eye.position, tracker_eye.rotation, Vector3.one);
//         Matrix4x4 cur = mtx * mtx_trackeye;
//         transform.position = cur.GetColumn(3);
//         transform.rotation = cur.rotation;
            
    }
}
