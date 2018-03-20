using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibrate104 : MonoBehaviour {

    public Transform headset;
    public Transform v_obj;
    public Transform p_obj;

    public Matrix4x4 Mheadset, Moffset, Mvobj, Mpobj;
    public Matrix4x4 curOffset;

    public Text visualOffset;
    // Use this for initialization
    void Start () {
        Moffset = Matrix4x4.identity;
	}
	
	// Update is called once per frame
	void Update () {
        Mheadset = Matrix4x4.TRS(headset.position, headset.rotation, Vector3.one);
        Mvobj = Matrix4x4.TRS(v_obj.localPosition, v_obj.localRotation, Vector3.one);
        Mpobj = Matrix4x4.TRS(p_obj.position, p_obj.rotation, Vector3.one);

        Moffset = Mheadset.inverse * Mpobj * Mvobj.inverse;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            curOffset = Moffset;
            
        }
        visualOffset.text = "rot:" + Moffset.rotation.ToString("F4") + "\npos:" + Moffset.GetColumn(3).ToString("F4");
    }
}
