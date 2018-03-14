using UnityEngine; 

public abstract class PhoneSpaceControllerBase : MonoBehaviour {
	public abstract float FOV {get; set;}
	public abstract float BarrelRadius {get; set;}
	public abstract float PupilDistance {get; set;}
	public abstract Vector3 CameraOffset {get; set;}
    public abstract float FrameOffsetX { get; set; }
    public abstract float FrameOffsetY { get; set; }
    public abstract float FrameAllX { get; set; }
}
