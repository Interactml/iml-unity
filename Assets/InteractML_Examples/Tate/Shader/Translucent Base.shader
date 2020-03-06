//Shader by larsbertram69 https://github.com/larsbertram69/Lux-2.02-Personal/blob/master/Lux%20Shaders/Lux%20custom%20Translucent%20Shaders/Translucent%20DoubleSided.shader
Shader "Lux/Translucent Lighting/DoubleSided" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_SpecColor("Specular Color", Color) = (0.2,0.2,0.2)

			// Lux translucent lighting properties
			[Space(4)]
			[Header(Translucent Lighting ______________________________________________________)]
			[Space(4)]
			[NoScaleOffset] _TranslucencyOcclusion("Lokal Thickness (B) Occlusion (G)", 2D) = "white" {}
			_TranslucencyStrength("Translucency Strength", Range(0,1)) = 1
			_ScatteringPower("Scattering Power", Range(0,8)) = 4

	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

				//	!!!! SINGLE SIDED GEOMETRY: Make the shader not cull the backfaces
					Cull Off

					CGPROGRAM

				//	Lux: declare "addshadow" to get proper lighting/shadows in forward
					#pragma surface surf LuxTranslucentSpecular fullforwardshadows addshadow

					#pragma multi_compile __ LUX_AREALIGHTS
					#include "../Lux Core/Lux Config.cginc"
					#include "../Lux Core/Lux Lighting/LuxTranslucentPBSLighting.cginc"
					#pragma target 3.0

					struct Input {
						float2 uv_MainTex;			// As we do not include "LuxStructs.cginc" and "LuxParallax.cginc" we can use "uv_MainTex"
						float FacingSign : FACE;	// needed as we support single sided geometry		
					};

					sampler2D _MainTex;
					sampler2D _BumpMap;
					sampler2D _TranslucencyOcclusion;
					half _Glossiness;
					fixed4 _Color;

					void surf(Input IN, inout SurfaceOutputLuxTranslucentSpecular o) {
						// !!!! SINGLE SIDED GEOMETRY: As we use double sided rendering (and single sided geometry) we might have to flip IN.viewDir and o.Normal
						float3 flipFacing = float3(1.0, 1.0, IN.FacingSign);

						fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
						o.Albedo = c.rgb;
						o.Specular = _SpecColor;
						o.Smoothness = _Glossiness;
						o.Alpha = c.a;

						//	!!!! SINGLE SIDED GEOMETRY: Correct final normal direction
							o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)) * flipFacing;

							//	Lux: Occlusion and Translucency are stored in a combined map
								half4 transOcclusion = tex2D(_TranslucencyOcclusion, IN.uv_MainTex);
								o.Occlusion = transOcclusion.g;
								//	Lux: Write translucent lighting parameters to the output struct 
									o.Translucency = transOcclusion.b * _TranslucencyStrength;
									o.ScatteringPower = _ScatteringPower;
								}
								ENDCG
			}
				// No fallback – we have to declare "addshadow" to get proper lighting
				//FallBack "Diffuse"
}