using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingCtrl : MonoBehaviour {

   
    Vector3 holoEyePos, vtPos;
    Quaternion holoEyeRot, vtRot;

    float holoEyeEulerY, vtEulerY;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void fusion(Transform vt, ref Transform eye)
    {
        // once the alignment finished, I need to remember the p,q for vive controller at 0,0,0 of hololens system
        // and then I know where vive tracker should be then, I can get hololens camera's pos by calling main cmr's position
        float alignedY = holoEyeEulerY/* - 180*/;
        eye.localPosition = Quaternion.Euler(0, -alignedY, 0) * vt.position + Quaternion.Euler(0, -alignedY, 0) * -holoEyePos;
        eye.localRotation = Quaternion.Euler(0, -alignedY, 0) * vt.rotation;
        print("[hehe] fusion: ");
        print("vt: " + vt.position + " rot:" + vt.rotation.eulerAngles.y);
        print("\teye: " + eye.position + " rot:" + eye.rotation.eulerAngles.y);
    }

    public void AssignEyeWhenAligning(Vector3 eyePos, Quaternion eyeRot)
    {
        print("[hehe] into AssignEyeWhenAligning\n");
        holoEyePos = vtPos - eyePos;
        holoEyeEulerY = vtEulerY - eyeRot.eulerAngles.y;
        holoEyeRot = vtRot;
        print("[hehe] pos: vt.position:" + vtPos + " eye.position: " + eyePos + "euler of vt:" + vtEulerY + " eye.eulerAngles.y:" + eyeRot.eulerAngles.y + "\n");
    }

    public void AssignVTWhenAligning(Transform vt)
    {
        print("[hehe] into AssignVTWhenAligning\n");
        vtPos = vt.position;
        vtEulerY = vt.eulerAngles.y;
        vtRot = vt.rotation;
        print("[hehe] pos: vt.position:" + vt.position + "euler of vt:" + vt.eulerAngles.y + "\n");
    }
}
