// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Anisotropic/Circular"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0
		[NoScaleOffset]_AlbedoRGBOpacityA("Albedo (RGB) Opacity (A)", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,1)
		[NoScaleOffset]_Specular("Specular", 2D) = "white" {}
		_SpecularTint("Specular Tint", Color) = (1,1,1,1)
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalAmount("Normal Amount", Float) = 1
		_AnisotropyFalloff("Anisotropy Falloff", Range( 1 , 256)) = 64
		_AnisotropyOffset("Anisotropy Offset", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Off
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform float _NormalAmount;
		uniform sampler2D _Normal;
		uniform float4 _AlbedoTint;
		uniform sampler2D _AlbedoRGBOpacityA;
		uniform float4 _SpecularTint;
		uniform sampler2D _Specular;
		uniform float _AnisotropyOffset;
		uniform float _AnisotropyFalloff;
		uniform float _Cutoff = 0;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Normal45 = i.uv_texcoord;
			float3 NormalMap62 = UnpackScaleNormal( tex2D( _Normal, uv_Normal45, float2( 0,0 ), float2( 0,0 ) ) ,_NormalAmount );
			o.Normal = NormalMap62;
			float2 uv_AlbedoRGBOpacityA1 = i.uv_texcoord;
			float4 tex2DNode1 = tex2D( _AlbedoRGBOpacityA, uv_AlbedoRGBOpacityA1 );
			o.Albedo = ( _AlbedoTint * tex2DNode1 ).rgb;
			float2 uv_Specular4 = i.uv_texcoord;
			float3 PixelNormalWorld52 = normalize( WorldNormalVector( i , NormalMap62 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 LightDirection16 = ase_worldlightDir;
			float3 normalizeResult9 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
			float3 ViewDirection11 = normalizeResult9;
			float3 normalizeResult18 = normalize( ( LightDirection16 + ViewDirection11 ) );
			float3 HalfVector46 = normalizeResult18;
			float dotResult23 = dot( PixelNormalWorld52 , HalfVector46 );
			float nDotH24 = dotResult23;
			float dotResult21 = dot( PixelNormalWorld52 , LightDirection16 );
			float nDotL22 = dotResult21;
			o.Specular = max( ( ( ( _SpecularTint * tex2D( _Specular, uv_Specular4 ) ) * pow( max( sin( radians( ( ( _AnisotropyOffset + nDotH24 ) * 180.0 ) ) ) , 0.0 ) , _AnisotropyFalloff ) ) * nDotL22 ) , float4( 0,0,0,0 ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows 

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
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
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
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
130;218;1666;825;5197.627;294.3466;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;57;-3991.898,642.3001;Float;False;891.5006;424.4899;View Direction Vector;5;10;6;11;9;8;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-3974.112,869.1987;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;6;-3975.809,680.5999;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;8;-3699.808,732.3992;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;59;-3648.292,314.6999;Float;False;533.0206;260.4803;Light Direction Vector;2;14;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-4356.228,51.32429;Float;False;Property;_NormalAmount;Normal Amount;6;0;Create;True;0;0;False;0;1;-2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;9;-3472.11,833.199;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;14;-3628.803,355.3016;Float;True;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-3304.008,728.8986;Float;False;ViewDirection;4;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;60;-2989.588,457.5991;Float;False;661.2201;238.5203;Halfway Vector;3;46;18;17;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;45;-4064.827,-44.64075;Float;True;Property;_Normal;Normal;5;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;3ace6785e82879140a83d7b71c25b6a2;True;0;True;bump;Auto;True;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-3362.401,353.8022;Float;True;LightDirection;3;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-3763.211,-48.7751;Float;False;NormalMap;1;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;61;-3555,-96;Float;False;537.9105;289.5802;Pixel Normal Vector;2;51;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-2943.107,530.9026;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;51;-3526,-44;Float;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;18;-2808.305,500.902;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2570.402,505.2017;Float;False;HalfVector;6;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-3282,-33;Float;True;PixelNormalWorld;2;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;23;-2335.101,279.9036;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2166.802,277.3034;Float;False;nDotH;7;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2303.304,176.503;Float;False;Property;_AnisotropyOffset;Anisotropy Offset;8;0;Create;True;0;0;False;0;0;-0.23;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1955.204,253.5026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1787.703,253.0026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;180;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;29;-1604.702,253.3027;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1599.799,508.6237;Float;False;Property;_AnisotropyFalloff;Anisotropy Falloff;7;0;Create;True;0;0;False;0;64;132;1;256;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;30;-1430.901,254.2025;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1344.803,-29.40015;Float;True;Property;_Specular;Specular;3;1;[NoScaleOffset];Create;True;0;0;False;0;None;fdff26987352bec4fb16248607342456;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;43;-1315.204,-195.798;Float;False;Property;_SpecularTint;Specular Tint;4;0;Create;True;0;0;False;0;1,1,1,1;0.625,0.625,0.625,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;31;-1251.303,254.8026;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;64;-1145.531,400.0973;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;21;-2942.199,74.0029;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;38;-984.004,252.1033;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1042.105,-89.49779;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-850.9669,-90.29791;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-2715.766,67.66237;Float;False;nDotL;5;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-843.8511,160.7657;Float;False;22;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-1266.202,-631.3965;Float;False;Property;_AlbedoTint;Albedo Tint;2;0;Create;True;0;0;False;0;1,1,1,1;0.8970588,0.8970588,0.8970588,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1288.102,-460.4998;Float;True;Property;_AlbedoRGBOpacityA;Albedo (RGB) Opacity (A);1;1;[NoScaleOffset];Create;True;0;0;False;0;None;ce559ff03dfd1f745a86e88ddf1582ad;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-595.3027,142.2005;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;66;-187.7192,-100.433;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-321.8917,-324.7738;Float;True;62;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;54;-335.202,206.4997;Float;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-928.085,-544.9233;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;34.43259,-348.373;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Malbers/Anisotropic/Circular;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;0;False;0;Custom;0;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;6;0
WireConnection;8;1;10;0
WireConnection;9;0;8;0
WireConnection;11;0;9;0
WireConnection;45;5;47;0
WireConnection;16;0;14;0
WireConnection;62;0;45;0
WireConnection;17;0;16;0
WireConnection;17;1;11;0
WireConnection;51;0;62;0
WireConnection;18;0;17;0
WireConnection;46;0;18;0
WireConnection;52;0;51;0
WireConnection;23;0;52;0
WireConnection;23;1;46;0
WireConnection;24;0;23;0
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;27;0;26;0
WireConnection;29;0;27;0
WireConnection;30;0;29;0
WireConnection;31;0;30;0
WireConnection;64;0;3;0
WireConnection;21;0;52;0
WireConnection;21;1;16;0
WireConnection;38;0;31;0
WireConnection;38;1;64;0
WireConnection;44;0;43;0
WireConnection;44;1;4;0
WireConnection;39;0;44;0
WireConnection;39;1;38;0
WireConnection;22;0;21;0
WireConnection;53;0;39;0
WireConnection;53;1;65;0
WireConnection;66;0;1;4
WireConnection;54;0;53;0
WireConnection;41;0;40;0
WireConnection;41;1;1;0
WireConnection;0;0;41;0
WireConnection;0;1;63;0
WireConnection;0;3;54;0
WireConnection;0;10;66;0
ASEEND*/
//CHKSM=1AF9C025417C547A4B85FA94C8D63234643EE20D