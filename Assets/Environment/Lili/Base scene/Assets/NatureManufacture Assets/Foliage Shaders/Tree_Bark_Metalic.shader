Shader "NatureManufacture Shaders/Trees/Tree Bark Metalic"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset]_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 5)) = 1
		[NoScaleOffset]_MetalicRAOGSmothnessA("Metalic (R) AO (G) Smothness (A)", 2D) = "white" {}
		_MetallicPower("Metallic Power", Range( 0 , 2)) = 0
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 0
		_DetailMask("DetailMask", 2D) = "black" {}
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "white" {}
		[Toggle(_DETALUSEUV3_ON)] _DetalUseUV3("Detal Use UV3", Float) = 0
		[NoScaleOffset]_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 1
		[NoScaleOffset]_DetailMetalicRAOGSmothnessA("Detail Metalic (R) AO (G) Smothness (A) ", 2D) = "white" {}
		_InitialBend("Wind Initial Bend", Float) = 1
		_Stiffness("Wind Stiffness", Float) = 1
		_Drag("Wind Drag", Float) = 1
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _DETALUSEUV3_ON
		#include "NMWindNoShiver.cginc"
		#include "NM_indirect.cginc"
		#pragma vertex vert
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv3_texcoord3;
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _DetailNormalMapScale;
		uniform sampler2D _DetailNormalMap;
		uniform sampler2D _DetailAlbedoMap;
		uniform float4 _DetailAlbedoMap_ST;
		uniform float4 _DetailNormalMap_ST;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform float4 _Color;
		uniform sampler2D _MetalicRAOGSmothnessA;
		uniform sampler2D _DetailMetalicRAOGSmothnessA;
		uniform float _MetallicPower;
		uniform float _SmoothnessPower;
		uniform float _AmbientOcclusionPower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float2 uv_DetailNormalMap = i.uv3_texcoord3 * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw;
			#ifdef _DETALUSEUV3_ON
				float2 staticSwitch280 = uv_DetailNormalMap;
			#else
				float2 staticSwitch280 = uv_DetailAlbedoMap;
			#endif
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			float4 tex2DNode25 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult19 = lerp( UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_BumpScale ) , UnpackScaleNormal( tex2D( _DetailNormalMap, staticSwitch280 ) ,_DetailNormalMapScale ) , tex2DNode25.a);
			o.Normal = lerpResult19;
			float4 lerpResult16 = lerp( tex2D( _MainTex, uv_MainTex ) , tex2D( _DetailAlbedoMap, staticSwitch280 ) , tex2DNode25.a);
			o.Albedo = ( lerpResult16 * _Color ).rgb;
			float4 lerpResult18 = lerp( tex2D( _MetalicRAOGSmothnessA, uv_MainTex ) , tex2D( _DetailMetalicRAOGSmothnessA, staticSwitch280 ) , tex2DNode25.a);
			float4 break22 = lerpResult18;
			o.Metallic = ( break22.r * _MetallicPower );
			o.Smoothness = ( break22.a * _SmoothnessPower );
			float clampResult31 = clamp( break22.g , ( 1.0 - _AmbientOcclusionPower ) , 1.0 );
			o.Occlusion = clampResult31;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}