// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "RealisticWater/CausticMobile" {
		Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_ColorStrength ("Color strength", Float) = 1.0
	_CausticTex1 ("Caustic Texture1", 2D) = "white" {}
	_CausticMask ("Caustic Distortion Mask", 2D) = "white" {}
	_CausticDirection ("Caustic Direction 1 & 2", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_DistortionDirection ("Distortion Direction X Y", Vector) = (0.5 ,-0.5, 0, 0)
	_Distortion("Distortion", Float) = 5
	_Height("Height", Float) = 1
	}
	Subshader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Pass {
			Fog { Mode Off}
			Blend SrcAlpha One
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma target 3.0
			#include "UnityCG.cginc"
			#pragma glsl

			
			fixed4 _TintColor;
			fixed _ColorStrength;
			sampler2D _CausticTex1;
			sampler2D _CausticMask;
			fixed4 _CausticDirection;
			fixed4 _DistortionDirection;
			fixed _Distortion;
			fixed4 _LightColor0; 
			fixed _Height;

			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed2 texcoord: TEXCOORD0;
			};

			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed2 uv_CausticTex1 : TEXCOORD1;
				fixed2 uv_CausticTex2 : TEXCOORD2;
				fixed2 uv_CausticMask : TEXCOORD3;
				fixed3 worldPos : TEXCOORD4;
			};
			
			fixed4 _CausticTex1_ST;
			fixed4 _CausticMask_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				fixed2 time1 = fixed2(fmod(_Time.x*_CausticDirection.x, 1), fmod(_Time.x*_CausticDirection.y, 1));
				fixed2 time2 = fixed2(fmod(_Time.x*_CausticDirection.z, 1), fmod(_Time.x*_CausticDirection.w, 1));
				fixed2 time3 = fixed2(fmod(_Time.x*_DistortionDirection.x, 1), fmod(_Time.x*_DistortionDirection.y, 1));
				o.pos = UnityObjectToClipPos (v.vertex);
				o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
				o.worldPos.y = 1 - o.worldPos.y + _Height;
				o.uv_CausticTex1 = TRANSFORM_TEX(v.texcoord, _CausticTex1)  + time1;
				o.uv_CausticTex2 = TRANSFORM_TEX( v.texcoord, _CausticTex1) + time2;
				o.uv_CausticMask = TRANSFORM_TEX( v.texcoord, _CausticMask) + time3;
				return o;
			}
			
			
			

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 causticColor;
				fixed4 mask = tex2D (_CausticMask, i.uv_CausticMask);
				fixed4 caustic1 = tex2D (_CausticTex1, i.uv_CausticTex1 + mask.rg/_Distortion);
				fixed4 caustic2 = tex2D (_CausticTex1, i.uv_CausticTex2 + mask.rg/_Distortion);
				causticColor = lerp(fixed4(0,0,0,1), mask, caustic1.x*caustic2.x);
				fixed4 res = causticColor * _TintColor * _ColorStrength * i.worldPos.y * _LightColor0;
				return res;
			}
			ENDCG
		}
	}
}