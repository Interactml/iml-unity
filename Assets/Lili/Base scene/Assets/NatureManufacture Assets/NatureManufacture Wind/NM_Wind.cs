﻿using UnityEngine;

[ExecuteInEditMode]
public class NM_Wind : MonoBehaviour
{

	[Header ("General Parameters")]
	[Tooltip ("Wind Speed in Kilometers per hour")]
	public float WindSpeed = 30;
	[Range (0.0f, 2.0f)]
	[Tooltip ("Wind Turbulence in percentage of wind Speed")]
	public float Turbulence = 0.25f;


	[Header ("Noise Parameters")]
	[Tooltip ("Texture used for wind turbulence")]
	public Texture2D NoiseTexture;
	[Tooltip ("Size of one world tiling patch of the Noise Texture, for bending trees")]
	public float FlexNoiseWorldSize = 175.0f;
	[Tooltip ("Size of one world tiling patch of the Noise Texture, for leaf shivering")]
	public float ShiverNoiseWorldSize = 10.0f;

	[Header ("Gust Parameters")]
	[Tooltip ("Texture used for wind gusts")]
	public Texture2D GustMaskTexture;
	[Tooltip ("Size of one world tiling patch of the Gust Texture, for leaf shivering")]
	public float GustWorldSize = 600.0f;

	[Tooltip ("Wind Gust Speed in Kilometers per hour")]
	public float GustSpeed = 50;
	[Tooltip ("Wind Gust Influence on trees")]
	public float GustScale = 1.0f;


	// Use this for initialization
	void Start ()
	{
		ApplySettings ();
	}

	// Update is called once per frame
	void Update ()
	{
		ApplySettings ();
	}

	void OnValidate ()
	{
		ApplySettings ();
	}

	void ApplySettings ()
	{
		Shader.SetGlobalTexture ("WIND_SETTINGS_TexNoise", NoiseTexture);
		Shader.SetGlobalTexture ("WIND_SETTINGS_TexGust", GustMaskTexture);
		Shader.SetGlobalVector ("WIND_SETTINGS_WorldDirectionAndSpeed", GetDirectionAndSpeed ());
		Shader.SetGlobalFloat ("WIND_SETTINGS_FlexNoiseScale", 1.0f / Mathf.Max (0.01f, FlexNoiseWorldSize));
		Shader.SetGlobalFloat ("WIND_SETTINGS_ShiverNoiseScale", 1.0f / Mathf.Max (0.01f, ShiverNoiseWorldSize));
		Shader.SetGlobalFloat ("WIND_SETTINGS_Turbulence", WindSpeed * Turbulence);
		Shader.SetGlobalFloat ("WIND_SETTINGS_GustSpeed", GustSpeed);
		Shader.SetGlobalFloat ("WIND_SETTINGS_GustScale", GustScale);
		Shader.SetGlobalFloat ("WIND_SETTINGS_GustWorldScale", 1.0f / Mathf.Max (0.01f, GustWorldSize));
	}

	Vector4 GetDirectionAndSpeed ()
	{
		Vector3 dir = transform.forward.normalized;
		return new Vector4 (dir.x, dir.y, dir.z, WindSpeed * 0.2777f);
	}

}