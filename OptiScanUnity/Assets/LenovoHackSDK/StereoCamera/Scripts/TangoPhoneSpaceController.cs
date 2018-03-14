using UnityEngine;
using System.Collections;

public class TangoPhoneSpaceController : PhoneSpaceControllerBase
{

    public Camera MultiCamera;
    public Camera LeftCamera;
    public Camera RightCamera;

    public float LocalFrameOffsetX = 0.133f;
    public float LocalFrameOffsetY = 0.245f;
    public float LocalFrameAllX = 0.015f;
    private BarrelDistortion leftBarrel;
    private BarrelDistortion rightBarrel;

    private bool previousCameraSeeThrough;
    public override float FrameAllX
    {
        get
        {
            return LocalFrameAllX;
        }
        set
        {
            LocalFrameAllX = value;
            Rect leftRect = LeftCamera.rect;
            Rect rightRect = RightCamera.rect;
            leftRect.x = LocalFrameAllX + LocalFrameOffsetX;
            leftRect.width = 0.5f - LocalFrameOffsetX;
            rightRect.x = 0.5f + LocalFrameAllX;
            rightRect.width = leftRect.width;
            LeftCamera.rect = leftRect;
            RightCamera.rect = rightRect;
        }
    }

    public override float FrameOffsetX
    {
        get
        {
            return LocalFrameOffsetX;
        }
        set
        {
            LocalFrameOffsetX = value;
            Rect leftRect = LeftCamera.rect;
            Rect rightRect = RightCamera.rect;
            leftRect.x = LocalFrameOffsetX + LocalFrameAllX;
            leftRect.width = 0.5f - LocalFrameOffsetX;
            rightRect.x = 0.5f + LocalFrameAllX;
            rightRect.width = leftRect.width;
            LeftCamera.rect = leftRect;
            RightCamera.rect = rightRect;
        }
    }

    public override float FrameOffsetY
    {
        get
        {
            return LocalFrameOffsetY;
        }
        set
        {
            LocalFrameOffsetY = value;
            Rect leftRect = LeftCamera.rect;
            Rect rightRect = RightCamera.rect;
            leftRect.y = LocalFrameOffsetY;
            rightRect.y = LocalFrameOffsetY;
            leftRect.height = 1.0f - Mathf.Abs(LocalFrameOffsetY);
            rightRect.height = 1.0f - Mathf.Abs(LocalFrameOffsetY);
            LeftCamera.rect = leftRect;
            RightCamera.rect = rightRect;
        }
    }
    public override float FOV
    {
        get
        {
            return LeftCamera.fieldOfView;
        }
        set
        {
            LeftCamera.fieldOfView = value;
            RightCamera.fieldOfView = value;
            MultiCamera.fieldOfView = value;
        }
    }

    public override float BarrelRadius
    {
        get
        {
            return leftBarrel.FovRadians;
        }
        set
        {
            leftBarrel.FovRadians = value;
            rightBarrel.FovRadians = value;
        }
    }

    public override float PupilDistance
    {
        get
        {
            return RightCamera.transform.localPosition.x - LeftCamera.transform.localPosition.x;
        }
        set
        {
            float halfDist = value / 2;
            LeftCamera.transform.localPosition = new Vector3(-halfDist, 0, 0);
            RightCamera.transform.localPosition = new Vector3(halfDist, 0, 0);
        }
    }

    public override Vector3 CameraOffset
    {
        get
        {
            return MultiCamera.transform.localPosition;
        }

        set
        {
            MultiCamera.transform.localPosition = value;
        }
    }

    void Start()
    {
        leftBarrel = LeftCamera.GetComponent<BarrelDistortion>();
        rightBarrel = RightCamera.GetComponent<BarrelDistortion>();

		StartCoroutine(fullScreenClear());
    }

	private IEnumerator fullScreenClear() {
		CameraClearFlags oldClearFlags = MultiCamera.clearFlags;
		Color oldBackgroundColor = MultiCamera.backgroundColor;
		bool oldEnabled = MultiCamera.enabled;

		MultiCamera.enabled = true;
		MultiCamera.clearFlags = CameraClearFlags.SolidColor;
		MultiCamera.backgroundColor = Color.black;

		yield return null;
		yield return null;

		MultiCamera.enabled = oldEnabled;
		MultiCamera.clearFlags = oldClearFlags;
		MultiCamera.backgroundColor = oldBackgroundColor;
	}
}
