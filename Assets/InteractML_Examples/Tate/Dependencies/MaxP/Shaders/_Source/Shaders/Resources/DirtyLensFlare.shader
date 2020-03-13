Shader "Hidden/Dirty Lens Flare" {
Properties {
	_MainTex     ("Base (RGB)",  2D)           = "white" {}
	_Flare       ("Flare",       2D)           = "black" {}
	_Dirt        ("Dirt",        2D)           = "white" {} 
	_Threshold   ("Threshold",   Range (0,1))  = 1.5
	_Scale       ("Scale",       Range(-10,10))= 1
	_BloomScale  ("Bloom Scale", Range(-10,10))= 1
	_desaturate  ("Desaturate",  Range(0,1))   = 0.4
}


	CGINCLUDE
	#include "UnityCG.cginc"
	
	uniform sampler2D _MainTex;
	uniform sampler2D _Flare;
	uniform sampler2D _Dirt;
	uniform float     _Threshold;
	uniform float     _Scale;
	uniform float     _BloomScale;
	uniform float     _desaturate;
	
	// Threshold
	fixed4 frag_threshold (v2f_img IN) : COLOR
	{
		// Get original color
		fixed4 original = tex2D(_MainTex, IN.uv);
		
		// Flip uv
		fixed2 uv = float2(1,1) - IN.uv;
		
		// Apply threshold
		fixed4 output = max( float4(0,0,0,0), tex2D(_MainTex, uv) - _Threshold ) * _Scale;
		
		// Desaturate
		output = Luminance(output.rgb) * _desaturate + output * (float4(1,1,1,1)-_desaturate);
		
		return output;
	}
	
	// Threshold + Bloom
	fixed4 frag_thresholdBloom (v2f_img IN) : COLOR
	{
		// Get original color
		fixed4 original = tex2D(_MainTex, IN.uv);
		
		// Get bloom
		fixed4 bloom = max( float4(0,0,0,0), original - _Threshold ) * _BloomScale;
		
		// Flip uv
		fixed2 uv = float2(1,1) - IN.uv;
		
		// Apply threshold
		fixed4 output = max( float4(0,0,0,0), tex2D(_MainTex, uv) - _Threshold ) * _Scale;
		
		// Desaturate
		output = Luminance(output.rgb) * _desaturate + output * (float4(1,1,1,1)-_desaturate);
		
		// Add bloom
		output += bloom;
		return output;
	}
	
	// Threshold + Bloom
	fixed4 frag_bloom (v2f_img IN) : COLOR
	{
		fixed4 original = tex2D(_MainTex, IN.uv);
		fixed4 bloom = max( float4(0,0,0,0), original - _Threshold ) * _BloomScale;
		return bloom;
	}
	
	// Additive blending
	fixed4 frag_blend (v2f_img IN) : COLOR
	{
		fixed4 output = tex2D(_MainTex, IN.uv) + tex2D(_Flare, IN.uv) * tex2D(_Dirt, IN.uv);
		return output;
	}
	
	// Additive blending (No Dirt)
	fixed4 frag_blend_noDirt (v2f_img IN) : COLOR
	{
		fixed4 output = tex2D(_MainTex, IN.uv) + tex2D(_Flare, IN.uv);
		return output;
	}
	
	ENDCG
	
SubShader {
	
	ZTest Always Cull Off ZWrite Off
	Fog { Mode off }
	
	// 0: Flare
	Pass
	{
		CGPROGRAM
		
		#pragma vertex vert_img
		#pragma fragment frag_threshold
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	
	}
	
	// 1: Flare + Bloom
	Pass
	{
		CGPROGRAM
		
		#pragma vertex vert_img
		#pragma fragment frag_thresholdBloom
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	
	}
	
	// 2: Bloom
	Pass
	{	
		CGPROGRAM
		
		#pragma vertex vert_img
		#pragma fragment frag_bloom
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	
	}
	
	// 3: Apply additive blending
	Pass 
	{	
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag_blend
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
	}
	
	// 4: Apply additive blending (no dirt)
	Pass 
	{	
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag_blend_noDirt
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
	}
	
}

Fallback off

}