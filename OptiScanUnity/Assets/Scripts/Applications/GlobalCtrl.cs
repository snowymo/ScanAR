using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCtrl : MonoBehaviour {

    public bool isHololens;

    public GameObject[] hololensRelated;

    public GameObject[] unityRelated;

	// Use this for initialization
	void Start () {

            foreach(GameObject go in hololensRelated)
            {
                go.SetActive(isHololens);
            }
            foreach(GameObject go in unityRelated)
            {
                go.SetActive(!isHololens);
            }

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
