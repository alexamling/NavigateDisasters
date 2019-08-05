// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "RealisticWater/CausticPC" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_ColorStrength ("Caustic strength", Float) = 1
	_WetStrength ("Wet strength", Range(0, 1)) = 0.8
	_CausticTex1 ("Caustic Texture1", 2D) = "white" {}
	_CausticTex2 ("Caustic Texture2", 2D) = "white" {}
	_CausticMask ("Caustic Distortion Mask", 2D) = "white" {}
	_FalloffTex ("FallOff", 2D) = "white" {}
	_WetTex ("WetTex", 2D) = "white" {}
	_CausticDirection ("Caustic Direction 1 & 2", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_DistortionDirection ("Distortion Direction X Y", Vector) = (0.5 ,-0.5, 0, 0)
	_Distortion("Distortion", Float) = 5
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			Fog { Mode Off}
			Blend SrcAlpha One
			Offset -1, -1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			fixed4 _TintColor;
			fixed _ColorStrength;
			fixed _RiplesStrength;
			sampler2D _CausticTex1;
			sampler2D _CausticTex2;
			sampler2D _CausticMask;
			float4 _CausticDirection;
			float4 _DistortionDirection;
			float _Distortion;
			sampler2D _FalloffTex;
			float4 _LightColor0; 
			float4x4 _projectiveMatrCausticScale;

			struct v2f {
				float4 vertex : POSITION;
				float4 uv_Offset : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float2 uv_CausticTex1 : TEXCOORD2;
				float2 uv_CausticTex2 : TEXCOORD3;
				float2 uv_CausticMask : TEXCOORD4;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			float4 _CausticTex1_ST;
			float4 _CausticTex2_ST;
			float4 _CausticMask_ST;

			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos (vertex);
				o.uv_Offset = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				half3 tileableUv = mul(_projectiveMatrCausticScale,(vertex));
				o.uv_CausticTex1 = TRANSFORM_TEX( tileableUv.xyz, _CausticTex1);
				o.uv_CausticTex2 = TRANSFORM_TEX( tileableUv.xyz, _CausticTex2);
				o.uv_CausticMask = TRANSFORM_TEX( tileableUv.xyz, _CausticMask);
				
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				
				half4 causticColor;
				fixed4 mask = tex2D (_CausticMask, i.uv_CausticMask + _Time.xx*_DistortionDirection.xy);
				fixed4 caustic1 = tex2D (_CausticTex1, i.uv_CausticTex1 + _Time.xx*_CausticDirection.xy+mask.rg/_Distortion);
				fixed4 caustic2 = tex2D (_CausticTex2, i.uv_CausticTex2 + _Time.xx*_CausticDirection.zw+mask.rg/_Distortion);
				
				causticColor = lerp(half4(0,0,0,1), mask+1, caustic1.x*(caustic2.x+caustic2.x));
				fixed4 res = lerp(half4(0,0,0,0), causticColor * _TintColor * _ColorStrength, texF.a)*_LightColor0;
				return res;
			}
			ENDCG
		}


		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile_fog
		
			fixed _WetStrength;
			sampler2D _WetTex;

			
			struct v2f {
				float4 uv_Offset : TEXCOORD0;
				float4 uv_WetTex : TEXCOORD1;
				float4 vertex : POSITION;
				#if UNITY_VERSION >= 500
				UNITY_FOG_COORDS(2)
				#endif
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos (vertex);
				o.uv_Offset = mul (unity_Projector, vertex);
				o.uv_WetTex = mul (unity_ProjectorClip, vertex);
				#if UNITY_VERSION >= 500
				UNITY_TRANSFER_FOG(o, o.vertex);
				#endif
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texF = tex2Dproj (_WetTex, UNITY_PROJ_COORD(i.uv_WetTex));
				fixed4 col = fixed4(1-texF.rgb, saturate(texF.a - _WetStrength));
				#if UNITY_VERSION >= 500
 				UNITY_APPLY_FOG(i.fogCoord, col);
				#endif
				return col;
				
			}
			ENDCG
		}
	}
}

