// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Color4x4v2"
{
	Properties
	{
		_Color1("Color 1", Color) = (0.8088235,0.2378893,0.2378893,0.603)
		_Color2("Color 2", Color) = (0.7942072,0.9191176,0.01351645,0.347)
		_Color3("Color 3", Color) = (0.07444853,0.4662017,0.5955882,0.172)
		_Color4("Color 4", Color) = (0.5882353,0.5363321,0.5363321,0.428)
		_Color5("Color 5", Color) = (0.008974922,0.2868259,0.6102941,0.866)
		_Color6("Color 6", Color) = (0.2468329,0.1977184,0.5073529,0.822)
		_Color7("Color 7", Color) = (0.803,0.303,0.572,0.478)
		_Color8("Color 8", Color) = (0.991,0.647,0.684,0.591)
		_Color9("Color 9", Color) = (0,0.859,0.353,0.497)
		_Color10("Color 10", Color) = (0.303,0.503,0,0.241)
		_Color11("Color 11", Color) = (0.4505191,0.4558824,0.06704153,0.684)
		_Color12("Color 12", Color) = (0.1397059,0.4701825,0.5,0.616)
		_Color13("Color 13", Color) = (0.3455882,0.05590397,0.2297145,0)
		_Color14("Color 14", Color) = (0.3657548,0.4809159,0.9044118,0.616)
		_Color15("Color 15", Color) = (1,0,0,0.503)
		_Color16("Color 16", Color) = (1,0.6598378,0.05147058,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5953899
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _Color1;
		uniform float4 _Color2;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform float4 _Color5;
		uniform float4 _Color6;
		uniform float4 _Color7;
		uniform float4 _Color8;
		uniform float4 _Color9;
		uniform float4 _Color10;
		uniform float4 _Color11;
		uniform float4 _Color12;
		uniform float4 _Color13;
		uniform float4 _Color14;
		uniform float4 _Color15;
		uniform float4 _Color16;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_3_0_g1 = 1.0;
			float temp_output_7_0_g1 = 4.0;
			float temp_output_9_0_g1 = 4.0;
			float temp_output_8_0_g1 = 4.0;
			float temp_output_3_0_g2 = 2.0;
			float temp_output_7_0_g2 = 4.0;
			float temp_output_9_0_g2 = 4.0;
			float temp_output_8_0_g2 = 4.0;
			float temp_output_3_0_g3 = 3.0;
			float temp_output_7_0_g3 = 4.0;
			float temp_output_9_0_g3 = 4.0;
			float temp_output_8_0_g3 = 4.0;
			float temp_output_3_0_g4 = 4.0;
			float temp_output_7_0_g4 = 4.0;
			float temp_output_9_0_g4 = 4.0;
			float temp_output_8_0_g4 = 4.0;
			float temp_output_3_0_g5 = 1.0;
			float temp_output_7_0_g5 = 4.0;
			float temp_output_9_0_g5 = 3.0;
			float temp_output_8_0_g5 = 4.0;
			float temp_output_3_0_g6 = 2.0;
			float temp_output_7_0_g6 = 4.0;
			float temp_output_9_0_g6 = 3.0;
			float temp_output_8_0_g6 = 4.0;
			float temp_output_3_0_g7 = 3.0;
			float temp_output_7_0_g7 = 4.0;
			float temp_output_9_0_g7 = 3.0;
			float temp_output_8_0_g7 = 4.0;
			float temp_output_3_0_g8 = 4.0;
			float temp_output_7_0_g8 = 4.0;
			float temp_output_9_0_g8 = 3.0;
			float temp_output_8_0_g8 = 4.0;
			float temp_output_3_0_g9 = 1.0;
			float temp_output_7_0_g9 = 4.0;
			float temp_output_9_0_g9 = 2.0;
			float temp_output_8_0_g9 = 4.0;
			float temp_output_3_0_g10 = 2.0;
			float temp_output_7_0_g10 = 4.0;
			float temp_output_9_0_g10 = 2.0;
			float temp_output_8_0_g10 = 4.0;
			float temp_output_3_0_g11 = 3.0;
			float temp_output_7_0_g11 = 4.0;
			float temp_output_9_0_g11 = 2.0;
			float temp_output_8_0_g11 = 4.0;
			float temp_output_3_0_g12 = 4.0;
			float temp_output_7_0_g12 = 4.0;
			float temp_output_9_0_g12 = 2.0;
			float temp_output_8_0_g12 = 4.0;
			float temp_output_3_0_g13 = 1.0;
			float temp_output_7_0_g13 = 4.0;
			float temp_output_9_0_g13 = 1.0;
			float temp_output_8_0_g13 = 4.0;
			float temp_output_3_0_g14 = 2.0;
			float temp_output_7_0_g14 = 4.0;
			float temp_output_9_0_g14 = 1.0;
			float temp_output_8_0_g14 = 4.0;
			float temp_output_3_0_g15 = 3.0;
			float temp_output_7_0_g15 = 4.0;
			float temp_output_9_0_g15 = 1.0;
			float temp_output_8_0_g15 = 4.0;
			float temp_output_3_0_g16 = 4.0;
			float temp_output_7_0_g16 = 4.0;
			float temp_output_9_0_g16 = 1.0;
			float temp_output_8_0_g16 = 4.0;
			float4 temp_output_28_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g1 - 1.0 ) / temp_output_7_0_g1 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g1 / temp_output_7_0_g1 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g1 - 1.0 ) / temp_output_8_0_g1 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g1 / temp_output_8_0_g1 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g2 - 1.0 ) / temp_output_7_0_g2 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g2 / temp_output_7_0_g2 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g2 - 1.0 ) / temp_output_8_0_g2 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g2 / temp_output_8_0_g2 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g3 - 1.0 ) / temp_output_7_0_g3 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g3 / temp_output_7_0_g3 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g3 - 1.0 ) / temp_output_8_0_g3 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g3 / temp_output_8_0_g3 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g4 - 1.0 ) / temp_output_7_0_g4 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g4 / temp_output_7_0_g4 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g4 - 1.0 ) / temp_output_8_0_g4 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g4 / temp_output_8_0_g4 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g5 - 1.0 ) / temp_output_7_0_g5 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g5 / temp_output_7_0_g5 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g5 - 1.0 ) / temp_output_8_0_g5 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g5 / temp_output_8_0_g5 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g6 - 1.0 ) / temp_output_7_0_g6 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g6 / temp_output_7_0_g6 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g6 - 1.0 ) / temp_output_8_0_g6 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g6 / temp_output_8_0_g6 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g7 - 1.0 ) / temp_output_7_0_g7 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g7 / temp_output_7_0_g7 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g7 - 1.0 ) / temp_output_8_0_g7 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g7 / temp_output_8_0_g7 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g8 - 1.0 ) / temp_output_7_0_g8 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g8 / temp_output_7_0_g8 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g8 - 1.0 ) / temp_output_8_0_g8 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g8 / temp_output_8_0_g8 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g9 - 1.0 ) / temp_output_7_0_g9 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g9 / temp_output_7_0_g9 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g9 - 1.0 ) / temp_output_8_0_g9 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g9 / temp_output_8_0_g9 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g10 - 1.0 ) / temp_output_7_0_g10 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g10 / temp_output_7_0_g10 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g10 - 1.0 ) / temp_output_8_0_g10 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g10 / temp_output_8_0_g10 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g11 - 1.0 ) / temp_output_7_0_g11 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g11 / temp_output_7_0_g11 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g11 - 1.0 ) / temp_output_8_0_g11 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g11 / temp_output_8_0_g11 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g12 - 1.0 ) / temp_output_7_0_g12 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g12 / temp_output_7_0_g12 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g12 - 1.0 ) / temp_output_8_0_g12 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g12 / temp_output_8_0_g12 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g13 - 1.0 ) / temp_output_7_0_g13 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g13 / temp_output_7_0_g13 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g13 - 1.0 ) / temp_output_8_0_g13 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g13 / temp_output_8_0_g13 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g14 - 1.0 ) / temp_output_7_0_g14 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g14 / temp_output_7_0_g14 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g14 - 1.0 ) / temp_output_8_0_g14 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g14 / temp_output_8_0_g14 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g15 - 1.0 ) / temp_output_7_0_g15 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g15 / temp_output_7_0_g15 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g15 - 1.0 ) / temp_output_8_0_g15 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g15 / temp_output_8_0_g15 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g16 - 1.0 ) / temp_output_7_0_g16 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g16 / temp_output_7_0_g16 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g16 - 1.0 ) / temp_output_8_0_g16 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g16 / temp_output_8_0_g16 ) ) * 1.0 ) ) ) ) ) );
			o.Albedo = temp_output_28_0.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = ( (temp_output_28_0).a * _Smoothness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
1927;29;1352;732;781.71;-496.0655;2.615016;True;True
Node;AmplifyShaderEditor.ColorNode;12;-1233.216,87.29334;Float;False;Property;_Color3;Color 3;2;0;Create;True;0;0;False;0;0.07444853,0.4662017,0.5955882,0.172;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-1243.943,-96.81123;Float;False;Property;_Color2;Color 2;1;0;Create;True;0;0;False;0;0.7942072,0.9191176,0.01351645,0.347;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-1236.792,276.7597;Float;False;Property;_Color4;Color 4;3;0;Create;True;0;0;False;0;0.5882353,0.5363321,0.5363321,0.428;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-1259.084,803.9532;Float;False;Property;_Color6;Color 6;5;0;Create;True;0;0;False;0;0.2468329,0.1977184,0.5073529,0.822;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-1249.832,1184.475;Float;False;Property;_Color8;Color 8;7;0;Create;True;0;0;False;0;0.991,0.647,0.684,0.591;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-1239.928,-324.3583;Half;False;Property;_Color1;Color 1;0;0;Create;True;0;0;False;0;0.8088235,0.2378893,0.2378893,0.603;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-1264.003,608.2642;Float;False;Property;_Color5;Color 5;4;0;Create;True;0;0;False;0;0.008974922,0.2868259,0.6102941,0.866;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;16;-1246.661,984.1765;Float;False;Property;_Color7;Color 7;6;0;Create;True;0;0;False;0;0.803,0.303,0.572,0.478;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;-1112.07,2989.539;Float;False;Property;_Color16;Color 16;15;0;Create;True;0;0;False;0;1,0.6598378,0.05147058,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-1101.465,2782.908;Float;False;Property;_Color15;Color 15;14;0;Create;True;0;0;False;0;1,0,0,0.503;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-1100.469,2578.59;Float;False;Property;_Color14;Color 14;13;0;Create;True;0;0;False;0;0.3657548,0.4809159,0.9044118,0.616;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-1099.256,2386.722;Float;False;Property;_Color13;Color 13;12;0;Create;True;0;0;False;0;0.3455882,0.05590397,0.2297145,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;21;-1126.923,2129.295;Float;False;Property;_Color12;Color 12;11;0;Create;True;0;0;False;0;0.1397059,0.4701825,0.5,0.616;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-1156.896,1560.213;Float;False;Property;_Color9;Color 9;8;0;Create;True;0;0;False;0;0,0.859,0.353,0.497;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;20;-1151.505,1915.588;Float;False;Property;_Color11;Color 11;10;0;Create;True;0;0;False;0;0.4505191,0.4558824,0.06704153,0.684;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-1155.468,1735.796;Float;False;Property;_Color10;Color 10;9;0;Create;True;0;0;False;0;0.303,0.503,0,0.241;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;33;-883.738,1017.461;Float;True;ColorShartSlot;-1;;7;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;36;-861.3791,1479.35;Float;True;ColorShartSlot;-1;;9;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;31;-889.2806,578.1581;Float;True;ColorShartSlot;-1;;5;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;3;-920.4946,-316.7903;Float;True;ColorShartSlot;-1;;1;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;37;-857.7004,1704.987;Float;True;ColorShartSlot;-1;;10;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;34;-875.6722,1222.434;Float;True;ColorShartSlot;-1;;8;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;43;-826.4865,2584.346;Float;True;ColorShartSlot;-1;;14;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;30;-906.8862,327.4857;Float;True;ColorShartSlot;-1;;4;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;42;-830.165,2374.298;Float;True;ColorShartSlot;-1;;13;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;38;-854.3907,1913.169;Float;True;ColorShartSlot;-1;;11;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;26;-916.816,-91.15295;Float;True;ColorShartSlot;-1;;2;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;39;-847.7707,2123.626;Float;True;ColorShartSlot;-1;;12;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;45;-827.6922,2998.531;Float;True;ColorShartSlot;-1;;16;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;29;-913.5062,123.9578;Float;True;ColorShartSlot;-1;;3;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;44;-826.8495,2791.331;Float;True;ColorShartSlot;-1;;15;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;32;-885.602,803.7955;Float;True;ColorShartSlot;-1;;6;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-414.6628,2587.227;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-542.853,16.40221;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-483.7376,1812.542;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-511.6391,911.3508;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;38.72588,1414.863;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;579.5587,1808.014;Float;False;Property;_Smoothness;Smoothness;16;0;Create;True;0;0;False;0;0.5953899;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;607.6827,1574.874;Float;True;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;883.8115,1270.785;Float;False;Property;_Metallic;Metallic;17;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;1007.758,1622.633;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1388.965,947.3503;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Malbers/Color4x4v2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;38;16;0
WireConnection;36;38;18;0
WireConnection;31;38;14;0
WireConnection;3;38;2;0
WireConnection;37;38;19;0
WireConnection;34;38;17;0
WireConnection;43;38;23;0
WireConnection;30;38;13;0
WireConnection;42;38;22;0
WireConnection;38;38;20;0
WireConnection;26;38;11;0
WireConnection;39;38;21;0
WireConnection;45;38;25;0
WireConnection;29;38;12;0
WireConnection;44;38;24;0
WireConnection;32;38;15;0
WireConnection;41;0;42;0
WireConnection;41;1;43;0
WireConnection;41;2;44;0
WireConnection;41;3;45;0
WireConnection;27;0;3;0
WireConnection;27;1;26;0
WireConnection;27;2;29;0
WireConnection;27;3;30;0
WireConnection;40;0;36;0
WireConnection;40;1;37;0
WireConnection;40;2;38;0
WireConnection;40;3;39;0
WireConnection;35;0;31;0
WireConnection;35;1;32;0
WireConnection;35;2;33;0
WireConnection;35;3;34;0
WireConnection;28;0;27;0
WireConnection;28;1;35;0
WireConnection;28;2;40;0
WireConnection;28;3;41;0
WireConnection;5;0;28;0
WireConnection;7;0;5;0
WireConnection;7;1;8;0
WireConnection;0;0;28;0
WireConnection;0;3;6;0
WireConnection;0;4;7;0
ASEEND*/
//CHKSM=60818E43AE68147B79B0A2293388BA140DDB3F67