Shader "NatureManufacture Shaders/Standard Metalic UV Walls and Floors"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,1,1,1)
		_AlbedoRGB("Albedo (RGB)", 2D) = "white" {}
		_Tilling("Tilling", Range( 0 , 100)) = -1.570796
		_TriplanarFalloff("Triplanar Falloff", Range( 0 , 200)) = 200
		_NormalRGB("Normal (RGB)", 2D) = "bump" {}
		_MetalicRAmbientOcclusionGSmothnessA("Metalic (R) Ambient Occlusion (G) Smothness (A)", 2D) = "white" {}
		_MetalicPower("Metalic Power", Range( 0 , 2)) = 1
		_AOPower("AO Power", Range( 0 , 1)) = 0
		_SmothnessPower("Smothness Power", Range( 0 , 2)) = 1
		_DirtAmount("Dirt Amount", Range( 0 , 10)) = 1
		_DirtAlbedoColor("Dirt Albedo Color", Color) = (1,1,1,1)
		_DirtMaskR("Dirt Mask (R)", 2D) = "white" {}
		_DirtMaskTiling("Dirt Mask Tiling", Float) = 0.1
		_DirtMaskFalloff("Dirt Mask Falloff", Range( 0 , 200)) = 200
		_DirtAlbedo("Dirt Albedo", 2D) = "white" {}
		_DirtTilling("Dirt Tilling", Range( 0 , 100)) = 0.4
		_DirtTriplanarFalloff("Dirt Triplanar Falloff", Range( 0 , 200)) = 200
		_DirtNormalRGB("Dirt Normal (RGB)", 2D) = "bump" {}
		_DirtMetalicRAmbientOcclusionGSmothnessA("Dirt Metalic (R) Ambient Occlusion (G) Smothness (A)", 2D) = "white" {}
		_DirtMetalicPower("Dirt Metalic Power", Range( 0 , 2)) = 0
		_DirtAmbientOcclusionPower("Dirt Ambient Occlusion Power", Range( 0 , 1)) = 0
		_DirtSmothnessPower("Dirt Smothness Power", Range( 0 , 2)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalRGB;
		uniform float _Tilling;
		uniform float _TriplanarFalloff;
		uniform sampler2D _DirtNormalRGB;
		uniform float _DirtTilling;
		uniform float _DirtTriplanarFalloff;
		uniform sampler2D _DirtMaskR;
		uniform float _DirtMaskTiling;
		uniform float _DirtMaskFalloff;
		uniform float _DirtAmount;
		uniform sampler2D _AlbedoRGB;
		uniform sampler2D _DirtAlbedo;
		uniform sampler2D _MetalicRAmbientOcclusionGSmothnessA;
		uniform float _MetalicPower;
		uniform sampler2D _DirtMetalicRAmbientOcclusionGSmothnessA;
		uniform float _DirtMetalicPower;
		uniform float _SmothnessPower;
		uniform float _DirtSmothnessPower;
		uniform float _AOPower;
		uniform float _DirtAmbientOcclusionPower;

		UNITY_INSTANCING_BUFFER_START(NatureManufactureShadersStandardMetalicUVWallsandFloors)
			UNITY_DEFINE_INSTANCED_PROP(float4, _AlbedoColor)
#define _AlbedoColor_arr NatureManufactureShadersStandardMetalicUVWallsandFloors
			UNITY_DEFINE_INSTANCED_PROP(float4, _DirtAlbedoColor)
#define _DirtAlbedoColor_arr NatureManufactureShadersStandardMetalicUVWallsandFloors
		UNITY_INSTANCING_BUFFER_END(NatureManufactureShadersStandardMetalicUVWallsandFloors)


		inline float3 TriplanarNormal( sampler2D topBumpMap, sampler2D midBumpMap, sampler2D botBumpMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign(worldNormal);
			half3 xNorm; half3 yNorm; half3 zNorm;
			if(vertex == 1){
			xNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zx ) );
			zNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			xNorm = normalize( half3( xNorm.xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ) );
			yNorm = normalize( half3( yNorm.xy + worldNormal.zx, worldNormal.y));
			zNorm = normalize( half3( zNorm.xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ) );
			xNorm = xNorm.zyx;
			yNorm = yNorm.yzx;
			zNorm = zNorm.xyz;
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			if(vertex == 1){
			xNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			return xNorm* projNormal.x + yNorm* projNormal.y + zNorm* projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal3591 = TriplanarNormal( _NormalRGB, _NormalRGB, _NormalRGB, ase_worldPos, ase_worldNormal, _TriplanarFalloff, _Tilling, 0 );
			float3 tanTriplanarNormal3591 = mul( ase_worldToTangent, worldTriplanarNormal3591 );
			float3 worldTriplanarNormal3600 = TriplanarNormal( _DirtNormalRGB, _DirtNormalRGB, _DirtNormalRGB, ase_worldPos, ase_worldNormal, _DirtTriplanarFalloff, _DirtTilling, 0 );
			float3 tanTriplanarNormal3600 = mul( ase_worldToTangent, worldTriplanarNormal3600 );
			float4 triplanar3606 = TriplanarSampling( _DirtMaskR, _DirtMaskR, _DirtMaskR, ase_worldPos, ase_worldNormal, _DirtMaskFalloff, _DirtMaskTiling, 0 );
			float clampResult3611 = clamp( ( triplanar3606.x * _DirtAmount ) , 0.0 , 1.0 );
			float temp_output_3321_0 = saturate( clampResult3611 );
			float3 lerpResult3318 = lerp( tanTriplanarNormal3591 , tanTriplanarNormal3600 , temp_output_3321_0);
			float3 normalizeResult3376 = normalize( lerpResult3318 );
			o.Normal = normalizeResult3376;
			float4 triplanar3590 = TriplanarSampling( _AlbedoRGB, _AlbedoRGB, _AlbedoRGB, ase_worldPos, ase_worldNormal, _TriplanarFalloff, _Tilling, 0 );
			float4 _AlbedoColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_AlbedoColor_arr, _AlbedoColor);
			float4 triplanar3604 = TriplanarSampling( _DirtAlbedo, _DirtAlbedo, _DirtAlbedo, ase_worldPos, ase_worldNormal, _DirtTriplanarFalloff, _DirtTilling, 0 );
			float4 _DirtAlbedoColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_DirtAlbedoColor_arr, _DirtAlbedoColor);
			float4 lerpResult3317 = lerp( ( triplanar3590 * _AlbedoColor_Instance ) , ( triplanar3604 * _DirtAlbedoColor_Instance ) , temp_output_3321_0);
			float4 clampResult3290 = clamp( lerpResult3317 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Albedo = clampResult3290.xyz;
			float4 triplanar3592 = TriplanarSampling( _MetalicRAmbientOcclusionGSmothnessA, _MetalicRAmbientOcclusionGSmothnessA, _MetalicRAmbientOcclusionGSmothnessA, ase_worldPos, ase_worldNormal, _TriplanarFalloff, _Tilling, 0 );
			float4 triplanar3603 = TriplanarSampling( _DirtMetalicRAmbientOcclusionGSmothnessA, _DirtMetalicRAmbientOcclusionGSmothnessA, _DirtMetalicRAmbientOcclusionGSmothnessA, ase_worldPos, ase_worldNormal, _DirtTriplanarFalloff, _DirtTilling, 0 );
			float lerpResult3332 = lerp( ( triplanar3592.x * _MetalicPower ) , ( triplanar3603.x * _DirtMetalicPower ) , temp_output_3321_0);
			o.Metallic = lerpResult3332;
			float lerpResult3345 = lerp( ( triplanar3592.w * _SmothnessPower ) , ( triplanar3603.w * _DirtSmothnessPower ) , temp_output_3321_0);
			o.Smoothness = lerpResult3345;
			float clampResult3582 = clamp( triplanar3592.y , ( 1.0 - _AOPower ) , 1.0 );
			float clampResult3589 = clamp( triplanar3603.y , ( 1.0 - _DirtAmbientOcclusionPower ) , 1.0 );
			float lerpResult3333 = lerp( clampResult3582 , clampResult3589 , temp_output_3321_0);
			o.Occlusion = lerpResult3333;
			o.Alpha = 1;
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
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
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