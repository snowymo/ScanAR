using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHandler : MonoBehaviour {

    public Vector3 tunerTranslation, tunerEulerAngle;
    public float tunerScale;

    public GameObject[] gos;

    // Use this for initialization
    void Start () {
        tunerScale = 0.6265f;
        tunerTranslation = new Vector3(0.137f, -0.04f, -0.05f);
        tunerEulerAngle = new Vector3(180f,0f,148.45f);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
        {
            tunerTranslation.y += 0.001f;
        }
        else if (Input.GetKeyDown("s"))
        {
            tunerTranslation.y -= 0.001f;
        }
        else if (Input.GetKeyDown("a"))
        {
            tunerTranslation.x -= 0.001f;
        }
        else if (Input.GetKeyDown("d"))
        {
            tunerTranslation.x += 0.001f;
        }
        else if (Input.GetKeyDown("q"))
        {
            tunerTranslation.z += 0.001f;
        }
        else if (Input.GetKeyDown("e"))
        {
            tunerTranslation.z -= 0.001f;
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
            tunerScale += 0.01f;
        }
        else if (Input.GetKeyDown("-"))
        {
            tunerScale -= 0.01f;
        }

        foreach (GameObject go in gos)
        {
            go.transform.localPosition = tunerTranslation;
            go.transform.localEulerAngles = tunerEulerAngle;
            go.transform.localScale = new Vector3(tunerScale, tunerScale, tunerScale);
        }
    }
}
