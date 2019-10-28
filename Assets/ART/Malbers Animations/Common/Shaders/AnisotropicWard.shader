// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Anisotropic/Ward"
{
	Properties
	{
		_TillingOffset("Tilling & Offset", Vector) = (1,1,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.6
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,1)
		_Specular("Specular", 2D) = "white" {}
		_SpecularTint("Specular Tint", Color) = (1,1,1,1)
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalAmount("Normal Amount", Float) = 1
		_AnisotropyX("Anisotropy X", Range( 0 , 1)) = 1
		_AnisotropyY("Anisotropy Y", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _NormalAmount;
		uniform sampler2D _NormalMap;
		uniform float4 _TillingOffset;
		uniform sampler2D _Albedo;
		uniform float4 _AlbedoTint;
		uniform sampler2D _Specular;
		uniform float4 _SpecularTint;
		uniform float _AnisotropyX;
		uniform float _AnisotropyY;
		uniform float _Cutoff = 0.6;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 appendResult85 = (float2(_TillingOffset.z , _TillingOffset.w));
			float2 uv_TexCoord82 = i.uv_texcoord * appendResult85;
			float3 tex2DNode25 = UnpackScaleNormal( tex2D( _NormalMap, uv_TexCoord82 ) ,_NormalAmount );
			o.Normal = tex2DNode25;
			float4 temp_output_60_0 = ( tex2D( _Albedo, uv_TexCoord82 ) * _AlbedoTint );
			o.Albedo = temp_output_60_0.rgb;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 LightDirection12 = ase_worldlightDir;
			float3 normalizeResult7 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
			float3 ViewDirection8 = normalizeResult7;
			float3 normalizeResult71 = normalize( ( LightDirection12 + ViewDirection8 ) );
			float3 HalfVector16 = normalizeResult71;
			float3 NormalDirection56 = normalize( WorldNormalVector( i , tex2DNode25 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3 normalizeResult79 = normalize( cross( NormalDirection56 , ase_worldBitangent ) );
			float3 TangentDirection78 = normalizeResult79;
			float dotResult34 = dot( HalfVector16 , TangentDirection78 );
			float HX68 = ( dotResult34 / _AnisotropyX );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 normalizeResult76 = normalize( cross( NormalDirection56 , ase_worldTangent ) );
			float3 BinormalDirection22 = normalizeResult76;
			float dotResult35 = dot( HalfVector16 , BinormalDirection22 );
			float HY41 = ( dotResult35 / _AnisotropyY );
			float dotResult29 = dot( NormalDirection56 , HalfVector16 );
			float NdotH30 = dotResult29;
			float dotResult26 = dot( NormalDirection56 , LightDirection12 );
			float NdotL27 = dotResult26;
			o.Specular = ( ( tex2D( _Specular, uv_TexCoord82 ) * _SpecularTint ) * ( exp( ( ( ( ( HX68 * HX68 ) + ( HY41 * HY41 ) ) / ( NdotH30 + 1.0 ) ) * -2.0 ) ) * max( NdotL27 , 0.0 ) ) ).rgb;
			o.Alpha = 1;
			clip( temp_output_60_0.a - _Cutoff );
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
180;337;1080;540;3991.346;-305.1346;1;True;False
Node;AmplifyShaderEditor.Vector4Node;83;-5104.344,-1005.934;Float;True;Property;_TillingOffset;Tilling & Offset;0;0;Create;True;0;0;False;0;1,1,0,0;1,1,4.51,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;9;-3964.098,372.1005;Float;False;851.9999;454.2498;View Direction Vector;5;8;7;6;2;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;-4751.145,-956.6329;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;2;-3945.903,420.5996;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-3913.506,582.1996;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;58;-4477.512,-483.7573;Float;False;Property;_NormalAmount;Normal Amount;7;0;Create;True;0;0;False;0;1;1.66;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;82;-4452.483,-973.6951;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;6;-3655.7,439.3;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;25;-4161.5,-542.8994;Float;True;Property;_NormalMap;Normal Map;6;0;Create;True;0;0;False;0;None;e36807a1aeb234a45bae70c88ccd5fc8;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;57;-3695.204,-319.6251;Float;False;528.1206;281.0598;Normal Direction Vector;2;23;56;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;-3703.892,27.7998;Float;False;563.3999;295.5665;Light Direction Vector;2;4;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;23;-3653.093,-276.5013;Float;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-3632.809,94.25116;Float;True;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;7;-3440.398,449.4003;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;24;-2701.581,-418.403;Float;False;776.0801;271.7102;Binormal Direction Vector;4;76;19;22;77;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexBinormalNode;81;-2948.264,-752.8102;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;17;-2678.596,51.89973;Float;False;515.2925;268.7952;Halfway Vector;3;14;71;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexTangentNode;77;-2664.848,-301.4307;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-3369.497,104.7998;Float;False;LightDirection;2;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-3403.503,-255.7254;Float;False;NormalDirection;1;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-3379.309,699.1993;Float;False;ViewDirection;3;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;21;-2691.003,-805.1019;Float;False;921.1089;263.5102;Tangent Direction Vector;3;78;79;80;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CrossProductOpNode;80;-2461.048,-763.3286;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-2666.7,97.40108;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CrossProductOpNode;19;-2454.699,-372.1008;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;79;-2225.546,-761.8284;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;71;-2528.846,98.07848;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;76;-2296.958,-333.5321;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2371.798,98.70107;Float;True;HalfVector;6;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-2149.901,-309.2012;Float;False;BinormalDirection;5;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-2057.547,-760.6287;Float;True;TangentDirection;4;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1649.396,265.4982;Float;False;Property;_AnisotropyY;Anisotropy Y;9;0;Create;True;0;0;False;0;1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;35;-1516.398,97.39842;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;34;-1605.774,-255.2514;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1613.096,9.198168;Float;False;Property;_AnisotropyX;Anisotropy X;8;0;Create;True;0;0;False;0;1;0.5508147;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;29;-1957.698,373.9995;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;37;-1286.598,165.5984;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;-1287.598,-96.40155;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1094.196,-99.32366;Float;True;HX;10;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;41;-1096.196,160.9981;Float;True;HY;11;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1810.798,388.1992;Float;False;NdotH;9;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-795.595,-20.32379;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1089.397,396.2962;Float;True;2;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-854.1968,154.4969;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;92;-2942.107,377.6998;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;91;-2925.875,612.3439;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;26;-2655.965,602.4556;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;93;-639.1787,390.1096;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-604.8968,9.19689;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-2370.183,559.6396;Float;True;NdotL;8;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;47;-419.2962,64.19641;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;94;-872.1787,600.1096;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-245.9968,79.59687;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;44;-74.2966,99.3969;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;88;-79.22814,349.0702;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;59;256.7027,-603.4249;Float;False;Property;_AlbedoTint;Albedo Tint;3;0;Create;True;0;0;False;0;1,1,1,1;0.3235294,0.2737639,0.2355294,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;61;221.8039,-293.2251;Float;True;Property;_Specular;Specular;4;0;Create;True;0;0;False;0;None;43a4069e5be635e42aacce6f0e64fb5a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;236.7004,-798.4986;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;None;61ee572e89ec0bb428a27ae25c12dd48;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;65;269.2042,-95.6257;Float;False;Property;_SpecularTint;Specular Tint;5;0;Create;True;0;0;False;0;1,1,1,1;0.7279412,0.5353365,0.1712803,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;312.5681,206.1701;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;592.9036,-665.226;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;569.3049,-159.126;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-2375.544,339.7971;Float;True;NdotV;7;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;817.605,-68.02586;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-4750.445,-1062.533;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;66;842.3063,-372.9246;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DotProductOpNode;32;-2661.953,359.5889;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1333.799,-451.6001;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Malbers/Anisotropic/Ward;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;0;False;0;Masked;0.6;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;85;0;83;3
WireConnection;85;1;83;4
WireConnection;82;0;85;0
WireConnection;6;0;2;0
WireConnection;6;1;10;0
WireConnection;25;1;82;0
WireConnection;25;5;58;0
WireConnection;23;0;25;0
WireConnection;7;0;6;0
WireConnection;12;0;4;0
WireConnection;56;0;23;0
WireConnection;8;0;7;0
WireConnection;80;0;56;0
WireConnection;80;1;81;0
WireConnection;14;0;12;0
WireConnection;14;1;8;0
WireConnection;19;0;56;0
WireConnection;19;1;77;0
WireConnection;79;0;80;0
WireConnection;71;0;14;0
WireConnection;76;0;19;0
WireConnection;16;0;71;0
WireConnection;22;0;76;0
WireConnection;78;0;79;0
WireConnection;35;0;16;0
WireConnection;35;1;22;0
WireConnection;34;0;16;0
WireConnection;34;1;78;0
WireConnection;29;0;56;0
WireConnection;29;1;16;0
WireConnection;37;0;35;0
WireConnection;37;1;39;0
WireConnection;36;0;34;0
WireConnection;36;1;38;0
WireConnection;68;0;36;0
WireConnection;41;0;37;0
WireConnection;30;0;29;0
WireConnection;67;0;68;0
WireConnection;67;1;68;0
WireConnection;48;0;30;0
WireConnection;43;0;41;0
WireConnection;43;1;41;0
WireConnection;92;0;56;0
WireConnection;91;0;12;0
WireConnection;26;0;92;0
WireConnection;26;1;91;0
WireConnection;93;0;48;0
WireConnection;45;0;67;0
WireConnection;45;1;43;0
WireConnection;27;0;26;0
WireConnection;47;0;45;0
WireConnection;47;1;93;0
WireConnection;94;0;27;0
WireConnection;46;0;47;0
WireConnection;44;0;46;0
WireConnection;88;0;94;0
WireConnection;61;1;82;0
WireConnection;1;1;82;0
WireConnection;87;0;44;0
WireConnection;87;1;88;0
WireConnection;60;0;1;0
WireConnection;60;1;59;0
WireConnection;62;0;61;0
WireConnection;62;1;65;0
WireConnection;33;0;32;0
WireConnection;63;0;62;0
WireConnection;63;1;87;0
WireConnection;84;0;83;1
WireConnection;84;1;83;2
WireConnection;66;0;60;0
WireConnection;32;0;56;0
WireConnection;32;1;8;0
WireConnection;0;0;60;0
WireConnection;0;1;25;0
WireConnection;0;3;63;0
WireConnection;0;10;66;3
ASEEND*/
//CHKSM=0763E580006D765292BD739465996B08DDA5741F