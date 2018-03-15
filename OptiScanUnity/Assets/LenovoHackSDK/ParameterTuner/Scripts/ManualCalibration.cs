using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ManualCalibration : MonoBehaviour {

    public CalibrationParam paramPrefab;

    //public PhoneSpaceControllerBase phoneSpaceController;
    public ManualMatrix unknownMatrix;

    public Text deltaLabel;

    private List<CalibrationParam> calibrationParams = new List<CalibrationParam>();

    private List<int> deltaPows = new List<int>();

    private const int deltaBase = 10;

    void Start()
    {
        setupScene();
        CreateParam("Pos: x",
            () => unknownMatrix.manual_pos.x,
            (value) =>
            {
                unknownMatrix.manual_pos.x = value;
            },
            deltaPow: -3

        );
        CreateParam("Pos: Y",
            () => unknownMatrix.manual_pos.y,
            (value) =>
            {
                unknownMatrix.manual_pos.y = value;
            },
            deltaPow: -3

        );
        CreateParam("Pos: Z",
            () => unknownMatrix.manual_pos.z,
            (value) =>
            {
                unknownMatrix.manual_pos.z  = value;
            },
            deltaPow: -3

        );
        CreateParam("Rot: X",
            () => unknownMatrix.manual_euler_angle.x,
            (value) =>
            {
                unknownMatrix.manual_euler_angle.x = value;
            },
            deltaPow: -1

        );

        CreateParam("Rot: Y",
            () => unknownMatrix.manual_euler_angle.y,
            (value) =>
            {
                unknownMatrix.manual_euler_angle.y = value;
            },
            deltaPow: -1
        );

        CreateParam("Rot: Z",
            () => unknownMatrix.manual_euler_angle.z,
            (value) =>
            {
                unknownMatrix.manual_euler_angle.z = value;
            },
            deltaPow: -1
        );

        SelectedIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) ||
            RemoteKeyboardReceiver.Instance.GetKeyDown('s'))
        {
            SelectedIndex = (SelectedIndex + 1) % calibrationParams.Count;
        }
        else if (Input.GetKeyDown(KeyCode.W) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('w'))
        {
            int selected = (SelectedIndex - 1) % calibrationParams.Count;
            if (selected < 0)
            {
                selected += calibrationParams.Count;
            }
            SelectedIndex = selected;
        }
        else if (Input.GetKeyDown(KeyCode.Q) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('q'))
        {
            deltaPows[SelectedIndex]++;
        }
        else if (Input.GetKeyDown(KeyCode.E) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('e'))
        {
            deltaPows[SelectedIndex]--;
        }
        else if (Input.GetKey(KeyCode.D) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('d'))
        {
            CalibrationParam param = calibrationParams[SelectedIndex];
            float value = param.ValueOnUpdate();
            value += Mathf.Pow(deltaBase, deltaPows[SelectedIndex]);
            param.SetValue(value);
        }
        else if (Input.GetKey(KeyCode.A) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('a'))
        {
            CalibrationParam param = calibrationParams[SelectedIndex];
            float value = param.ValueOnUpdate();
            value -= Mathf.Pow(deltaBase, deltaPows[SelectedIndex]);
            param.SetValue(value);
        }

        deltaLabel.text = string.Format("Delta = {0}", Mathf.Pow(deltaBase, deltaPows[SelectedIndex]));
    }

    private void CreateParam(string name, Func<float> valueOnUpdate, Action<float> setValue, int deltaPow = -1)
    {
        CalibrationParam param = Instantiate(paramPrefab);
        param.name = name;
        param.ValueOnUpdate = valueOnUpdate;
        param.SetValue = setValue;

        param.transform.SetParent(transform, false);

        calibrationParams.Add(param);

        deltaPows.Add(deltaPow);
    }

    private int selectedIndex;
    public int SelectedIndex
    {
        get
        {
            return selectedIndex;
        }

        set
        {
            calibrationParams[selectedIndex].IsHighlighted = false;
            selectedIndex = value;
            calibrationParams[selectedIndex].IsHighlighted = true;
        }
    }

    private void setupScene()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            Debug.LogError("You need to tag at least one camera as 'MainCamera'!!!");
            Debug.Break();
        }
        transform.parent.SetParent(mainCamera.transform, false);
    }
}
