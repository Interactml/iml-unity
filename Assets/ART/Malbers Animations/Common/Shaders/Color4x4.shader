// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Color4x4"
{
	Properties
	{
		_Color1("Color 1", Color) = (1,0.1544118,0.1544118,0.291)
		_Color2("Color 2", Color) = (1,0.1544118,0.8017241,0.253)
		_Color3("Color 3", Color) = (0.2535501,0.1544118,1,0.541)
		_Color4("Color 4", Color) = (0.1544118,0.5451319,1,0.253)
		_Color5("Color 5", Color) = (0.9533468,1,0.1544118,0.553)
		_Color6("Color 6", Color) = (0.2720588,0.1294625,0,0.097)
		_Color7("Color 7", Color) = (0.1544118,0.6151115,1,0.178)
		_Color8("Color 8", Color) = (0.4849697,0.5008695,0.5073529,0.078)
		_Color9("Color 9", Color) = (0.3164301,0,0.7058823,0.134)
		_Color10("Color 10", Color) = (0.362069,0.4411765,0,0.759)
		_Color11("Color 11", Color) = (0.6691177,0.6691177,0.6691177,0.647)
		_Color12("Color 12", Color) = (0.5073529,0.1574544,0,0.128)
		_Color13("Color 13", Color) = (1,0.5586207,0,0.272)
		_Color14("Color 14", Color) = (0,0.8025862,0.875,0.047)
		_Color15("Color 15", Color) = (1,0,0,0.391)
		_Color16("Color 16", Color) = (0.4080882,0.75,0.4811866,0.134)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
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

		uniform float4 _Color1;
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
			float temp_output_3_0_g207 = 1.0;
			float temp_output_7_0_g207 = 4.0;
			float temp_output_9_0_g207 = 4.0;
			float temp_output_8_0_g207 = 4.0;
			float temp_output_3_0_g220 = 2.0;
			float temp_output_7_0_g220 = 4.0;
			float temp_output_9_0_g220 = 4.0;
			float temp_output_8_0_g220 = 4.0;
			float temp_output_3_0_g211 = 3.0;
			float temp_output_7_0_g211 = 4.0;
			float temp_output_9_0_g211 = 4.0;
			float temp_output_8_0_g211 = 4.0;
			float temp_output_3_0_g212 = 4.0;
			float temp_output_7_0_g212 = 4.0;
			float temp_output_9_0_g212 = 4.0;
			float temp_output_8_0_g212 = 4.0;
			float temp_output_3_0_g216 = 1.0;
			float temp_output_7_0_g216 = 4.0;
			float temp_output_9_0_g216 = 3.0;
			float temp_output_8_0_g216 = 4.0;
			float temp_output_3_0_g214 = 2.0;
			float temp_output_7_0_g214 = 4.0;
			float temp_output_9_0_g214 = 3.0;
			float temp_output_8_0_g214 = 4.0;
			float temp_output_3_0_g215 = 3.0;
			float temp_output_7_0_g215 = 4.0;
			float temp_output_9_0_g215 = 3.0;
			float temp_output_8_0_g215 = 4.0;
			float temp_output_3_0_g222 = 4.0;
			float temp_output_7_0_g222 = 4.0;
			float temp_output_9_0_g222 = 3.0;
			float temp_output_8_0_g222 = 4.0;
			float temp_output_3_0_g217 = 1.0;
			float temp_output_7_0_g217 = 4.0;
			float temp_output_9_0_g217 = 2.0;
			float temp_output_8_0_g217 = 4.0;
			float temp_output_3_0_g208 = 2.0;
			float temp_output_7_0_g208 = 4.0;
			float temp_output_9_0_g208 = 2.0;
			float temp_output_8_0_g208 = 4.0;
			float temp_output_3_0_g213 = 3.0;
			float temp_output_7_0_g213 = 4.0;
			float temp_output_9_0_g213 = 2.0;
			float temp_output_8_0_g213 = 4.0;
			float temp_output_3_0_g221 = 4.0;
			float temp_output_7_0_g221 = 4.0;
			float temp_output_9_0_g221 = 2.0;
			float temp_output_8_0_g221 = 4.0;
			float temp_output_3_0_g210 = 1.0;
			float temp_output_7_0_g210 = 4.0;
			float temp_output_9_0_g210 = 1.0;
			float temp_output_8_0_g210 = 4.0;
			float temp_output_3_0_g219 = 2.0;
			float temp_output_7_0_g219 = 4.0;
			float temp_output_9_0_g219 = 1.0;
			float temp_output_8_0_g219 = 4.0;
			float temp_output_3_0_g209 = 3.0;
			float temp_output_7_0_g209 = 4.0;
			float temp_output_9_0_g209 = 1.0;
			float temp_output_8_0_g209 = 4.0;
			float temp_output_3_0_g218 = 4.0;
			float temp_output_7_0_g218 = 4.0;
			float temp_output_9_0_g218 = 1.0;
			float temp_output_8_0_g218 = 4.0;
			float4 temp_output_155_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g207 - 1.0 ) / temp_output_7_0_g207 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g207 / temp_output_7_0_g207 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g207 - 1.0 ) / temp_output_8_0_g207 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g207 / temp_output_8_0_g207 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g220 - 1.0 ) / temp_output_7_0_g220 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g220 / temp_output_7_0_g220 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g220 - 1.0 ) / temp_output_8_0_g220 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g220 / temp_output_8_0_g220 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g211 - 1.0 ) / temp_output_7_0_g211 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g211 / temp_output_7_0_g211 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g211 - 1.0 ) / temp_output_8_0_g211 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g211 / temp_output_8_0_g211 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g212 - 1.0 ) / temp_output_7_0_g212 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g212 / temp_output_7_0_g212 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g212 - 1.0 ) / temp_output_8_0_g212 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g212 / temp_output_8_0_g212 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g216 - 1.0 ) / temp_output_7_0_g216 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g216 / temp_output_7_0_g216 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g216 - 1.0 ) / temp_output_8_0_g216 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g216 / temp_output_8_0_g216 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g214 - 1.0 ) / temp_output_7_0_g214 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g214 / temp_output_7_0_g214 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g214 - 1.0 ) / temp_output_8_0_g214 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g214 / temp_output_8_0_g214 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g215 - 1.0 ) / temp_output_7_0_g215 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g215 / temp_output_7_0_g215 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g215 - 1.0 ) / temp_output_8_0_g215 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g215 / temp_output_8_0_g215 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g222 - 1.0 ) / temp_output_7_0_g222 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g222 / temp_output_7_0_g222 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g222 - 1.0 ) / temp_output_8_0_g222 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g222 / temp_output_8_0_g222 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g217 - 1.0 ) / temp_output_7_0_g217 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g217 / temp_output_7_0_g217 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g217 - 1.0 ) / temp_output_8_0_g217 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g217 / temp_output_8_0_g217 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g208 - 1.0 ) / temp_output_7_0_g208 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g208 / temp_output_7_0_g208 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g208 - 1.0 ) / temp_output_8_0_g208 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g208 / temp_output_8_0_g208 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g213 - 1.0 ) / temp_output_7_0_g213 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g213 / temp_output_7_0_g213 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g213 - 1.0 ) / temp_output_8_0_g213 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g213 / temp_output_8_0_g213 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g221 - 1.0 ) / temp_output_7_0_g221 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g221 / temp_output_7_0_g221 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g221 - 1.0 ) / temp_output_8_0_g221 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g221 / temp_output_8_0_g221 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g210 - 1.0 ) / temp_output_7_0_g210 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g210 / temp_output_7_0_g210 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g210 - 1.0 ) / temp_output_8_0_g210 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g210 / temp_output_8_0_g210 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g219 - 1.0 ) / temp_output_7_0_g219 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g219 / temp_output_7_0_g219 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g219 - 1.0 ) / temp_output_8_0_g219 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g219 / temp_output_8_0_g219 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g209 - 1.0 ) / temp_output_7_0_g209 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g209 / temp_output_7_0_g209 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g209 - 1.0 ) / temp_output_8_0_g209 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g209 / temp_output_8_0_g209 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g218 - 1.0 ) / temp_output_7_0_g218 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g218 / temp_output_7_0_g218 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g218 - 1.0 ) / temp_output_8_0_g218 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g218 / temp_output_8_0_g218 ) ) * 1.0 ) ) ) ) ) );
			o.Albedo = temp_output_155_0.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = ( (temp_output_155_0).a * _Smoothness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
1927;29;1352;732;-843.2847;-811.6501;1.950566;True;True
Node;AmplifyShaderEditor.ColorNode;181;-218.8154,2174.284;Float;False;Property;_Color11;Color 11;10;0;Create;True;0;0;False;0;0.6691177,0.6691177,0.6691177,0.647;0.1985294,0.1664144,0.1664144,0.472;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;182;-220.2247,2417.44;Float;False;Property;_Color12;Color 12;11;0;Create;True;0;0;False;0;0.5073529,0.1574544,0,0.128;0,0.1460954,0.3161765,0.472;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;183;-224.4024,1681.061;Float;False;Property;_Color9;Color 9;8;0;Create;True;0;0;False;0;0.3164301,0,0.7058823,0.134;0.5367647,0.4258277,0.2802228,0.422;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;180;-232.3431,1940.419;Float;False;Property;_Color10;Color 10;9;0;Create;True;0;0;False;0;0.362069,0.4411765,0,0.759;0.2647059,0.2534304,0.2413495,0.484;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;218;-229.103,3176.23;Float;False;Property;_Color15;Color 15;14;0;Create;True;0;0;False;0;1,0,0,0.391;0.1985294,0.1664144,0.1664144,0.472;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;217;-264.3738,3419.386;Float;False;Property;_Color16;Color 16;15;0;Create;True;0;0;False;0;0.4080882,0.75,0.4811866,0.134;0,0.1460954,0.3161765,0.472;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;213;-234.6901,2683.007;Float;False;Property;_Color13;Color 13;12;0;Create;True;0;0;False;0;1,0.5586207,0,0.272;1,0.5586207,0,0.272;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;214;-242.6307,2942.365;Float;False;Property;_Color14;Color 14;13;0;Create;True;0;0;False;0;0,0.8025862,0.875,0.047;0.2647059,0.2534304,0.2413495,0.484;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-194.2135,166.9271;Float;False;Property;_Color3;Color 3;2;0;Create;True;0;0;False;0;0.2535501,0.1544118,1,0.541;0.4264706,0.4264706,0.4264706,0.428;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;154;-195.6228,411.2479;Float;False;Property;_Color4;Color 4;3;0;Create;True;0;0;False;0;0.1544118,0.5451319,1,0.253;0.4264706,0.4264706,0.4264706,0.422;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-199.8005,-326.2955;Float;False;Property;_Color1;Color 1;0;0;Create;True;0;0;False;0;1,0.1544118,0.1544118,0.291;0.1102941,0.1102941,0.1102941,0.216;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;150;-207.7412,-66.93771;Float;False;Property;_Color2;Color 2;1;0;Create;True;0;0;False;0;1,0.1544118,0.8017241,0.253;0.4264706,0.4264706,0.4264706,0.303;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;157;-182.3802,1181.25;Float;False;Property;_Color7;Color 7;6;0;Create;True;0;0;False;0;0.1544118,0.6151115,1,0.178;0.2205882,0.2189662,0.2189662,0.491;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;158;-183.7895,1424.406;Float;False;Property;_Color8;Color 8;7;0;Create;True;0;0;False;0;0.4849697,0.5008695,0.5073529,0.078;0,0,0,0.459;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;159;-187.9672,688.0273;Float;False;Property;_Color5;Color 5;4;0;Create;True;0;0;False;0;0.9533468,1,0.1544118,0.553;0.6544118,0.5677984,0.4378785,0.409;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;156;-195.9079,947.3851;Float;False;Property;_Color6;Color 6;5;0;Create;True;0;0;False;0;0.2720588,0.1294625,0,0.097;0.1911765,0.1883651,0.1883651,0.441;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;162;133.8517,1429.247;Float;True;ColorShartSlot;-1;;222;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;186;96.90227,2179.125;Float;True;ColorShartSlot;-1;;213;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;153;122.0185,414.924;Float;True;ColorShartSlot;-1;;212;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;163;127.7504,692.868;Float;True;ColorShartSlot;-1;;216;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;161;133.3375,1186.091;Float;True;ColorShartSlot;-1;;215;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;160;119.8096,952.2258;Float;True;ColorShartSlot;-1;;214;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;224;86.61465,3181.071;Float;True;ColorShartSlot;-1;;209;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;187;83.37437,1945.26;Float;True;ColorShartSlot;-1;;208;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;145;115.9171,-321.4549;Float;True;ColorShartSlot;-1;;207;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;151;121.5042,171.7677;Float;True;ColorShartSlot;-1;;211;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;216;81.02762,2687.848;Float;True;ColorShartSlot;-1;;210;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;185;97.41646,2422.281;Float;True;ColorShartSlot;-1;;221;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;149;107.9764,-62.09709;Float;True;ColorShartSlot;-1;;220;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;223;73.08682,2945.046;Float;True;ColorShartSlot;-1;;219;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;188;91.31517,1685.902;Float;True;ColorShartSlot;-1;;217;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;222;87.12894,3424.227;Float;True;ColorShartSlot;-1;;218;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;146;1539.255,777.6315;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;184;1537.758,1310.802;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;1539.944,1043.66;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;225;1534.365,1575.009;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;155;1964.993,1140.165;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;166;1887.168,1900.592;Float;False;Property;_Smoothness;Smoothness;16;0;Create;True;0;0;False;0;1;0.081;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;227;1935.602,1617.235;Float;True;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;2014.597,1413.642;Float;False;Property;_Metallic;Metallic;17;0;Create;True;0;0;False;0;0;0.053;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;212;2229.031,1787.579;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2469.067,1277.475;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Malbers/Color4x4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;162;38;158;0
WireConnection;186;38;181;0
WireConnection;153;38;154;0
WireConnection;163;38;159;0
WireConnection;161;38;157;0
WireConnection;160;38;156;0
WireConnection;224;38;218;0
WireConnection;187;38;180;0
WireConnection;145;38;23;0
WireConnection;151;38;152;0
WireConnection;216;38;213;0
WireConnection;185;38;182;0
WireConnection;149;38;150;0
WireConnection;223;38;214;0
WireConnection;188;38;183;0
WireConnection;222;38;217;0
WireConnection;146;0;145;0
WireConnection;146;1;149;0
WireConnection;146;2;151;0
WireConnection;146;3;153;0
WireConnection;184;0;188;0
WireConnection;184;1;187;0
WireConnection;184;2;186;0
WireConnection;184;3;185;0
WireConnection;164;0;163;0
WireConnection;164;1;160;0
WireConnection;164;2;161;0
WireConnection;164;3;162;0
WireConnection;225;0;216;0
WireConnection;225;1;223;0
WireConnection;225;2;224;0
WireConnection;225;3;222;0
WireConnection;155;0;146;0
WireConnection;155;1;164;0
WireConnection;155;2;184;0
WireConnection;155;3;225;0
WireConnection;227;0;155;0
WireConnection;212;0;227;0
WireConnection;212;1;166;0
WireConnection;0;0;155;0
WireConnection;0;3;165;0
WireConnection;0;4;212;0
ASEEND*/
//CHKSM=7E8144934B26576E6D385FF062129D49EC3B96AF