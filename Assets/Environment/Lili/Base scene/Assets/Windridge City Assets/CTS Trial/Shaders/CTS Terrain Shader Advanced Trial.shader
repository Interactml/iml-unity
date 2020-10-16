Shader "CTS/CTS Terrain Shader Advanced Trial"
{
	Properties
	{
		_Geological_Tiling_Close("Geological_Tiling_Close", Range( 0 , 1000)) = 87
		_Geological_Tiling_Far("Geological_Tiling_Far", Range( 0 , 1000)) = 87
		_Geological_Map_Offset_Far("Geological_Map_Offset _Far", Range( 0 , 1)) = 1
		_Geological_Map_Offset_Close("Geological_Map_Offset _Close", Range( 0 , 1)) = 1
		_Geological_Map_Close_Power("Geological_Map_Close_Power", Range( 0 , 2)) = 0
		_Geological_Map_Far_Power("Geological_Map_Far_Power", Range( 0 , 2)) = 1
		_UV_Mix_Power("UV_Mix_Power", Range( 0.01 , 10)) = 4
		_UV_Mix_Start_Distance("UV_Mix_Start_Distance", Range( 0 , 100000)) = 400
		_Perlin_Normal_Tiling_Close("Perlin_Normal_Tiling_Close", Range( 0.01 , 1000)) = 40
		_Perlin_Normal_Tiling_Far("Perlin_Normal_Tiling_Far", Range( 0.01 , 1000)) = 40
		_Perlin_Normal_Power("Perlin_Normal_Power", Range( 0 , 2)) = 0
		_Perlin_Normal_Power_Close("Perlin_Normal_Power_Close", Range( 0 , 1)) = 0.5
		_Terrain_Smoothness("Terrain_Smoothness", Range( 0 , 2)) = 1
		_Terrain_Specular("Terrain_Specular", Range( 0 , 3)) = 1
		_Texture_1_Tiling("Texture_1_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_2_Tiling("Texture_2_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_3_Tiling("Texture_3_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_4_Tiling("Texture_4_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_5_Tiling("Texture_5_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_6_Tiling("Texture_6_Tiling", Range( 0.0001 , 100)) = 15
		_Texture_1_Far_Multiplier("Texture_1_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_2_Far_Multiplier("Texture_2_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_3_Far_Multiplier("Texture_3_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_4_Far_Multiplier("Texture_4_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_5_Far_Multiplier("Texture_5_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_6_Far_Multiplier("Texture_6_Far_Multiplier", Range( 0 , 10)) = 3
		_Texture_1_Perlin_Power("Texture_1_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_2_Perlin_Power("Texture_2_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_3_Perlin_Power("Texture_3_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_4_Perlin_Power("Texture_4_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_5_Perlin_Power("Texture_5_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_6_Perlin_Power("Texture_6_Perlin_Power", Range( 0 , 2)) = 0
		_Texture_1_Heightmap_Depth("Texture_1_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_6_Height_Contrast("Texture_6_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_3_Height_Contrast("Texture_3_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_5_Height_Contrast("Texture_5_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_4_Height_Contrast("Texture_4_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_1_Height_Contrast("Texture_1_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_2_Height_Contrast("Texture_2_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_2_Heightmap_Depth("Texture_2_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_3_Heightmap_Depth("Texture_3_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_4_Heightmap_Depth("Texture_4_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_5_Heightmap_Depth("Texture_5_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_6_Heightmap_Depth("Texture_6_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_3_Heightblend_Far("Texture_3_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_1_Heightblend_Far("Texture_1_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_4_Heightblend_Far("Texture_4_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_2_Heightblend_Far("Texture_2_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_6_Heightblend_Far("Texture_6_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_5_Heightblend_Far("Texture_5_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_6_Heightblend_Close("Texture_6_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_3_Heightblend_Close("Texture_3_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_5_Heightblend_Close("Texture_5_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_2_Heightblend_Close("Texture_2_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_1_Heightblend_Close("Texture_1_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_4_Heightblend_Close("Texture_4_Heightblend_Close", Range( 1 , 10)) = 5
		_Texture_1_Geological_Power("Texture_1_Geological_Power", Range( 0 , 2)) = 1
		_Texture_2_Geological_Power("Texture_2_Geological_Power", Range( 0 , 2)) = 1
		_Texture_3_Geological_Power("Texture_3_Geological_Power", Range( 0 , 2)) = 1
		_Texture_4_Geological_Power("Texture_4_Geological_Power", Range( 0 , 2)) = 1
		_Texture_5_Geological_Power("Texture_5_Geological_Power", Range( 0 , 2)) = 1
		_Texture_6_Geological_Power("Texture_6_Geological_Power", Range( 0 , 2)) = 1
		_Texture_Array_Normal("Texture_Array_Normal", 2DArray ) = "" {}
		_Texture_4_Color("Texture_4_Color", Vector) = (1,1,1,1)
		_Texture_6_Color("Texture_6_Color", Vector) = (1,1,1,1)
		_Texture_5_Color("Texture_5_Color", Vector) = (1,1,1,1)
		_Texture_3_Color("Texture_3_Color", Vector) = (1,1,1,1)
		_Texture_1_Color("Texture_1_Color", Vector) = (1,1,1,1)
		_Texture_2_Color("Texture_2_Color", Vector) = (1,1,1,1)
		_Texture_6_Triplanar("Texture_6_Triplanar", Range( 0 , 1)) = 0
		_Texture_Geological_Map("Texture_Geological_Map", 2D) = "white" {}
		_Texture_1_Albedo_Index("Texture_1_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_2_Albedo_Index("Texture_2_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_3_Normal_Index("Texture_3_Normal_Index", Range( -1 , 100)) = -1
		_Texture_1_H_AO_Index("Texture_1_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_5_Normal_Index("Texture_5_Normal_Index", Range( -1 , 100)) = -1
		_Texture_6_Albedo_Index("Texture_6_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_6_H_AO_Index("Texture_6_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_6_Normal_Index("Texture_6_Normal_Index", Range( -1 , 100)) = -1
		_Texture_5_H_AO_Index("Texture_5_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_4_H_AO_Index("Texture_4_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_4_Normal_Index("Texture_4_Normal_Index", Range( -1 , 100)) = -1
		_Texture_5_Albedo_Index("Texture_5_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_4_Albedo_Index("Texture_4_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_3_H_AO_Index("Texture_3_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_3_Albedo_Index("Texture_3_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_2_H_AO_Index("Texture_2_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_2_Normal_Index("Texture_2_Normal_Index", Range( -1 , 100)) = -1
		_Texture_1_Normal_Index("Texture_1_Normal_Index", Range( -1 , 100)) = -1
		_Texture_1_Normal_Power("Texture_1_Normal_Power", Range( 0 , 2)) = 1
		_Texture_2_Normal_Power("Texture_2_Normal_Power", Range( 0 , 2)) = 1
		_Texture_3_Normal_Power("Texture_3_Normal_Power", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_Texture_4_Normal_Power("Texture_4_Normal_Power", Range( 0 , 2)) = 1
		_Texture_5_Normal_Power("Texture_5_Normal_Power", Range( 0 , 2)) = 1
		_Texture_6_Normal_Power("Texture_6_Normal_Power", Range( 0 , 2)) = 1
		_Texture_Splat_1("Texture_Splat_1", 2D) = "black" {}
		_Texture_Splat_2("Texture_Splat_2", 2D) = "black" {}
		_Texture_Perlin_Normal_Index("Texture_Perlin_Normal_Index", Range( -1 , 100)) = -1
		_Texture_Array_Albedo("Texture_Array_Albedo", 2DArray ) = "" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+100" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
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
			float3 worldPos;
			half2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform UNITY_DECLARE_TEX2DARRAY( _Texture_Array_Normal );
		uniform half _Perlin_Normal_Tiling_Close;
		uniform half _Texture_Perlin_Normal_Index;
		uniform half _Perlin_Normal_Power_Close;
		uniform half _Perlin_Normal_Tiling_Far;
		uniform half _Perlin_Normal_Power;
		uniform half _UV_Mix_Start_Distance;
		uniform half _UV_Mix_Power;
		uniform half _Texture_6_Perlin_Power;
		uniform sampler2D _Texture_Splat_2;
		uniform half _Texture_5_Perlin_Power;
		uniform half _Texture_1_Perlin_Power;
		uniform sampler2D _Texture_Splat_1;
		uniform half _Texture_2_Perlin_Power;
		uniform half _Texture_4_Perlin_Power;
		uniform half _Texture_3_Perlin_Power;
		uniform half _Texture_5_Heightmap_Depth;
		uniform half _Texture_5_H_AO_Index;
		uniform UNITY_DECLARE_TEX2DARRAY( _Texture_Array_Albedo );
		uniform half _Texture_5_Tiling;
		uniform half _Texture_5_Far_Multiplier;
		uniform half _Texture_5_Height_Contrast;
		uniform half _Texture_5_Heightblend_Close;
		uniform half _Texture_5_Heightblend_Far;
		uniform half _Texture_6_Heightmap_Depth;
		uniform half _Texture_6_H_AO_Index;
		uniform half _Texture_6_Triplanar;
		uniform half _Texture_6_Tiling;
		uniform half _Texture_6_Far_Multiplier;
		uniform half _Texture_6_Height_Contrast;
		uniform half _Texture_6_Heightblend_Close;
		uniform half _Texture_6_Heightblend_Far;
		uniform half _Texture_1_H_AO_Index;
		uniform half _Texture_1_Tiling;
		uniform half _Texture_1_Far_Multiplier;
		uniform half _Texture_1_Height_Contrast;
		uniform half _Texture_1_Heightmap_Depth;
		uniform half _Texture_1_Heightblend_Close;
		uniform half _Texture_1_Heightblend_Far;
		uniform half _Texture_2_Heightmap_Depth;
		uniform half _Texture_2_H_AO_Index;
		uniform half _Texture_2_Tiling;
		uniform half _Texture_2_Far_Multiplier;
		uniform half _Texture_2_Height_Contrast;
		uniform half _Texture_2_Heightblend_Close;
		uniform half _Texture_2_Heightblend_Far;
		uniform half _Texture_3_Heightmap_Depth;
		uniform half _Texture_3_H_AO_Index;
		uniform half _Texture_3_Tiling;
		uniform half _Texture_3_Far_Multiplier;
		uniform half _Texture_3_Height_Contrast;
		uniform half _Texture_3_Heightblend_Close;
		uniform half _Texture_3_Heightblend_Far;
		uniform half _Texture_4_Heightmap_Depth;
		uniform half _Texture_4_H_AO_Index;
		uniform half _Texture_4_Tiling;
		uniform half _Texture_4_Far_Multiplier;
		uniform half _Texture_4_Height_Contrast;
		uniform half _Texture_4_Heightblend_Close;
		uniform half _Texture_4_Heightblend_Far;
		uniform half _Texture_1_Normal_Index;
		uniform half _Texture_1_Normal_Power;
		uniform half _Texture_2_Normal_Index;
		uniform half _Texture_2_Normal_Power;
		uniform half _Texture_3_Normal_Power;
		uniform half _Texture_3_Normal_Index;
		uniform half _Texture_4_Normal_Power;
		uniform half _Texture_4_Normal_Index;
		uniform half _Texture_5_Normal_Power;
		uniform half _Texture_5_Normal_Index;
		uniform half _Texture_6_Normal_Power;
		uniform half _Texture_6_Normal_Index;
		uniform half _Texture_1_Albedo_Index;
		uniform half4 _Texture_1_Color;
		uniform half _Texture_2_Albedo_Index;
		uniform half4 _Texture_2_Color;
		uniform half _Texture_3_Albedo_Index;
		uniform half4 _Texture_3_Color;
		uniform half _Texture_4_Albedo_Index;
		uniform half4 _Texture_4_Color;
		uniform half _Texture_5_Albedo_Index;
		uniform half4 _Texture_5_Color;
		uniform half _Texture_6_Albedo_Index;
		uniform half4 _Texture_6_Color;
		uniform half _Geological_Map_Offset_Close;
		uniform half _Geological_Tiling_Close;
		uniform half _Geological_Map_Close_Power;
		uniform sampler2D _Texture_Geological_Map;
		uniform half _Geological_Tiling_Far;
		uniform half _Geological_Map_Offset_Far;
		uniform half _Geological_Map_Far_Power;
		uniform half _Texture_6_Geological_Power;
		uniform half _Texture_5_Geological_Power;
		uniform half _Texture_1_Geological_Power;
		uniform half _Texture_2_Geological_Power;
		uniform half _Texture_4_Geological_Power;
		uniform half _Texture_3_Geological_Power;
		uniform half _Terrain_Specular;
		uniform half _Terrain_Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float localMyCustomExpression7999 = ( 0.0 );
			v.tangent.xyz = cross ( v.normal, float3( 0, 0, 1 ) );
			v.tangent.w = -1;
			half3 temp_cast_0 = (localMyCustomExpression7999).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 break8106 = ase_worldPos;
			float2 appendResult8002 = (half2(break8106.x , break8106.z));
			half2 Top_Bottom1999 = appendResult8002;
			float4 texArray6256 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(( Top_Bottom1999 / _Perlin_Normal_Tiling_Close ), _Texture_Perlin_Normal_Index)  );
			float2 appendResult11_g1308 = (half2(texArray6256.w , texArray6256.y));
			float2 temp_output_4_0_g1308 = ( ( ( appendResult11_g1308 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Perlin_Normal_Power_Close );
			float2 break8_g1308 = temp_output_4_0_g1308;
			float dotResult5_g1308 = dot( temp_output_4_0_g1308 , temp_output_4_0_g1308 );
			float temp_output_9_0_g1308 = sqrt( ( 1.0 - saturate( dotResult5_g1308 ) ) );
			float3 appendResult20_g1308 = (half3(break8_g1308.x , break8_g1308.y , temp_output_9_0_g1308));
			float4 texArray4374 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(( Top_Bottom1999 / _Perlin_Normal_Tiling_Far ), _Texture_Perlin_Normal_Index)  );
			float2 appendResult11_g1309 = (half2(texArray4374.w , texArray4374.y));
			float2 temp_output_4_0_g1309 = ( ( ( appendResult11_g1309 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Perlin_Normal_Power );
			float2 break8_g1309 = temp_output_4_0_g1309;
			float dotResult5_g1309 = dot( temp_output_4_0_g1309 , temp_output_4_0_g1309 );
			float temp_output_9_0_g1309 = sqrt( ( 1.0 - saturate( dotResult5_g1309 ) ) );
			float3 appendResult20_g1309 = (half3(break8_g1309.x , break8_g1309.y , temp_output_9_0_g1309));
			float3 break7977 = abs( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float clampResult297 = clamp( pow( ( max( max( break7977.x , break7977.y ) , break7977.z ) / _UV_Mix_Start_Distance ) , _UV_Mix_Power ) , 0.0 , 1.0 );
			half UVmixDistance636 = clampResult297;
			float3 lerpResult6257 = lerp( appendResult20_g1308 , appendResult20_g1309 , UVmixDistance636);
			half4 tex2DNode4369 = tex2D( _Texture_Splat_2, i.uv_texcoord );
			half Splat2_G2107 = tex2DNode4369.g;
			half Splat2_R2106 = tex2DNode4369.r;
			half4 tex2DNode4368 = tex2D( _Texture_Splat_1, i.uv_texcoord );
			half Splat1_R1438 = tex2DNode4368.r;
			half Splat1_G1441 = tex2DNode4368.g;
			half Splat1_A1491 = tex2DNode4368.a;
			half Splat1_B1442 = tex2DNode4368.b;
			float clampResult3775 = clamp( ( ( _Texture_6_Perlin_Power * Splat2_G2107 ) + ( ( _Texture_5_Perlin_Power * Splat2_R2106 ) + ( ( _Texture_1_Perlin_Power * Splat1_R1438 ) + ( ( _Texture_2_Perlin_Power * Splat1_G1441 ) + ( ( _Texture_4_Perlin_Power * Splat1_A1491 ) + ( _Texture_3_Perlin_Power * Splat1_B1442 ) ) ) ) ) ) , 0.0 , 1.0 );
			float3 lerpResult3776 = lerp( float3( 0,0,1 ) , lerpResult6257 , clampResult3775);
			float temp_output_4397_0 = ( 1.0 / _Texture_5_Tiling );
			float2 appendResult4399 = (half2(temp_output_4397_0 , temp_output_4397_0));
			float2 temp_output_4416_0 = ( Top_Bottom1999 * appendResult4399 );
			float4 texArray7334 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4416_0, _Texture_5_H_AO_Index)  );
			float2 temp_output_4440_0 = ( temp_output_4416_0 / _Texture_5_Far_Multiplier );
			float4 texArray5655 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4440_0, _Texture_5_H_AO_Index)  );
			float lerpResult6634 = lerp( texArray7334.y , texArray5655.y , UVmixDistance636);
			half ifLocalVar7742 = 0;
			UNITY_BRANCH 
			if( _Texture_5_H_AO_Index > -1.0 )
				ifLocalVar7742 = lerpResult6634;
			half Texture_5_H5671 = ifLocalVar7742;
			float lerpResult7226 = lerp( _Texture_5_Heightblend_Close , _Texture_5_Heightblend_Far , UVmixDistance636);
			float HeightMask6205 = saturate(pow(((( _Texture_5_Heightmap_Depth * pow( Texture_5_H5671 , _Texture_5_Height_Contrast ) )*Splat2_R2106)*4)+(Splat2_R2106*2),lerpResult7226));
			float temp_output_4469_0 = ( 1.0 / _Texture_6_Tiling );
			float2 appendResult4471 = (half2(temp_output_4469_0 , temp_output_4469_0));
			float2 temp_output_4485_0 = ( Top_Bottom1999 * appendResult4471 );
			float4 texArray7346 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4485_0, _Texture_6_H_AO_Index)  );
			float2 temp_output_4507_0 = ( temp_output_4485_0 / _Texture_6_Far_Multiplier );
			float4 texArray5695 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4507_0, _Texture_6_H_AO_Index)  );
			float lerpResult6640 = lerp( texArray7346.y , texArray5695.y , UVmixDistance636);
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 clampResult6387 = clamp( pow( ( ase_worldNormal * ase_worldNormal ) , 25.0 ) , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			half3 BlendComponents91 = clampResult6387;
			float2 appendResult8003 = (half2(break8106.z , break8106.y));
			half2 Front_Back1991 = appendResult8003;
			float2 temp_output_4472_0 = ( Front_Back1991 * appendResult4471 );
			float4 texArray7347 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4472_0, _Texture_6_H_AO_Index)  );
			float2 appendResult8001 = (half2(break8106.x , break8106.y));
			half2 Left_Right2003 = appendResult8001;
			float2 temp_output_4483_0 = ( Left_Right2003 * appendResult4471 );
			float4 texArray7341 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4483_0, _Texture_6_H_AO_Index)  );
			float3 weightedBlendVar7344 = BlendComponents91;
			float weightedAvg7344 = ( ( weightedBlendVar7344.x*texArray7347.y + weightedBlendVar7344.y*texArray7346.y + weightedBlendVar7344.z*texArray7341.y )/( weightedBlendVar7344.x + weightedBlendVar7344.y + weightedBlendVar7344.z ) );
			float2 temp_output_4503_0 = ( temp_output_4472_0 / _Texture_6_Far_Multiplier );
			float4 texArray5676 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4503_0, _Texture_6_H_AO_Index)  );
			float2 temp_output_4504_0 = ( temp_output_4483_0 / _Texture_6_Far_Multiplier );
			float4 texArray5684 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4504_0, _Texture_6_H_AO_Index)  );
			float3 weightedBlendVar6428 = BlendComponents91;
			float weightedAvg6428 = ( ( weightedBlendVar6428.x*texArray5676.y + weightedBlendVar6428.y*texArray5695.y + weightedBlendVar6428.z*texArray5684.y )/( weightedBlendVar6428.x + weightedBlendVar6428.y + weightedBlendVar6428.z ) );
			float lerpResult5709 = lerp( weightedAvg7344 , weightedAvg6428 , UVmixDistance636);
			half ifLocalVar6638 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Triplanar == 1.0 )
				ifLocalVar6638 = lerpResult5709;
			else
				ifLocalVar6638 = lerpResult6640;
			half ifLocalVar7746 = 0;
			UNITY_BRANCH 
			if( _Texture_6_H_AO_Index > -1.0 )
				ifLocalVar7746 = ifLocalVar6638;
			half Texture_6_H5711 = ifLocalVar7746;
			float lerpResult7230 = lerp( _Texture_6_Heightblend_Close , _Texture_6_Heightblend_Far , UVmixDistance636);
			float HeightMask6208 = saturate(pow(((( _Texture_6_Heightmap_Depth * pow( Texture_6_H5711 , _Texture_6_Height_Contrast ) )*Splat2_G2107)*4)+(Splat2_G2107*2),lerpResult7230));
			float4 appendResult6524 = (half4(HeightMask6205 , HeightMask6208 , 0.0 , 0.0));
			float temp_output_3830_0 = ( 1.0 / _Texture_1_Tiling );
			float2 appendResult3284 = (half2(temp_output_3830_0 , temp_output_3830_0));
			float2 temp_output_3275_0 = ( Top_Bottom1999 * appendResult3284 );
			float4 texArray7282 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3275_0, _Texture_1_H_AO_Index)  );
			float2 temp_output_3298_0 = ( temp_output_3275_0 / _Texture_1_Far_Multiplier );
			float4 texArray5491 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3298_0, _Texture_1_H_AO_Index)  );
			float lerpResult6611 = lerp( texArray7282.y , texArray5491.y , UVmixDistance636);
			half ifLocalVar7731 = 0;
			UNITY_BRANCH 
			if( _Texture_1_H_AO_Index > -1.0 )
				ifLocalVar7731 = lerpResult6611;
			half Texture_1_H5480 = ifLocalVar7731;
			float lerpResult7218 = lerp( _Texture_1_Heightblend_Close , _Texture_1_Heightblend_Far , UVmixDistance636);
			float HeightMask6196 = saturate(pow(((( pow( Texture_1_H5480 , _Texture_1_Height_Contrast ) * _Texture_1_Heightmap_Depth )*Splat1_R1438)*4)+(Splat1_R1438*2),lerpResult7218));
			float temp_output_3831_0 = ( 1.0 / _Texture_2_Tiling );
			float2 appendResult3349 = (half2(temp_output_3831_0 , temp_output_3831_0));
			float2 temp_output_3343_0 = ( Top_Bottom1999 * appendResult3349 );
			float4 texArray7293 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3343_0, _Texture_2_H_AO_Index)  );
			float2 temp_output_3345_0 = ( temp_output_3343_0 / _Texture_2_Far_Multiplier );
			float4 texArray5533 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3345_0, _Texture_2_H_AO_Index)  );
			float lerpResult6616 = lerp( texArray7293.y , texArray5533.y , UVmixDistance636);
			half ifLocalVar7734 = 0;
			UNITY_BRANCH 
			if( _Texture_2_H_AO_Index > -1.0 )
				ifLocalVar7734 = lerpResult6616;
			half Texture_2_H5497 = ifLocalVar7734;
			float lerpResult7222 = lerp( _Texture_2_Heightblend_Close , _Texture_2_Heightblend_Far , UVmixDistance636);
			float HeightMask6515 = saturate(pow(((( _Texture_2_Heightmap_Depth * pow( Texture_2_H5497 , _Texture_2_Height_Contrast ) )*Splat1_G1441)*4)+(Splat1_G1441*2),lerpResult7222));
			float temp_output_3832_0 = ( 1.0 / _Texture_3_Tiling );
			float2 appendResult3415 = (half2(temp_output_3832_0 , temp_output_3832_0));
			float2 temp_output_3410_0 = ( Top_Bottom1999 * appendResult3415 );
			float4 texArray7310 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3410_0, _Texture_3_H_AO_Index)  );
			float2 temp_output_3412_0 = ( temp_output_3410_0 / _Texture_3_Far_Multiplier );
			float4 texArray5586 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3412_0, _Texture_3_H_AO_Index)  );
			float lerpResult6622 = lerp( texArray7310.y , texArray5586.y , UVmixDistance636);
			half ifLocalVar7736 = 0;
			UNITY_BRANCH 
			if( _Texture_3_H_AO_Index > -1.0 )
				ifLocalVar7736 = lerpResult6622;
			half Texture_3_H5581 = ifLocalVar7736;
			float lerpResult7214 = lerp( _Texture_3_Heightblend_Close , _Texture_3_Heightblend_Far , UVmixDistance636);
			float HeightMask6516 = saturate(pow(((( _Texture_3_Heightmap_Depth * pow( Texture_3_H5581 , _Texture_3_Height_Contrast ) )*Splat1_B1442)*4)+(Splat1_B1442*2),lerpResult7214));
			float temp_output_3833_0 = ( 1.0 / _Texture_4_Tiling );
			float2 appendResult3482 = (half2(temp_output_3833_0 , temp_output_3833_0));
			float2 temp_output_3477_0 = ( Top_Bottom1999 * appendResult3482 );
			float4 texArray7322 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3477_0, _Texture_4_H_AO_Index)  );
			float2 temp_output_3479_0 = ( temp_output_3477_0 / _Texture_4_Far_Multiplier );
			float4 texArray5615 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3479_0, _Texture_4_H_AO_Index)  );
			float lerpResult6628 = lerp( texArray7322.y , texArray5615.y , UVmixDistance636);
			half ifLocalVar7738 = 0;
			UNITY_BRANCH 
			if( _Texture_4_H_AO_Index > -1.0 )
				ifLocalVar7738 = lerpResult6628;
			half Texture_4_H5631 = ifLocalVar7738;
			float lerpResult7211 = lerp( _Texture_4_Heightblend_Close , _Texture_4_Heightblend_Far , UVmixDistance636);
			float HeightMask6203 = saturate(pow(((( _Texture_4_Heightmap_Depth * pow( Texture_4_H5631 , _Texture_4_Height_Contrast ) )*Splat1_A1491)*4)+(Splat1_A1491*2),lerpResult7211));
			float4 appendResult6517 = (half4(HeightMask6196 , HeightMask6515 , HeightMask6516 , HeightMask6203));
			float4 texArray3299 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_3275_0, _Texture_1_Normal_Index)  );
			float2 appendResult11_g1304 = (half2(texArray3299.w , texArray3299.y));
			float2 temp_output_4_0_g1304 = ( ( ( appendResult11_g1304 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_1_Normal_Power );
			float2 break8_g1304 = temp_output_4_0_g1304;
			float dotResult5_g1304 = dot( temp_output_4_0_g1304 , temp_output_4_0_g1304 );
			float temp_output_9_0_g1304 = sqrt( ( 1.0 - saturate( dotResult5_g1304 ) ) );
			float3 appendResult20_g1304 = (half3(break8_g1304.x , break8_g1304.y , temp_output_9_0_g1304));
			half3 EmptyNRM7781 = half3(0,0,1);
			half3 ifLocalVar7594 = 0;
			UNITY_BRANCH 
			if( _Texture_1_Normal_Index <= -1.0 )
				ifLocalVar7594 = EmptyNRM7781;
			else
				ifLocalVar7594 = appendResult20_g1304;
			half3 Normal_1569 = ifLocalVar7594;
			float4 texArray3350 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_3343_0, _Texture_2_Normal_Index)  );
			float2 appendResult11_g1303 = (half2(texArray3350.w , texArray3350.y));
			float2 temp_output_4_0_g1303 = ( ( ( appendResult11_g1303 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_2_Normal_Power );
			float2 break8_g1303 = temp_output_4_0_g1303;
			float dotResult5_g1303 = dot( temp_output_4_0_g1303 , temp_output_4_0_g1303 );
			float temp_output_9_0_g1303 = sqrt( ( 1.0 - saturate( dotResult5_g1303 ) ) );
			float3 appendResult20_g1303 = (half3(break8_g1303.x , break8_g1303.y , temp_output_9_0_g1303));
			half3 ifLocalVar7600 = 0;
			UNITY_BRANCH 
			if( _Texture_2_Normal_Index <= -1.0 )
				ifLocalVar7600 = EmptyNRM7781;
			else
				ifLocalVar7600 = appendResult20_g1303;
			half3 Normal_23361 = ifLocalVar7600;
			float4 texArray3416 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_3410_0, _Texture_3_Normal_Index)  );
			float2 appendResult11_g1305 = (half2(texArray3416.w , texArray3416.y));
			float2 temp_output_4_0_g1305 = ( ( ( appendResult11_g1305 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_3_Normal_Power );
			float2 break8_g1305 = temp_output_4_0_g1305;
			float dotResult5_g1305 = dot( temp_output_4_0_g1305 , temp_output_4_0_g1305 );
			float temp_output_9_0_g1305 = sqrt( ( 1.0 - saturate( dotResult5_g1305 ) ) );
			float3 appendResult20_g1305 = (half3(break8_g1305.x , break8_g1305.y , temp_output_9_0_g1305));
			half3 ifLocalVar7604 = 0;
			UNITY_BRANCH 
			if( _Texture_3_Normal_Power <= -1.0 )
				ifLocalVar7604 = EmptyNRM7781;
			else
				ifLocalVar7604 = appendResult20_g1305;
			half3 Normal_33452 = ifLocalVar7604;
			float4 texArray3483 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_3477_0, _Texture_4_Normal_Index)  );
			float2 appendResult11_g1306 = (half2(texArray3483.w , texArray3483.y));
			float2 temp_output_4_0_g1306 = ( ( ( appendResult11_g1306 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_4_Normal_Power );
			float2 break8_g1306 = temp_output_4_0_g1306;
			float dotResult5_g1306 = dot( temp_output_4_0_g1306 , temp_output_4_0_g1306 );
			float temp_output_9_0_g1306 = sqrt( ( 1.0 - saturate( dotResult5_g1306 ) ) );
			float3 appendResult20_g1306 = (half3(break8_g1306.x , break8_g1306.y , temp_output_9_0_g1306));
			half3 ifLocalVar7610 = 0;
			UNITY_BRANCH 
			if( _Texture_4_Normal_Power <= -1.0 )
				ifLocalVar7610 = EmptyNRM7781;
			else
				ifLocalVar7610 = appendResult20_g1306;
			half3 Normal_43519 = ifLocalVar7610;
			float4 layeredBlendVar7722 = appendResult6517;
			float3 layeredBlend7722 = ( lerp( lerp( lerp( lerp( float3( 0,0,0 ) , Normal_1569 , layeredBlendVar7722.x ) , Normal_23361 , layeredBlendVar7722.y ) , Normal_33452 , layeredBlendVar7722.z ) , Normal_43519 , layeredBlendVar7722.w ) );
			float4 texArray4424 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_4416_0, _Texture_5_Normal_Index)  );
			float2 appendResult11_g1307 = (half2(texArray4424.w , texArray4424.y));
			float2 temp_output_4_0_g1307 = ( ( ( appendResult11_g1307 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_5_Normal_Power );
			float2 break8_g1307 = temp_output_4_0_g1307;
			float dotResult5_g1307 = dot( temp_output_4_0_g1307 , temp_output_4_0_g1307 );
			float temp_output_9_0_g1307 = sqrt( ( 1.0 - saturate( dotResult5_g1307 ) ) );
			float3 appendResult20_g1307 = (half3(break8_g1307.x , break8_g1307.y , temp_output_9_0_g1307));
			half3 ifLocalVar7614 = 0;
			UNITY_BRANCH 
			if( _Texture_5_Normal_Power <= -1.0 )
				ifLocalVar7614 = EmptyNRM7781;
			else
				ifLocalVar7614 = appendResult20_g1307;
			half3 Normal_54456 = ifLocalVar7614;
			float4 texArray4493 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_4485_0, _Texture_6_Normal_Index)  );
			float2 appendResult11_g1298 = (half2(texArray4493.w , texArray4493.y));
			float2 temp_output_4_0_g1298 = ( ( ( appendResult11_g1298 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_6_Normal_Power );
			float2 break8_g1298 = temp_output_4_0_g1298;
			float dotResult5_g1298 = dot( temp_output_4_0_g1298 , temp_output_4_0_g1298 );
			float temp_output_9_0_g1298 = sqrt( ( 1.0 - saturate( dotResult5_g1298 ) ) );
			float3 appendResult20_g1298 = (half3(break8_g1298.x , break8_g1298.y , temp_output_9_0_g1298));
			float3 temp_output_7004_0 = appendResult20_g1298;
			float4 texArray4486 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_4472_0, _Texture_6_Normal_Index)  );
			float2 appendResult11_g1295 = (half2(texArray4486.w , texArray4486.y));
			float2 temp_output_4_0_g1295 = ( ( ( appendResult11_g1295 * float2( 2,2 ) ) + float2( -1,-1 ) ) * ( _Texture_6_Normal_Power * -1.0 ) );
			float2 break8_g1295 = temp_output_4_0_g1295;
			float dotResult5_g1295 = dot( temp_output_4_0_g1295 , temp_output_4_0_g1295 );
			float temp_output_9_0_g1295 = sqrt( ( 1.0 - saturate( dotResult5_g1295 ) ) );
			float3 appendResult21_g1295 = (half3(break8_g1295.y , break8_g1295.x , temp_output_9_0_g1295));
			float3 appendResult6892 = (half3(ase_worldNormal.x , -1.0 , 1.0));
			float4 texArray4491 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(temp_output_4483_0, _Texture_6_Normal_Index)  );
			float2 appendResult11_g1294 = (half2(texArray4491.w , texArray4491.y));
			float2 temp_output_4_0_g1294 = ( ( ( appendResult11_g1294 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Texture_6_Normal_Power );
			float2 break8_g1294 = temp_output_4_0_g1294;
			float dotResult5_g1294 = dot( temp_output_4_0_g1294 , temp_output_4_0_g1294 );
			float temp_output_9_0_g1294 = sqrt( ( 1.0 - saturate( dotResult5_g1294 ) ) );
			float3 appendResult20_g1294 = (half3(break8_g1294.x , break8_g1294.y , temp_output_9_0_g1294));
			float3 appendResult6895 = (half3(1.0 , ( ase_worldNormal.z * -1.0 ) , 1.0));
			float3 weightedBlendVar6429 = BlendComponents91;
			float3 weightedAvg6429 = ( ( weightedBlendVar6429.x*( appendResult21_g1295 * appendResult6892 ) + weightedBlendVar6429.y*temp_output_7004_0 + weightedBlendVar6429.z*( appendResult20_g1294 * appendResult6895 ) )/( weightedBlendVar6429.x + weightedBlendVar6429.y + weightedBlendVar6429.z ) );
			half3 ifLocalVar6637 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Triplanar == 1.0 )
				ifLocalVar6637 = weightedAvg6429;
			else
				ifLocalVar6637 = temp_output_7004_0;
			half3 ifLocalVar7618 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Normal_Power <= -1.0 )
				ifLocalVar7618 = EmptyNRM7781;
			else
				ifLocalVar7618 = ifLocalVar6637;
			half3 Normal_64537 = ifLocalVar7618;
			half3 _Vector1 = half3(0,0,1);
			float4 layeredBlendVar7724 = appendResult6524;
			float3 layeredBlend7724 = ( lerp( lerp( lerp( lerp( layeredBlend7722 , Normal_54456 , layeredBlendVar7724.x ) , Normal_64537 , layeredBlendVar7724.y ) , _Vector1 , layeredBlendVar7724.z ) , _Vector1 , layeredBlendVar7724.w ) );
			float3 normalizeResult3900 = normalize( layeredBlend7724 );
			float3 normalizeResult3901 = normalize( normalizeResult3900 );
			o.Normal = BlendNormals( lerpResult3776 , normalizeResult3901 );
			float4 texArray3287 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3275_0, _Texture_1_Albedo_Index)  );
			float4 texArray3293 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3298_0, _Texture_1_Albedo_Index)  );
			float4 lerpResult6608 = lerp( texArray3287 , texArray3293 , UVmixDistance636);
			half4 ifLocalVar7593 = 0;
			UNITY_BRANCH 
			if( _Texture_1_Albedo_Index > -1.0 )
				ifLocalVar7593 = ( lerpResult6608 * _Texture_1_Color );
			half4 Texture_1_Final950 = ifLocalVar7593;
			float4 texArray3338 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3343_0, _Texture_2_Albedo_Index)  );
			float4 texArray3339 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3345_0, _Texture_2_Albedo_Index)  );
			float4 lerpResult6617 = lerp( texArray3338 , texArray3339 , UVmixDistance636);
			half4 ifLocalVar7599 = 0;
			UNITY_BRANCH 
			if( _Texture_2_Albedo_Index > -1.0 )
				ifLocalVar7599 = ( lerpResult6617 * _Texture_2_Color );
			half4 Texture_2_Final3385 = ifLocalVar7599;
			float4 texArray3405 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3410_0, _Texture_3_Albedo_Index)  );
			float4 texArray3406 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3412_0, _Texture_3_Albedo_Index)  );
			float4 lerpResult6623 = lerp( texArray3405 , texArray3406 , UVmixDistance636);
			half4 ifLocalVar7603 = 0;
			UNITY_BRANCH 
			if( _Texture_3_Albedo_Index > -1.0 )
				ifLocalVar7603 = ( lerpResult6623 * _Texture_3_Color );
			half4 Texture_3_Final3451 = ifLocalVar7603;
			float4 texArray3472 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3477_0, _Texture_4_Albedo_Index)  );
			float4 texArray3473 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3479_0, _Texture_4_Albedo_Index)  );
			float4 lerpResult6629 = lerp( texArray3472 , texArray3473 , UVmixDistance636);
			half4 ifLocalVar7608 = 0;
			UNITY_BRANCH 
			if( _Texture_4_Albedo_Index > -1.0 )
				ifLocalVar7608 = ( lerpResult6629 * _Texture_4_Color );
			half4 Texture_4_Final3518 = ifLocalVar7608;
			float4 layeredBlendVar6512 = appendResult6517;
			float4 layeredBlend6512 = ( lerp( lerp( lerp( lerp( float4( 0,0,0,0 ) , Texture_1_Final950 , layeredBlendVar6512.x ) , Texture_2_Final3385 , layeredBlendVar6512.y ) , Texture_3_Final3451 , layeredBlendVar6512.z ) , Texture_4_Final3518 , layeredBlendVar6512.w ) );
			float4 texArray4450 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4416_0, _Texture_5_Albedo_Index)  );
			float4 texArray4445 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4440_0, _Texture_5_Albedo_Index)  );
			float4 lerpResult6635 = lerp( texArray4450 , texArray4445 , UVmixDistance636);
			half4 ifLocalVar7613 = 0;
			UNITY_BRANCH 
			if( _Texture_5_Albedo_Index > -1.0 )
				ifLocalVar7613 = ( lerpResult6635 * _Texture_5_Color );
			half4 Texture_5_Final4396 = ifLocalVar7613;
			float4 texArray4517 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4485_0, _Texture_6_Albedo_Index)  );
			float4 texArray4512 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4507_0, _Texture_6_Albedo_Index)  );
			float4 lerpResult6641 = lerp( texArray4517 , texArray4512 , UVmixDistance636);
			float4 texArray4509 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4472_0, _Texture_6_Albedo_Index)  );
			float4 texArray4510 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4483_0, _Texture_6_Albedo_Index)  );
			float3 weightedBlendVar6424 = BlendComponents91;
			float4 weightedAvg6424 = ( ( weightedBlendVar6424.x*texArray4509 + weightedBlendVar6424.y*texArray4517 + weightedBlendVar6424.z*texArray4510 )/( weightedBlendVar6424.x + weightedBlendVar6424.y + weightedBlendVar6424.z ) );
			float4 texArray4511 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4503_0, _Texture_6_Albedo_Index)  );
			float4 texArray4506 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4504_0, _Texture_6_Albedo_Index)  );
			float3 weightedBlendVar6426 = BlendComponents91;
			float4 weightedAvg6426 = ( ( weightedBlendVar6426.x*texArray4511 + weightedBlendVar6426.y*texArray4512 + weightedBlendVar6426.z*texArray4506 )/( weightedBlendVar6426.x + weightedBlendVar6426.y + weightedBlendVar6426.z ) );
			float4 lerpResult4532 = lerp( weightedAvg6424 , weightedAvg6426 , UVmixDistance636);
			half4 ifLocalVar6636 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Triplanar == 1.0 )
				ifLocalVar6636 = lerpResult4532;
			else
				ifLocalVar6636 = lerpResult6641;
			half4 ifLocalVar7617 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Albedo_Index > -1.0 )
				ifLocalVar7617 = ( ifLocalVar6636 * _Texture_6_Color );
			half4 Texture_6_Final4536 = ifLocalVar7617;
			half4 _Vector2 = half4(0,0,0,0);
			float4 layeredBlendVar6520 = appendResult6524;
			float4 layeredBlend6520 = ( lerp( lerp( lerp( lerp( layeredBlend6512 , Texture_5_Final4396 , layeredBlendVar6520.x ) , Texture_6_Final4536 , layeredBlendVar6520.y ) , _Vector2 , layeredBlendVar6520.z ) , _Vector2 , layeredBlendVar6520.w ) );
			float4 break3856 = layeredBlend6520;
			float3 appendResult3857 = (half3(break3856.x , break3856.y , break3856.z));
			half2 temp_cast_0 = (( _Geological_Map_Offset_Close + ( ase_worldPos.y / _Geological_Tiling_Close ) )).xx;
			half4 tex2DNode6968 = tex2D( _Texture_Geological_Map, temp_cast_0 );
			float3 appendResult6970 = (half3(tex2DNode6968.r , tex2DNode6968.g , tex2DNode6968.b));
			half2 temp_cast_1 = (( ( ase_worldPos.y / _Geological_Tiling_Far ) + _Geological_Map_Offset_Far )).xx;
			half4 tex2DNode6969 = tex2D( _Texture_Geological_Map, temp_cast_1 );
			float3 appendResult6971 = (half3(tex2DNode6969.r , tex2DNode6969.g , tex2DNode6969.b));
			float3 lerpResult1315 = lerp( ( ( appendResult6970 + float3( -0.3,-0.3,-0.3 ) ) * _Geological_Map_Close_Power ) , ( ( appendResult6971 + float3( -0.3,-0.3,-0.3 ) ) * _Geological_Map_Far_Power ) , UVmixDistance636);
			half3 blendOpSrc4362 = appendResult3857;
			half3 blendOpDest4362 = ( lerpResult1315 * ( ( _Texture_6_Geological_Power * Splat2_G2107 ) + ( ( _Texture_5_Geological_Power * Splat2_R2106 ) + ( ( _Texture_1_Geological_Power * Splat1_R1438 ) + ( ( _Texture_2_Geological_Power * Splat1_G1441 ) + ( ( _Texture_4_Geological_Power * Splat1_A1491 ) + ( _Texture_3_Geological_Power * Splat1_B1442 ) ) ) ) ) ) );
			o.Albedo = ( saturate( ( blendOpSrc4362 + blendOpDest4362 ) ));
			o.Specular = ( ( appendResult3857 * float3( 0.3,0.3,0.3 ) ) * _Terrain_Specular );
			o.Smoothness = ( break3856.w * _Terrain_Smoothness );
			o.Occlusion = 1.0;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				vertexDataFunc( v, customInputData );
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
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}

	Dependency "BaseMapShader"="CTS/CTS Terrain Shader Advanced LOD Trial"
	Fallback "Diffuse"
}