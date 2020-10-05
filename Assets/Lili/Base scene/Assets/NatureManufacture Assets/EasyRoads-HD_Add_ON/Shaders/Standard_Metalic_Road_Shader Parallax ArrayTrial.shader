Shader "NatureManufacture Shaders/Standard Metalic Road Material Parallax ArrayTrial"
{
	Properties
	{
		MainRoadIndex("Main Road Index", Range( 0 , 16)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainRoadColor("Main Road Color", Color) = (1,1,1,1)
		_MainRoadBrightness("Main Road Brightness", Float) = 1
		_MainRoadAlphaCutOut("Main Road Alpha CutOut", Range( 0 , 2)) = 1
		_BumpScale("Main Road BumpScale", Range( 0 , 5)) = 0
		_MainRoadMetalicPower("Main Road Metalic Power", Range( 0 , 2)) = 0
		_MainRoadAmbientOcclusionPower("Main Road Ambient Occlusion Power", Range( 0 , 1)) = 1
		_MainRoadSmoothnessPower("Main Road Smoothness Power", Range( 0 , 2)) = 1
		_DetailMask("DetailMask (A)", 2D) = "white" {}
		_MainRoadParallaxPower("Main Road Parallax Power", Range( 0 , 0.1)) = 0
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailAlbedoPower("Main Road Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("Main Road DetailNormalMapScale", Range( 0 , 5)) = 0
		_ArrayMainRoadAlbedo_T("Array Main Road Albedo_T", 2DArray ) = "" {}
		_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA("Array Main Road Metallic (R) Ambient Occlusion (G) Height (B) Smoothness (A)", 2DArray ) = "" {}
		_ArrayMainRoadNormal("Array Main Road Normal", 2DArray ) = "" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		ZTest LEqual
		Offset  -2 , 0
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
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
		uniform half4 _MainRoadColor;
		uniform half _DetailAlbedoPower;
		uniform half _MainRoadMetalicPower;
		uniform half _MainRoadSmoothnessPower;
		uniform half _MainRoadAmbientOcclusionPower;
		uniform half _MainRoadAlphaCutOut;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA = i.uv_texcoord * _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.xy + _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.zw;
			float4 texArray803 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, MainRoadIndex)  );
			float2 Offset710 = ( ( texArray803.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + i.uv_texcoord;
			float4 texArray804 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset710, MainRoadIndex)  );
			float2 Offset728 = ( ( texArray804.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset710;
			float4 texArray805 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset728, MainRoadIndex)  );
			float2 Offset754 = ( ( texArray805.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset728;
			float4 texArray806 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset754, MainRoadIndex)  );
			float2 Offset778 = ( ( texArray806.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset754;
			float4 texArray800 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadNormal, float3(Offset778, MainRoadIndex)  );
			float2 appendResult11_g1 = (half2(texArray800.a , texArray800.g));
			float2 temp_output_4_0_g1 = ( ( ( appendResult11_g1 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _BumpScale );
			float2 break8_g1 = temp_output_4_0_g1;
			float dotResult5_g1 = dot( temp_output_4_0_g1 , temp_output_4_0_g1 );
			float temp_output_9_0_g1 = sqrt( ( 1.0 - saturate( dotResult5_g1 ) ) );
			float3 appendResult20_g1 = (half3(break8_g1.x , break8_g1.y , temp_output_9_0_g1));
			float3 temp_output_808_0 = appendResult20_g1;
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			half4 tex2DNode786 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult798 = lerp( temp_output_808_0 , BlendNormals( temp_output_808_0 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ), _DetailNormalMapScale ) ) , tex2DNode786.a);
			o.Normal = lerpResult798;
			float4 texArray801 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadAlbedo_T, float3(Offset778, MainRoadIndex)  );
			float4 temp_output_77_0 = ( ( _MainRoadBrightness * texArray801 ) * _MainRoadColor );
			half4 blendOpSrc787 = temp_output_77_0;
			half4 blendOpDest787 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult797 = lerp( temp_output_77_0 , (( blendOpDest787 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest787 - 0.5 ) ) * ( 1.0 - blendOpSrc787 ) ) : ( 2.0 * blendOpDest787 * blendOpSrc787 ) ) , ( _DetailAlbedoPower * tex2DNode786.a ));
			o.Albedo = lerpResult797.rgb;
			float4 texArray799 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset778, MainRoadIndex)  );
			o.Metallic = ( texArray799.r * _MainRoadMetalicPower );
			o.Smoothness = ( texArray799.a * _MainRoadSmoothnessPower );
			float clampResult96 = clamp( texArray799.g , ( 1.0 - _MainRoadAmbientOcclusionPower ) , 1.0 );
			o.Occlusion = clampResult96;
			o.Alpha = 1;
			clip( ( texArray801.a * _MainRoadAlphaCutOut ) - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}