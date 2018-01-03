using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggleVive : MonoBehaviour
{
    public GameObject arcmr;
    public GameObject vivecmr;
    // Use this for initialization
    void Awake()
    {
        print("UnityEngine.XR.XRSettings.loadedDeviceName:" + UnityEngine.XR.XRSettings.loadedDeviceName);
        if (GetComponent<CustomMsgVive>().category == CustomMsgVive.Category.Scanner)
        {
            arcmr.SetActive(false);
            vivecmr.SetActive(true);
        }
        else if (GetComponent<CustomMsgVive>().category == CustomMsgVive.Category.Hololens)
        {
            arcmr.SetActive(true);
            vivecmr.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
