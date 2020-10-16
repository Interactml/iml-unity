Shader "NatureManufacture Shaders/Decal Metalic Road Material Parallax ArrayTrial"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		MainRoadIndex("Main Road Index", Range( 0 , 16)) = 0
		_MainRoadBrightness("Main Road Brightness", Float) = 1
		_BumpScale("Main Road BumpScale", Range( 0 , 5)) = 0
		_MainRoadMetalicPower("Main Road Metalic Power", Range( 0 , 2)) = 1
		_MainRoadAmbientOcclusionPower("Main Road Ambient Occlusion Power", Range( 0 , 1)) = 1
		_MainRoadSmoothnessPower("Main Road Smoothness Power", Range( 0 , 2)) = 1
		_MainRoadParallaxPower("Main Road Parallax Power", Range( -0.1 , 0.1)) = 0
		_DetailMask("DetailMask (A)", 2D) = "white" {}
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailAlbedoPower("Main Road Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("Main Road DetailNormalMapScale", Range( 0 , 5)) = 0
		_ArrayMainRoadAlbedo_T("Array Main Road Albedo_T", 2DArray ) = "" {}
		_ArrayMainRoadNormal("Array Main Road Normal", 2DArray ) = "" {}
		_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA("Array Main Road Metallic (R) Ambient Occlusion (G) Height (B) Smoothness (A)", 2DArray ) = "" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "AlphaTest+0" "Offset"="-2, -2" "ForceNoShadowCasting"="True" }
		Cull Off
		ZTest LEqual
		Offset  -3 , 0
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.5
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma multi_compile_fog
		#include "UnityPBSLighting.cginc"
		#pragma exclude_renderers gles
		#pragma surface surf Standard keepalpha  decal:blend
		struct Input
		{
			half2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform UNITY_DECLARE_TEX2DARRAY( _ArrayMainRoadNormal );
		uniform UNITY_DECLARE_TEX2DARRAY( _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA );
		uniform float4 _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST;
		uniform half MainRoadIndex;
		uniform half _MainRoadParallaxPower;
		uniform half _BumpScale;
		uniform half _DetailNormalMapScale;
		uniform sampler2D _DetailNormalMap;
		uniform sampler2D _DetailAlbedoMap;
		uniform float4 _DetailAlbedoMap_ST;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform half _MainRoadBrightness;
		uniform UNITY_DECLARE_TEX2DARRAY( _ArrayMainRoadAlbedo_T );
		uniform half4 _Color;
		uniform half _DetailAlbedoPower;
		uniform half _MainRoadMetalicPower;
		uniform half _MainRoadSmoothnessPower;
		uniform half _MainRoadAmbientOcclusionPower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA = i.uv_texcoord * _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.xy + _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.zw;
			float4 texArray782 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, MainRoadIndex)  );
			float2 Offset710 = ( ( texArray782.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + i.uv_texcoord;
			float4 texArray785 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, MainRoadIndex)  );
			float2 Offset728 = ( ( texArray785.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset710;
			float4 texArray786 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset728, MainRoadIndex)  );
			float2 Offset754 = ( ( texArray786.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset728;
			float4 texArray787 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset754, MainRoadIndex)  );
			float2 Offset778 = ( ( texArray787.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset754;
			float4 texArray780 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadNormal, float3(Offset778, MainRoadIndex)  );
			float2 appendResult11_g1 = (half2(texArray780.a , texArray780.g));
			float2 temp_output_4_0_g1 = ( ( ( appendResult11_g1 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _BumpScale );
			float2 break8_g1 = temp_output_4_0_g1;
			float dotResult5_g1 = dot( temp_output_4_0_g1 , temp_output_4_0_g1 );
			float temp_output_9_0_g1 = sqrt( ( 1.0 - saturate( dotResult5_g1 ) ) );
			float3 appendResult20_g1 = (half3(break8_g1.x , break8_g1.y , temp_output_9_0_g1));
			float3 temp_output_783_0 = appendResult20_g1;
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			half4 tex2DNode481 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult479 = lerp( temp_output_783_0 , BlendNormals( temp_output_783_0 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ), _DetailNormalMapScale ) ) , tex2DNode481.a);
			o.Normal = lerpResult479;
			float4 texArray779 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadAlbedo_T, float3(Offset778, MainRoadIndex)  );
			float4 temp_output_77_0 = ( ( _MainRoadBrightness * texArray779 ) * _Color );
			half4 blendOpSrc474 = temp_output_77_0;
			half4 blendOpDest474 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , ( _DetailAlbedoPower * tex2DNode481.a ));
			o.Albedo = lerpResult480.rgb;
			float4 texArray781 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset778, MainRoadIndex)  );
			o.Metallic = ( texArray781.r * _MainRoadMetalicPower );
			o.Smoothness = ( texArray781.a * _MainRoadSmoothnessPower );
			float clampResult96 = clamp( texArray781.g , ( 1.0 - _MainRoadAmbientOcclusionPower ) , 1.0 );
			o.Occlusion = clampResult96;
			o.Alpha = ( texArray779.a * _Color.a );
		}

		ENDCG
	}
	Fallback "Diffuse"
}