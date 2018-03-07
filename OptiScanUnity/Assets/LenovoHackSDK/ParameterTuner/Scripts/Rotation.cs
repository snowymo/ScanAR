using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
    public float speed = 100;
    public GameObject canvas;
    public bool isActive = true;
    public Transform trans;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        trans.Rotate(0, speed * Time.deltaTime, 0);
        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 newPos = trans.position;
            newPos.z += 0.2f;
            trans.position = newPos;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 newPos = trans.position;
            newPos.z -= 0.2f;
            trans.position = newPos;

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            isActive = !isActive;
            canvas.SetActive(isActive);
        }

	}
}
