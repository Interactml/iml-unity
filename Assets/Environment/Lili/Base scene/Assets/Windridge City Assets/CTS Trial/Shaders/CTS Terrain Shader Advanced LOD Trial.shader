 Shader "CTS/CTS Terrain Shader Advanced LOD Trial"
{
	Properties
	{
		_Geological_Tiling_Far("Geological_Tiling_Far", Range( 0 , 1000)) = 87
		_Geological_Map_Offset_Far("Geological_Map_Offset _Far", Range( 0 , 1)) = 1
		_Geological_Map_Far_Power("Geological_Map_Far_Power", Range( 0 , 2)) = 1
		_Perlin_Normal_Tiling_Far("Perlin_Normal_Tiling_Far", Range( 0.01 , 1000)) = 40
		_Perlin_Normal_Power("Perlin_Normal_Power", Range( 0 , 2)) = 1
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
		_Texture_6_Heightmap_Depth("Texture_6_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_5_Heightmap_Depth("Texture_5_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_Array_Normal("Texture_Array_Normal", 2DArray ) = "" {}
		_Texture_3_Heightmap_Depth("Texture_3_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_4_Heightmap_Depth("Texture_4_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_2_Heightmap_Depth("Texture_2_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_1_Heightmap_Depth("Texture_1_Heightmap_Depth", Range( 0 , 10)) = 1
		_Texture_1_Height_Contrast("Texture_1_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_6_Height_Contrast("Texture_6_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_3_Height_Contrast("Texture_3_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_5_Height_Contrast("Texture_5_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_4_Height_Contrast("Texture_4_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_2_Height_Contrast("Texture_2_Height_Contrast", Range( 0 , 10)) = 1
		_Texture_6_Heightblend_Far("Texture_6_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_5_Heightblend_Far("Texture_5_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_3_Heightblend_Far("Texture_3_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_4_Heightblend_Far("Texture_4_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_2_Heightblend_Far("Texture_2_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_1_Heightblend_Far("Texture_1_Heightblend_Far", Range( 1 , 10)) = 5
		_Texture_1_Geological_Power("Texture_1_Geological_Power", Range( 0 , 2)) = 1
		_Texture_2_Geological_Power("Texture_2_Geological_Power", Range( 0 , 2)) = 1
		_Texture_3_Geological_Power("Texture_3_Geological_Power", Range( 0 , 2)) = 1
		_Texture_4_Geological_Power("Texture_4_Geological_Power", Range( 0 , 2)) = 1
		_Texture_5_Geological_Power("Texture_5_Geological_Power", Range( 0 , 2)) = 1
		_Texture_6_Geological_Power("Texture_6_Geological_Power", Range( 0 , 2)) = 1
		_Texture_Perlin_Normal_Index("Texture_Perlin_Normal_Index", Int) = -1
		_Texture_5_Color("Texture_5_Color", Vector) = (1,1,1,1)
		_Texture_6_Color("Texture_6_Color", Vector) = (1,1,1,1)
		_Texture_4_Color("Texture_4_Color", Vector) = (1,1,1,1)
		_Texture_1_Color("Texture_1_Color", Vector) = (1,1,1,1)
		_Texture_3_Color("Texture_3_Color", Vector) = (1,1,1,1)
		_Texture_2_Color("Texture_2_Color", Vector) = (1,1,1,1)
		_Texture_Geological_Map("Texture_Geological_Map", 2D) = "white" {}
		_Texture_Splat_1("Texture_Splat_1", 2D) = "black" {}
		_Texture_Splat_2("Texture_Splat_2", 2D) = "black" {}
		_Texture_1_Albedo_Index("Texture_1_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_1_H_AO_Index("Texture_1_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_2_Albedo_Index("Texture_2_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_2_H_AO_Index("Texture_2_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_3_H_AO_Index("Texture_3_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_3_Albedo_Index("Texture_3_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_4_H_AO_Index("Texture_4_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_4_Albedo_Index("Texture_4_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_5_Albedo_Index("Texture_5_Albedo_Index", Range( -1 , 100)) = -1
		_Texture_5_H_AO_Index("Texture_5_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_6_H_AO_Index("Texture_6_H_AO_Index", Range( -1 , 100)) = -1
		_Texture_Array_Albedo("Texture_Array_Albedo", 2DArray ) = "" {}
		_Texture_6_Albedo_Index("Texture_6_Albedo_Index", Range( -1 , 100)) = -1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+100" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			half2 uv_texcoord;
		};

		uniform UNITY_DECLARE_TEX2DARRAY( _Texture_Array_Normal );
		uniform half _Perlin_Normal_Tiling_Far;
		uniform int _Texture_Perlin_Normal_Index;
		uniform half _Perlin_Normal_Power;
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
		uniform half _Texture_5_Heightblend_Far;
		uniform half _Texture_6_Heightmap_Depth;
		uniform half _Texture_6_H_AO_Index;
		uniform half _Texture_6_Tiling;
		uniform half _Texture_6_Far_Multiplier;
		uniform half _Texture_6_Height_Contrast;
		uniform half _Texture_6_Heightblend_Far;
		uniform half _Texture_1_H_AO_Index;
		uniform half _Texture_1_Tiling;
		uniform half _Texture_1_Far_Multiplier;
		uniform half _Texture_1_Height_Contrast;
		uniform half _Texture_1_Heightmap_Depth;
		uniform half _Texture_1_Heightblend_Far;
		uniform half _Texture_2_Heightmap_Depth;
		uniform half _Texture_2_H_AO_Index;
		uniform half _Texture_2_Tiling;
		uniform half _Texture_2_Far_Multiplier;
		uniform half _Texture_2_Height_Contrast;
		uniform half _Texture_2_Heightblend_Far;
		uniform half _Texture_3_Heightmap_Depth;
		uniform half _Texture_3_H_AO_Index;
		uniform half _Texture_3_Tiling;
		uniform half _Texture_3_Far_Multiplier;
		uniform half _Texture_3_Height_Contrast;
		uniform half _Texture_3_Heightblend_Far;
		uniform half _Texture_4_Heightmap_Depth;
		uniform half _Texture_4_H_AO_Index;
		uniform half _Texture_4_Tiling;
		uniform half _Texture_4_Far_Multiplier;
		uniform half _Texture_4_Height_Contrast;
		uniform half _Texture_4_Heightblend_Far;
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
			half localTangent7740 = ( 0.0 );
			v.tangent.xyz = cross ( v.normal, float3( 0, 0, 1 ) );
			v.tangent.w = -1;
			half3 temp_cast_0 = (localTangent7740).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 break7753 = ase_worldPos;
			float2 appendResult7739 = (half2(break7753.x , break7753.z));
			half2 Top_Bottom1999 = appendResult7739;
			float4 texArray4374 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Normal, float3(( Top_Bottom1999 / _Perlin_Normal_Tiling_Far ), (float)_Texture_Perlin_Normal_Index)  );
			float2 appendResult11_g668 = (half2(texArray4374.w , texArray4374.y));
			float2 temp_output_4_0_g668 = ( ( ( appendResult11_g668 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _Perlin_Normal_Power );
			float2 break8_g668 = temp_output_4_0_g668;
			float dotResult5_g668 = dot( temp_output_4_0_g668 , temp_output_4_0_g668 );
			float temp_output_9_0_g668 = sqrt( ( 1.0 - saturate( dotResult5_g668 ) ) );
			float3 appendResult20_g668 = (half3(break8_g668.x , break8_g668.y , temp_output_9_0_g668));
			half4 tex2DNode4369 = tex2D( _Texture_Splat_2, i.uv_texcoord );
			half Splat2_G2107 = tex2DNode4369.g;
			half Splat2_R2106 = tex2DNode4369.r;
			half4 tex2DNode4368 = tex2D( _Texture_Splat_1, i.uv_texcoord );
			half Splat1_R1438 = tex2DNode4368.r;
			half Splat1_G1441 = tex2DNode4368.g;
			half Splat1_A1491 = tex2DNode4368.a;
			half Splat1_B1442 = tex2DNode4368.b;
			float clampResult3775 = clamp( ( ( _Texture_6_Perlin_Power * Splat2_G2107 ) + ( ( _Texture_5_Perlin_Power * Splat2_R2106 ) + ( ( _Texture_1_Perlin_Power * Splat1_R1438 ) + ( ( _Texture_2_Perlin_Power * Splat1_G1441 ) + ( ( _Texture_4_Perlin_Power * Splat1_A1491 ) + ( _Texture_3_Perlin_Power * Splat1_B1442 ) ) ) ) ) ) , 0.0 , 1.0 );
			float3 lerpResult3776 = lerp( float3( 0,0,1 ) , appendResult20_g668 , clampResult3775);
			o.Normal = lerpResult3776;
			float temp_output_4397_0 = ( 1.0 / _Texture_5_Tiling );
			float2 appendResult4399 = (half2(temp_output_4397_0 , temp_output_4397_0));
			float2 temp_output_4440_0 = ( ( Top_Bottom1999 * appendResult4399 ) / _Texture_5_Far_Multiplier );
			float4 texArray5655 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4440_0, _Texture_5_H_AO_Index)  );
			half ifLocalVar7668 = 0;
			UNITY_BRANCH 
			if( _Texture_5_H_AO_Index > -1.0 )
				ifLocalVar7668 = texArray5655.y;
			half Texture_5_H5671 = ifLocalVar7668;
			float HeightMask6205 = saturate(pow(((( _Texture_5_Heightmap_Depth * pow( Texture_5_H5671 , _Texture_5_Height_Contrast ) )*Splat2_R2106)*4)+(Splat2_R2106*2),_Texture_5_Heightblend_Far));
			float temp_output_4469_0 = ( 1.0 / _Texture_6_Tiling );
			float2 appendResult4471 = (half2(temp_output_4469_0 , temp_output_4469_0));
			float2 temp_output_4507_0 = ( ( Top_Bottom1999 * appendResult4471 ) / _Texture_6_Far_Multiplier );
			float4 texArray5695 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4507_0, _Texture_6_H_AO_Index)  );
			half ifLocalVar7670 = 0;
			UNITY_BRANCH 
			if( _Texture_6_H_AO_Index > -1.0 )
				ifLocalVar7670 = texArray5695.y;
			half Texture_6_H5711 = ifLocalVar7670;
			float HeightMask6208 = saturate(pow(((( _Texture_6_Heightmap_Depth * pow( Texture_6_H5711 , _Texture_6_Height_Contrast ) )*Splat2_G2107)*4)+(Splat2_G2107*2),_Texture_6_Heightblend_Far));
			float4 appendResult6524 = (half4(HeightMask6205 , HeightMask6208 , 0.0 , 0.0));
			float temp_output_3830_0 = ( 1.0 / _Texture_1_Tiling );
			float2 appendResult3284 = (half2(temp_output_3830_0 , temp_output_3830_0));
			float2 temp_output_3298_0 = ( ( Top_Bottom1999 * appendResult3284 ) / _Texture_1_Far_Multiplier );
			float4 texArray7704 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3298_0, _Texture_1_H_AO_Index)  );
			half ifLocalVar7658 = 0;
			UNITY_BRANCH 
			if( _Texture_1_H_AO_Index > -1.0 )
				ifLocalVar7658 = texArray7704.g;
			half Texture_1_H5480 = ifLocalVar7658;
			float HeightMask6196 = saturate(pow(((( pow( Texture_1_H5480 , _Texture_1_Height_Contrast ) * _Texture_1_Heightmap_Depth )*Splat1_R1438)*4)+(Splat1_R1438*2),_Texture_1_Heightblend_Far));
			float temp_output_3831_0 = ( 1.0 / _Texture_2_Tiling );
			float2 appendResult3349 = (half2(temp_output_3831_0 , temp_output_3831_0));
			float2 temp_output_3345_0 = ( ( Top_Bottom1999 * appendResult3349 ) / _Texture_2_Far_Multiplier );
			float4 texArray5533 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3345_0, _Texture_2_H_AO_Index)  );
			half ifLocalVar7662 = 0;
			UNITY_BRANCH 
			if( _Texture_2_H_AO_Index > -1.0 )
				ifLocalVar7662 = texArray5533.y;
			half Texture_2_H5497 = ifLocalVar7662;
			float HeightMask6515 = saturate(pow(((( _Texture_2_Heightmap_Depth * pow( Texture_2_H5497 , _Texture_2_Height_Contrast ) )*Splat1_G1441)*4)+(Splat1_G1441*2),_Texture_2_Heightblend_Far));
			float temp_output_3832_0 = ( 1.0 / _Texture_3_Tiling );
			float2 appendResult3415 = (half2(temp_output_3832_0 , temp_output_3832_0));
			float2 temp_output_3412_0 = ( ( Top_Bottom1999 * appendResult3415 ) / _Texture_3_Far_Multiplier );
			float4 texArray5586 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3412_0, _Texture_3_H_AO_Index)  );
			half ifLocalVar7664 = 0;
			UNITY_BRANCH 
			if( _Texture_3_H_AO_Index > -1.0 )
				ifLocalVar7664 = texArray5586.y;
			half Texture_3_H5581 = ifLocalVar7664;
			float HeightMask6516 = saturate(pow(((( _Texture_3_Heightmap_Depth * pow( Texture_3_H5581 , _Texture_3_Height_Contrast ) )*Splat1_B1442)*4)+(Splat1_B1442*2),_Texture_3_Heightblend_Far));
			float temp_output_3833_0 = ( 1.0 / _Texture_4_Tiling );
			float2 appendResult3482 = (half2(temp_output_3833_0 , temp_output_3833_0));
			float2 temp_output_3479_0 = ( ( Top_Bottom1999 * appendResult3482 ) / _Texture_4_Far_Multiplier );
			float4 texArray5615 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3479_0, _Texture_4_H_AO_Index)  );
			half ifLocalVar7666 = 0;
			UNITY_BRANCH 
			if( _Texture_4_H_AO_Index > -1.0 )
				ifLocalVar7666 = texArray5615.y;
			half Texture_4_H5631 = ifLocalVar7666;
			float HeightMask6203 = saturate(pow(((( _Texture_4_Heightmap_Depth * pow( Texture_4_H5631 , _Texture_4_Height_Contrast ) )*Splat1_A1491)*4)+(Splat1_A1491*2),_Texture_4_Heightblend_Far));
			float4 appendResult6517 = (half4(HeightMask6196 , HeightMask6515 , HeightMask6516 , HeightMask6203));
			float4 texArray3292 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3298_0, _Texture_1_Albedo_Index)  );
			half4 ifLocalVar7657 = 0;
			UNITY_BRANCH 
			if( _Texture_1_Albedo_Index > -1.0 )
				ifLocalVar7657 = ( texArray3292 * _Texture_1_Color );
			half4 Texture_1_Final950 = ifLocalVar7657;
			float4 texArray3339 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3345_0, _Texture_2_Albedo_Index)  );
			half4 ifLocalVar7661 = 0;
			UNITY_BRANCH 
			if( _Texture_2_Albedo_Index > -1.0 )
				ifLocalVar7661 = ( texArray3339 * _Texture_2_Color );
			half4 Texture_2_Final3385 = ifLocalVar7661;
			float4 texArray3406 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3412_0, _Texture_3_Albedo_Index)  );
			half4 ifLocalVar7663 = 0;
			UNITY_BRANCH 
			if( _Texture_3_Albedo_Index > -1.0 )
				ifLocalVar7663 = ( texArray3406 * _Texture_3_Color );
			half4 Texture_3_Final3451 = ifLocalVar7663;
			float4 texArray3473 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_3479_0, _Texture_4_Albedo_Index)  );
			half4 ifLocalVar7665 = 0;
			UNITY_BRANCH 
			if( _Texture_4_Albedo_Index > -1.0 )
				ifLocalVar7665 = ( texArray3473 * _Texture_4_Color );
			half4 Texture_4_Final3518 = ifLocalVar7665;
			float4 layeredBlendVar6512 = appendResult6517;
			float4 layeredBlend6512 = ( lerp( lerp( lerp( lerp( float4( 0,0,0,0 ) , Texture_1_Final950 , layeredBlendVar6512.x ) , Texture_2_Final3385 , layeredBlendVar6512.y ) , Texture_3_Final3451 , layeredBlendVar6512.z ) , Texture_4_Final3518 , layeredBlendVar6512.w ) );
			float4 texArray4445 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4440_0, _Texture_5_Albedo_Index)  );
			half4 ifLocalVar7667 = 0;
			UNITY_BRANCH 
			if( _Texture_5_Albedo_Index > -1.0 )
				ifLocalVar7667 = ( texArray4445 * _Texture_5_Color );
			half4 Texture_5_Final4396 = ifLocalVar7667;
			float4 texArray4512 = UNITY_SAMPLE_TEX2DARRAY(_Texture_Array_Albedo, float3(temp_output_4507_0, _Texture_6_Albedo_Index)  );
			half4 ifLocalVar7669 = 0;
			UNITY_BRANCH 
			if( _Texture_6_Albedo_Index > -1.0 )
				ifLocalVar7669 = ( texArray4512 * _Texture_6_Color );
			half4 Texture_6_Final4536 = ifLocalVar7669;
			half4 _Vector0 = half4(0,0,0,0);
			float4 layeredBlendVar6520 = appendResult6524;
			float4 layeredBlend6520 = ( lerp( lerp( lerp( lerp( layeredBlend6512 , Texture_5_Final4396 , layeredBlendVar6520.x ) , Texture_6_Final4536 , layeredBlendVar6520.y ) , _Vector0 , layeredBlendVar6520.z ) , _Vector0 , layeredBlendVar6520.w ) );
			float4 break3856 = layeredBlend6520;
			float3 appendResult3857 = (half3(break3856.x , break3856.y , break3856.z));
			half2 temp_cast_1 = (( ( ase_worldPos.y / _Geological_Tiling_Far ) + _Geological_Map_Offset_Far )).xx;
			half4 tex2DNode6969 = tex2D( _Texture_Geological_Map, temp_cast_1 );
			float3 appendResult6971 = (half3(tex2DNode6969.r , tex2DNode6969.g , tex2DNode6969.b));
			half3 blendOpSrc4362 = appendResult3857;
			half3 blendOpDest4362 = ( ( ( appendResult6971 + float3( -0.3,-0.3,-0.3 ) ) * _Geological_Map_Far_Power ) * ( ( _Texture_6_Geological_Power * Splat2_G2107 ) + ( ( _Texture_5_Geological_Power * Splat2_R2106 ) + ( ( _Texture_1_Geological_Power * Splat1_R1438 ) + ( ( _Texture_2_Geological_Power * Splat1_G1441 ) + ( ( _Texture_4_Geological_Power * Splat1_A1491 ) + ( _Texture_3_Geological_Power * Splat1_B1442 ) ) ) ) ) ) );
			o.Albedo = ( saturate( ( blendOpSrc4362 + blendOpDest4362 ) ));
			o.Specular = ( ( appendResult3857 * float3( 0.3,0.3,0.3 ) ) * _Terrain_Specular );
			o.Smoothness = ( break3856.w * _Terrain_Smoothness );
			o.Occlusion = 1.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}