Shader "NatureManufacture Shaders/Trees/Tree_Leaves_Specular"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_HealthyColor("Healthy Color", Color) = (1,0.9735294,0.9338235,1)
		_DryColor("Dry Color", Color) = (0.8676471,0.818369,0.6124567,1)
		_ColorNoiseSpread("Color Noise Spread", Float) = 50
		[NoScaleOffset]_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 3)) = 1
		_SpecularPower("Specular Power", Range( 0 , 2)) = 0
		[NoScaleOffset]_AmbientOcclusionGSmoothnessA("Ambient Occlusion (G) Smoothness (A)", 2D) = "white" {}
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 0
		_InitialBend("Wind Initial Bend", Float) = 1
		_Stiffness("Wind Stiffness", Float) = 1
		_Drag("Wind Drag", Float) = 1
		_ShiverDrag("Wind Shiver Drag", Float) = 0.05
		_WindNormalInfluence("Wind Normal Influence", Float) = 0
		_ShiverDirectionality("Wind Shiver Directionality", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#include "NMWind.cginc"
		#include "NM_indirect.cginc"
		#pragma vertex vert
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _DryColor;
		uniform float4 _HealthyColor;
		uniform float _ColorNoiseSpread;
		uniform float _SpecularPower;
		uniform sampler2D _AmbientOcclusionGSmoothnessA;
		uniform float _SmoothnessPower;
		uniform float _AmbientOcclusionPower;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, i.uv_texcoord ) ,_BumpScale );
			o.Normal = tex2DNode4;
			float4 tex2DNode3 = tex2D( _MainTex, i.uv_texcoord );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult357 = (float2(ase_worldPos.x , ase_worldPos.z));
			float simplePerlin2D347 = snoise( ( appendResult357 / _ColorNoiseSpread ) );
			float4 lerpResult363 = lerp( _DryColor , _HealthyColor , simplePerlin2D347);
			float4 temp_output_35_0 = ( tex2DNode3 * lerpResult363 );
			o.Albedo = temp_output_35_0.rgb;
			o.Specular = ( temp_output_35_0 * _SpecularPower ).rgb;
			float4 tex2DNode37 = tex2D( _AmbientOcclusionGSmoothnessA, i.uv_texcoord );
			o.Smoothness = ( tex2DNode37.a * _SmoothnessPower );
			float clampResult41 = clamp( tex2DNode37.g , ( 1.0 - _AmbientOcclusionPower ) , 1.0 );
			o.Occlusion = clampResult41;
			o.Alpha = 1;
			clip( tex2DNode3.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}