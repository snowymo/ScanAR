using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CalibrationTuner : MonoBehaviour {
	public CalibrationParam paramPrefab;

	public PhoneSpaceControllerBase phoneSpaceController;
	
	public Text deltaLabel;

	private List<CalibrationParam> calibrationParams = new List<CalibrationParam>();

	private List<int> deltaPows = new List<int>();

	private const int deltaBase = 10;

	void Start () {
		setupScene();
        CreateParam("FrameAX",
            () => phoneSpaceController.FrameAllX,
            (value) => {
                phoneSpaceController.FrameAllX = value;
            },
            deltaPow: -3

        );
        CreateParam("FrameX",
            () => phoneSpaceController.FrameOffsetX,
            (value) => {
                phoneSpaceController.FrameOffsetX = value;
            },
            deltaPow: -3

        );
        CreateParam("FrameY",
            () => phoneSpaceController.FrameOffsetY,
            (value) => {
                phoneSpaceController.FrameOffsetY = value;
            },
            deltaPow: -3

        );
        CreateParam("Offset X",
			() => phoneSpaceController.CameraOffset.x, 
			(value) => {
				Vector3 localPos = phoneSpaceController.CameraOffset;
				phoneSpaceController.CameraOffset = new Vector3(value, localPos.y, localPos.z);
			},
			deltaPow: -2

		);

		CreateParam("Offset Y",
			() => phoneSpaceController.CameraOffset.y, 
			(value) => {
				Vector3 localPos = phoneSpaceController.CameraOffset;
				phoneSpaceController.CameraOffset = new Vector3(localPos.x, value, localPos.z);
			},
			deltaPow: -2
		);

		CreateParam("Offset Z",
			() => phoneSpaceController.CameraOffset.z, 
			(value) => {
				Vector3 localPos = phoneSpaceController.CameraOffset;
				phoneSpaceController.CameraOffset = new Vector3(localPos.x, localPos.y, value);
			},
			deltaPow: -2
		);

		CreateParam("FOV", 
			() => phoneSpaceController.FOV,
			(value) => {
				phoneSpaceController.FOV = value;
			},
			deltaPow: 0
		);

		CreateParam("Barrel", 
			() => phoneSpaceController.BarrelRadius,
			(value) => {
				phoneSpaceController.BarrelRadius = value;
			},
			deltaPow: -3
		);

		CreateParam("IPD",
			() => phoneSpaceController.PupilDistance,
			(value) => {
				phoneSpaceController.PupilDistance = value;
			},
			deltaPow: -3
		);

		SelectedIndex = 0;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.S) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('s')) {
			SelectedIndex = (SelectedIndex + 1) % calibrationParams.Count;
		} else if (Input.GetKeyDown(KeyCode.W) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('w')) {
			int selected = (SelectedIndex - 1) % calibrationParams.Count;
			if (selected < 0) {
				selected += calibrationParams.Count; 
			}
			SelectedIndex = selected;
		} else if (Input.GetKeyDown(KeyCode.Q) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('q')) {
			deltaPows[SelectedIndex]++;
		} else if (Input.GetKeyDown(KeyCode.E) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('e')) {
			deltaPows[SelectedIndex]--;
		} else if (Input.GetKey(KeyCode.D) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('d')) {
			CalibrationParam param = calibrationParams[SelectedIndex];
			float value = param.ValueOnUpdate();
			value += Mathf.Pow(deltaBase, deltaPows[SelectedIndex]);
			param.SetValue(value);
		} else if (Input.GetKey(KeyCode.A) ||
			RemoteKeyboardReceiver.Instance.GetKeyDown('a')) {
			CalibrationParam param = calibrationParams[SelectedIndex];
			float value = param.ValueOnUpdate();
			value -= Mathf.Pow(deltaBase, deltaPows[SelectedIndex]);
			param.SetValue(value);
        }
        else if (Input.GetKey(KeyCode.Space) ||
          RemoteKeyboardReceiver.Instance.GetKeyDown('p'))
        {
            // hide the panel
            transform.parent.gameObject.GetComponent<Canvas>().enabled = false;
        }

        deltaLabel.text = string.Format("Delta = {0}", Mathf.Pow(deltaBase, deltaPows[SelectedIndex]));
	}

	private void CreateParam(string name, Func<float> valueOnUpdate, Action<float> setValue, int deltaPow = -1) {
		CalibrationParam param = Instantiate(paramPrefab);
		param.name = name;
		param.ValueOnUpdate = valueOnUpdate;
		param.SetValue = setValue;

		param.transform.SetParent(transform, false);

		calibrationParams.Add(param);

		deltaPows.Add(deltaPow);
	}

	private int selectedIndex;
	public int SelectedIndex {
		get {
			return selectedIndex;
		}

		set {
			calibrationParams[selectedIndex].IsHighlighted = false;
			selectedIndex = value;
			calibrationParams[selectedIndex].IsHighlighted = true;
		}
	}

	private void setupScene() {
		GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		if (mainCamera == null) {
			Debug.LogError("You need to tag at least one camera as 'MainCamera'!!!");
			Debug.Break();
		}
		transform.parent.SetParent(mainCamera.transform, false);
	}
}
