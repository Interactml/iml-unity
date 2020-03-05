// original source from: http://wiki.unity3d.com/index.php/MirrorReflection4
Shader "FX/MirrorReflection"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_ReflectionTexLeft("_ReflectionTexLeft", 2D) = "white" {}
		_ReflectionTexRight("_ReflectionTexRight", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 refl : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			float4 _MainTex_ST;
			v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(pos);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				o.refl = ComputeScreenPos(o.pos);
				return o;
			}
			sampler2D _MainTex;
			sampler2D _ReflectionTexLeft;
			sampler2D _ReflectionTexRight;
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 refl;
				if (unity_StereoEyeIndex == 0) refl = tex2Dproj(_ReflectionTexLeft, UNITY_PROJ_COORD(i.refl));
				else refl = tex2Dproj(_ReflectionTexRight, UNITY_PROJ_COORD(i.refl));
				return tex * refl;
			}
			ENDCG
		}
	}
}