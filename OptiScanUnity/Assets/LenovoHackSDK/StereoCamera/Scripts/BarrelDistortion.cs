using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class BarrelDistortion : PostEffectsBase
{
	public bool FovAuto = true;
	public float FovRadians = 1.69f;

	public Shader BarrelDistortionShader = null;
	public Material BarrelDistortionMaterial = null;

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (!CheckResources ()) {
			Graphics.Blit (src, dest);
		}

		if (FovAuto) {
			FovRadians = Camera.main.fieldOfView * Mathf.Deg2Rad;
		}

		BarrelDistortionMaterial.SetFloat ("_FOV", FovRadians);
		Graphics.Blit (src, dest, BarrelDistortionMaterial);
	}

	public bool CheckResources() {
		CheckSupport (false);
		BarrelDistortionMaterial = CheckShaderAndCreateMaterial (BarrelDistortionShader, BarrelDistortionMaterial);

		if (!isSupported) {
			ReportAutoDisable ();
		}

		return isSupported;
	}

}