using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHandler : MonoBehaviour {

    public Vector3 tunerTranslation, tunerEulerAngle;
    public float tunerScale;

    public GameObject[] gos;

    // Use this for initialization
    void Start () {
        tunerScale = 1.0f;
        tunerTranslation = new Vector3();
        tunerEulerAngle = new Vector3();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
        {
            tunerTranslation.y += 0.1f;
        }
        else if (Input.GetKeyDown("s"))
        {
            tunerTranslation.y -= 0.1f;
        }
        else if (Input.GetKeyDown("a"))
        {
            tunerTranslation.x -= 0.1f;
        }
        else if (Input.GetKeyDown("d"))
        {
            tunerTranslation.x += 0.1f;
        }
        else if (Input.GetKeyDown("q"))
        {
            tunerTranslation.z += 0.1f;
        }
        else if (Input.GetKeyDown("e"))
        {
            tunerTranslation.z -= 0.1f;
        }
        else if (Input.GetKeyDown("i"))
        {
            tunerEulerAngle.x -= 0.1f;
        }
        else if (Input.GetKeyDown("k"))
        {
            tunerEulerAngle.x += 0.1f;
        }
        else if (Input.GetKeyDown("j"))
        {
            tunerEulerAngle.y -= 0.1f;
        }
        else if (Input.GetKeyDown("k"))
        {
            tunerEulerAngle.y += 0.1f;
        }
        else if (Input.GetKeyDown("u"))
        {
            tunerEulerAngle.z -= 0.1f;
        }
        else if (Input.GetKeyDown("o"))
        {
            tunerEulerAngle.z += 0.1f;
        }
        else if (Input.GetKeyDown("="))
        {
            tunerScale += 0.1f;
        }
        else if (Input.GetKeyDown("-"))
        {
            tunerScale -= 0.1f;
        }

        foreach (GameObject go in gos)
        {
            go.transform.localPosition = tunerTranslation;
            go.transform.localEulerAngles = tunerEulerAngle;
            go.transform.localScale = new Vector3(tunerScale, tunerScale, tunerScale);
        }
    }
}
