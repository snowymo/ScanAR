using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentMgr : MonoBehaviour {

    Vector3 alignedPos;
    Quaternion alignedRot;

    float alignedEulerY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AssignAlignmentResult(Transform tf)
    {
        alignedPos = tf.position;
        alignedEulerY = tf.eulerAngles.y/* - 180*/;
        alignedRot = tf.rotation;
        print("pos:" + tf.position + " euler:" + alignedEulerY);
    }

    public void CalculateTransform(Transform vivematrix, ref Transform holomatrix)
    {
        float alignedY = alignedEulerY/* - 180*/;
        holomatrix.localPosition = Quaternion.Euler(0, -alignedY, 0) * vivematrix.position + Quaternion.Euler(0, -alignedY, 0) * -alignedPos;
        holomatrix.localRotation = Quaternion.Euler(0, -alignedY, 0) * vivematrix.rotation;

        //print("vive: " + vivematrix.position + " rot:" + vivematrix.rotation.eulerAngles.y);
        //print("holomatrix: " + holomatrix.position + " rot:" + holomatrix.rotation.eulerAngles.y);
    }

    public void CalculateTransform2(Transform vivematrix, ref Transform holomatrix)
    {
        holomatrix.localPosition = Quaternion.Euler(0, -alignedEulerY, 0) * vivematrix.position + Quaternion.Euler(0, -alignedEulerY, 0) * -alignedPos;
        holomatrix.localRotation = Quaternion.Euler(0, -alignedEulerY, 0) * vivematrix.rotation;


        //print("vive 2: " + vivematrix.position + " rot:" + vivematrix.rotation.eulerAngles.y);
        //print("holomatrix 2: " + holomatrix.position + " rot:" + holomatrix.rotation.eulerAngles.y);
    }

//     public void CalculateTransform2(Transform vivematrix, ref Transform holomatrix)
//     {
//         holomatrix.position = Quaternion.Euler(0, alignedEulerY, 0) * vivematrix.position + alignedPos;
//         holomatrix.rotation = Quaternion.Euler(0, alignedEulerY, 0) * vivematrix.rotation;
//         print("2 vive: " + vivematrix.position + " rot:" + vivematrix.rotation.eulerAngles.y);
//         print("2 holomatrix: " + holomatrix.position + " rot:" + holomatrix.rotation.eulerAngles.y);
}
