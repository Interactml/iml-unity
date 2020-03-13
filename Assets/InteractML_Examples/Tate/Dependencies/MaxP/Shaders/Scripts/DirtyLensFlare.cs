using UnityEngine;
using System.Collections;

public enum LensFlareTypes { 
    BloomAndFlare = 0,
	Flare = 2,
	Bloom = 1
}

[RequireComponent (typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Max P/Dirty Lens Flare")]
public class DirtyLensFlare : MonoBehaviour {
	
	// Flare Settings
	public LensFlareTypes lensFlareType;
	public float saturation     = 0.9f;
	public float threshold      = 0.5f;
	public float flareIntensity = 2.5f;
	public float bloomIntensity = 2.0f;
	
	// Blur
	public int   iterations = 10;
	public float blurSpread = 0.6f;
	
	// Downsample
	public int   downsample = 6;
	
	// Textures
	public bool      useDirt = true;
	public Texture2D screenDirt;
	
	// Shaders and Materials
	private Shader   blurShader;
	private Material blurMaterial;
	
	// ------------------------------------------------------------------------------
	//  Image Effect Base (Default Unity Image Effect Script: Standard Assets/Image Effects (Pro Only)/ImageEffectBase.cs)
	// ------------------------------------------------------------------------------
	#region Image Effect Base
	private Shader   shader;
	private Material m_Material;
	
	// (Modified)
	protected virtual void Start ()
	{
		// Check Resources First
		CheckResources();
		
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		
		// Disable the image effect if the shader can't
		// run on the users graphics card
		if (!shader || !shader.isSupported)
			enabled = false;
	}
	
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material (shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		} 
	}
	
	protected virtual void OnDisable() {
		if( m_Material ) {
			DestroyImmediate( m_Material );
		}
	}
	#endregion
	
	
	// ------------------------------------------------------------------------------
	//  Blur (Default Unity Blur Shader Script: Standard Assets/Image Effects (Pro Only)/BlurEffect.cs)
	// ------------------------------------------------------------------------------
	#region Blur Shader Script
	// Performs one blur iteration.
	public void FourTapCone (RenderTexture source, RenderTexture dest, int iteration, Material blurMtl)
	{
		float off = 0.5f + iteration*blurSpread;
		Graphics.BlitMultiTap (source, dest, blurMtl,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}
	
	// Applies a blur effect (Modified from default script)
	void ApplyBlurPass(RenderTexture source, RenderTexture destination, Material blurMtl)
	{
		
		downsample = Mathf.Clamp(downsample, 1, 12);
		
		RenderTexture buffer  = RenderTexture.GetTemporary(source.width / downsample, source.height / downsample, 0);
		RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / downsample, source.height / downsample, 0);
		
		// Copy source into buffer
		Graphics.Blit(source, buffer);
		
		// Blur the small texture
		bool oddEven = true;
		for(int i = 0; i < iterations; i++)
		{
			if( oddEven )
				FourTapCone (buffer, buffer2, i, blurMtl);
			else
				FourTapCone (buffer2, buffer, i, blurMtl);
			oddEven = !oddEven;
		}
		if( oddEven )
			Graphics.Blit(buffer, destination);
		else
			Graphics.Blit(buffer2, destination);
		
		// Release the buffers
		RenderTexture.ReleaseTemporary(buffer);
		RenderTexture.ReleaseTemporary(buffer2);
	}
	#endregion
	
	// ------------------------------------------------------------------------------
	//  Dirty Lens Flare MAIN
	// ------------------------------------------------------------------------------
	#region Dirty Lens Flare
	
	bool CheckResources()
	{
		// Check blur shader
		if(!blurShader)
		{
			blurShader = Shader.Find("Hidden/Dirty Lens Flare Blur");
			if(!blurShader)
				return false;
		}
		
		// Check blur material
		if(!blurMaterial)
		{
			blurMaterial = new Material(blurShader);
			blurMaterial.hideFlags = HideFlags.HideAndDontSave;
			if(!blurMaterial)
				return false;
		}
		
		// Check dirty lens flare shader
		if(!shader)
		{
			shader = Shader.Find("Hidden/Dirty Lens Flare");
			if(!shader)
				return false;
		}
		
		return true;
	}
	
	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		
		if( CheckResources() )
		{
			
			material.SetFloat("_Threshold",  threshold);
			material.SetFloat("_Scale",      flareIntensity);
			material.SetFloat("_BloomScale", bloomIntensity);
			
			// Create downsampled image
			RenderTexture downSampled = RenderTexture.GetTemporary (source.width / 4, source.height / 4, 0, RenderTextureFormat.Default);
			
			// Apply threshold
			material.SetFloat("_desaturate", 1.0f-saturation);
			switch(lensFlareType)
			{
			case LensFlareTypes.Bloom:
				Graphics.Blit(source, downSampled, material, 2); // Bloom only
				break;
			case LensFlareTypes.Flare:
				Graphics.Blit(source, downSampled, material, 0); // Flare only
				break;
			case LensFlareTypes.BloomAndFlare:
				Graphics.Blit(source, downSampled, material, 1); // Flare + Bloom
				break;
			}
			
			// Create blur buffer
			RenderTexture blurred = RenderTexture.GetTemporary (downSampled.width, downSampled.height, 0, RenderTextureFormat.Default);	
			
			// Apply blur
			ApplyBlurPass(downSampled, blurred, blurMaterial);
			
			// Apply blending
			material.SetTexture("_Flare", blurred);
			if( useDirt )
			{
				material.SetTexture("_Dirt", screenDirt);
				Graphics.Blit (source, destination, material, 3);
			}
			else
			{
				Graphics.Blit (source, destination, material, 4);
			}
			
			// Release downsampled images
			RenderTexture.ReleaseTemporary (downSampled);
			RenderTexture.ReleaseTemporary (blurred);
			
		}
		
	}
	#endregion
	
}
