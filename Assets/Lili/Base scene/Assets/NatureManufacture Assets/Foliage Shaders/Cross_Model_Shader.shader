Shader "NatureManufacture Shaders/Trees/Cross Model Shader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.65
		_ColorAdjustment("Color Adjustment", Vector) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		_HealthyColor("Healthy Color", Color) = (1,0.9735294,0.9338235,1)
		_Smooothness("Smooothness", Float) = 0.3
		_AO("AO", Float) = 1
		[NoScaleOffset]_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 3)) = 1
		_InitialBend("Wind Initial Bend", Float) = 1
		_Stiffness("Wind Stiffness", Float) = 1
		_Drag("Wind Drag", Float) = 0.2
		_NewNormal("Vertex Normal Multiply", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#include "NMWindNoShiver.cginc"
		#include "NM_indirect.cginc"
		#pragma vertex vert
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _HealthyColor;
		uniform float3 _ColorAdjustment;
		uniform float _Smooothness;
		uniform float _AO;
		uniform float _Cutoff = 0.65;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_BumpScale );
			float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( ( tex2DNode2 * _HealthyColor ) * float4( _ColorAdjustment , 0.0 ) ).rgb;
			float3 temp_cast_2 = (0.0).xxx;
			o.Specular = temp_cast_2;
			o.Smoothness = _Smooothness;
			o.Occlusion = _AO;
			o.Alpha = 1;
			clip( tex2DNode2.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}