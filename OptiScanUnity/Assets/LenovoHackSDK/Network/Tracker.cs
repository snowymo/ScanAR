using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {
    public string id;
    public DataReceiver network;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = network.getPosition(id);
        transform.rotation = network.getRotation(id);
	}
}
