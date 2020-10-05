Shader "NatureManufacture Shaders/Standard Metalic Cross Road Material Parallax ArrayTrial"
{
	Properties
	{
		_Float2("Cross Road Index", Range( 0 , 16)) = 0
		MainRoadIndex("Main Road Index", Range( 0 , 16)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainRoadColor("Main Road Color", Color) = (1,1,1,1)
		_MainRoadBrightness("Main Road Brightness", Float) = 1
		[Toggle]_MainRoadUV3("Main Road UV3", Float) = 0
		_MainRoadAlphaCutOut("Main Road Alpha CutOut", Range( 0 , 2)) = 1
		_BumpScale("Main Road BumpScale", Range( 0 , 5)) = 0
		_MainRoadMetalicPower("Main Road Metalic Power", Range( 0 , 2)) = 0
		_MainRoadAmbientOcclusionPower("Main Road Ambient Occlusion Power", Range( 0 , 1)) = 1
		_MainRoadParallaxPower("Main Road Parallax Power", Range( 0 , 0.1)) = 0
		_MainRoadSmoothnessPower("Main Road Smoothness Power", Range( 0 , 2)) = 1
		_CrossRoadColor("Cross Road Color", Color) = (1,1,1,1)
		_CrossRoadBrightness("Cross Road Brightness", Float) = 1
		[Toggle]_CrossRoadUV3("Cross Road UV3", Float) = 0
		[Toggle(_IGNORECROSSROADALPHA_ON)] _IgnoreCrossRoadAlpha("Ignore Cross Road Alpha", Float) = 0
		_CrossRoadAlphaCutOut("Cross Road Alpha CutOut", Range( 0 , 2)) = 1
		_CrossRoadNormalScale("Cross Road Normal Scale", Range( 0 , 5)) = 0
		_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA("Array Main Road Metallic (R) Ambient Occlusion (G) Height (B) Smoothness (A)", 2DArray ) = "" {}
		_CrossRoadMetallicPower("Cross Road Metallic Power", Range( 0 , 2)) = 1
		_CrossRoadAmbientOcclusionPower("Cross Road Ambient Occlusion Power", Range( 0 , 1)) = 0
		_CrossRoadParallaxPower("Cross Road Parallax Power", Range( -0.1 , 0.1)) = 0
		_CrossRoadSmoothnessPower("Cross Road Smoothness Power", Range( 0 , 2)) = 1
		_DetailMask("DetailMask (A)", 2D) = "white" {}
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailAlbedoPower("Main Road Detail Albedo Power", Range( 0 , 2)) = 0
		_Float3("Cross Road Detail Albedo Power", Range( 0 , 2)) = 2
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("Main Road DetailNormalMapScale", Range( 0 , 5)) = 0
		_Float0("Cross Road Detail NormalMap Scale", Range( 0 , 5)) = 0
		_ArrayMainRoadAlbedo_T("Array Main Road Albedo_T", 2DArray ) = "" {}
		_ArrayMainRoadNormal("Array Main Road Normal", 2DArray ) = "" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
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
		Offset  -3 , 0
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature _IGNORECROSSROADALPHA_ON
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
			half2 uv3_texcoord3;
			float3 viewDir;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform UNITY_DECLARE_TEX2DARRAY( _ArrayMainRoadNormal );
		uniform half _MainRoadUV3;
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
		uniform half _CrossRoadUV3;
		uniform half _Float2;
		uniform half _CrossRoadParallaxPower;
		uniform half _CrossRoadNormalScale;
		uniform half _Float0;
		uniform half _MainRoadBrightness;
		uniform UNITY_DECLARE_TEX2DARRAY( _ArrayMainRoadAlbedo_T );
		uniform half4 _MainRoadColor;
		uniform half _DetailAlbedoPower;
		uniform half _CrossRoadBrightness;
		uniform half4 _CrossRoadColor;
		uniform half _Float3;
		uniform half _MainRoadMetalicPower;
		uniform half _CrossRoadMetallicPower;
		uniform half _MainRoadSmoothnessPower;
		uniform half _CrossRoadSmoothnessPower;
		uniform half _MainRoadAmbientOcclusionPower;
		uniform half _CrossRoadAmbientOcclusionPower;
		uniform half _MainRoadAlphaCutOut;
		uniform half _CrossRoadAlphaCutOut;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA = i.uv_texcoord * _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.xy + _ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA_ST.zw;
			float4 texArray850 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, MainRoadIndex)  );
			float2 Offset724 = ( ( texArray850.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + lerp(i.uv_texcoord,i.uv3_texcoord3,_MainRoadUV3);
			float4 texArray849 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset724, MainRoadIndex)  );
			float2 Offset744 = ( ( texArray849.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset724;
			float4 texArray848 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset744, MainRoadIndex)  );
			float2 Offset766 = ( ( texArray848.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset744;
			float4 texArray847 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset766, MainRoadIndex)  );
			float2 Offset795 = ( ( texArray847.b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset766;
			float4 texArray853 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadNormal, float3(Offset795, MainRoadIndex)  );
			float2 appendResult11_g3 = (half2(texArray853.a , texArray853.g));
			float2 temp_output_4_0_g3 = ( ( ( appendResult11_g3 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _BumpScale );
			float2 break8_g3 = temp_output_4_0_g3;
			float dotResult5_g3 = dot( temp_output_4_0_g3 , temp_output_4_0_g3 );
			float temp_output_9_0_g3 = sqrt( ( 1.0 - saturate( dotResult5_g3 ) ) );
			float3 appendResult20_g3 = (half3(break8_g3.x , break8_g3.y , temp_output_9_0_g3));
			float3 temp_output_854_0 = appendResult20_g3;
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			half4 tex2DNode481 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult479 = lerp( temp_output_854_0 , BlendNormals( temp_output_854_0 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ), _DetailNormalMapScale ) ) , tex2DNode481.a);
			float4 texArray859 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(uv_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, _Float2)  );
			float2 Offset805 = ( ( texArray859.b - 1 ) * i.viewDir.xy * _CrossRoadParallaxPower ) + lerp(i.uv_texcoord,i.uv3_texcoord3,_CrossRoadUV3);
			float4 texArray858 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset805, _Float2)  );
			float2 Offset815 = ( ( texArray858.b - 1 ) * i.viewDir.xy * _CrossRoadParallaxPower ) + Offset805;
			float4 texArray857 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset815, _Float2)  );
			float2 Offset839 = ( ( texArray857.b - 1 ) * i.viewDir.xy * _CrossRoadParallaxPower ) + Offset815;
			float4 texArray856 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset839, _Float2)  );
			float2 Offset838 = ( ( texArray856.b - 1 ) * i.viewDir.xy * _CrossRoadParallaxPower ) + Offset839;
			float4 texArray862 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadNormal, float3(Offset838, _Float2)  );
			float2 appendResult11_g2 = (half2(texArray862.a , texArray862.g));
			float2 temp_output_4_0_g2 = ( ( ( appendResult11_g2 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _CrossRoadNormalScale );
			float2 break8_g2 = temp_output_4_0_g2;
			float dotResult5_g2 = dot( temp_output_4_0_g2 , temp_output_4_0_g2 );
			float temp_output_9_0_g2 = sqrt( ( 1.0 - saturate( dotResult5_g2 ) ) );
			float3 appendResult20_g2 = (half3(break8_g2.x , break8_g2.y , temp_output_9_0_g2));
			float3 temp_output_863_0 = appendResult20_g2;
			float3 lerpResult647 = lerp( temp_output_863_0 , BlendNormals( temp_output_863_0 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ), _Float0 ) ) , tex2DNode481.a);
			float4 break666 = ( i.vertexColor / float4( 1,1,1,1 ) );
			float4 appendResult665 = (half4(( 1.0 - break666.r ) , ( 1.0 - break666.g ) , break666.b , break666.a));
			float4 clampResult672 = clamp( appendResult665 , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float3 lerpResult640 = lerp( lerpResult479 , lerpResult647 , ( 1.0 - clampResult672 ).y);
			o.Normal = lerpResult640;
			float4 texArray852 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadAlbedo_T, float3(Offset795, MainRoadIndex)  );
			float4 temp_output_77_0 = ( ( _MainRoadBrightness * texArray852 ) * _MainRoadColor );
			half4 tex2DNode486 = tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap );
			half4 blendOpSrc474 = temp_output_77_0;
			half4 blendOpDest474 = ( _DetailAlbedoPower * tex2DNode486 );
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , ( _DetailAlbedoPower * tex2DNode481.a ));
			float4 texArray861 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadAlbedo_T, float3(Offset838, _Float2)  );
			float4 temp_output_654_0 = ( ( _CrossRoadBrightness * texArray861 ) * _CrossRoadColor );
			half4 blendOpSrc652 = temp_output_654_0;
			half4 blendOpDest652 = tex2DNode486;
			float4 lerpResult653 = lerp( temp_output_654_0 , (( blendOpDest652 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest652 - 0.5 ) ) * ( 1.0 - blendOpSrc652 ) ) : ( 2.0 * blendOpDest652 * blendOpSrc652 ) ) , ( tex2DNode481.a * _Float3 ));
			float4 lerpResult644 = lerp( lerpResult480 , lerpResult653 , ( 1.0 - clampResult672 ).y);
			o.Albedo = lerpResult644.rgb;
			float4 texArray855 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset795, MainRoadIndex)  );
			float4 texArray864 = UNITY_SAMPLE_TEX2DARRAY(_ArrayMainRoadMetallicRAmbientOcclusionGHeightBSmoothnessA, float3(Offset838, _Float2)  );
			float lerpResult643 = lerp( ( texArray855.r * _MainRoadMetalicPower ) , ( _CrossRoadMetallicPower * texArray864.r ) , ( 1.0 - clampResult672 ).y);
			o.Metallic = lerpResult643;
			float lerpResult645 = lerp( ( texArray855.a * _MainRoadSmoothnessPower ) , ( texArray864.a * _CrossRoadSmoothnessPower ) , ( 1.0 - clampResult672 ).y);
			o.Smoothness = lerpResult645;
			float clampResult96 = clamp( texArray855.g , ( 1.0 - _MainRoadAmbientOcclusionPower ) , 1.0 );
			float clampResult662 = clamp( texArray864.g , ( 1.0 - _CrossRoadAmbientOcclusionPower ) , 1.0 );
			float lerpResult642 = lerp( clampResult96 , clampResult662 , ( 1.0 - clampResult672 ).y);
			o.Occlusion = lerpResult642;
			o.Alpha = 1;
			float temp_output_629_0 = ( texArray852.a * _MainRoadAlphaCutOut );
			#ifdef _IGNORECROSSROADALPHA_ON
				float staticSwitch696 = temp_output_629_0;
			#else
				float staticSwitch696 = ( texArray861.a * _CrossRoadAlphaCutOut );
			#endif
			float lerpResult641 = lerp( temp_output_629_0 , staticSwitch696 , ( 1.0 - clampResult672 ).y);
			clip( lerpResult641 - _Cutoff );
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
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				half4 color : COLOR0;
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
				o.customPack1.zw = customInputData.uv3_texcoord3;
				o.customPack1.zw = v.texcoord2;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				surfIN.uv3_texcoord3 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
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