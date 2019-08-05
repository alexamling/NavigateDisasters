
Shader "RealisticWater/WaterMobile" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ReflectionColor ("Reflection Color", Color) = (1,1,1,1)
	_FadeColor("Fade Color", Color) = (.9,.9,1,1)
	_FadeDepth("Fade Depth", Float) = 1
	_Wave1 ("Wave1 Distortion Texture", 2D) = "bump" {}
	_Wave2 ("Wave2 Distortion Texture", 2D) = "bump" {}
	_Cube("Reflection Map", Cube) = "" {}
	_Direction ("Waves Direction 1 & 2", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_FPOW("FPOW Fresnel", float) = 5.0
    _R0("R0 Fresnel", float) = 0.05
	_OffsetFresnel("Offset Fresnel", float) = 0.1
	
	_Distortion  ("Distortion", float) = 500
	_DistortionVert  ("Per Vertex Distortion", Float) = 1
	
	_GAmplitude ("Wave Amplitude", Vector) = (0.1 ,0.3, 0.2, 0.15)
	_GFrequency ("Wave Frequency", Vector) = (0.6, 0.5, 0.5, 1.8)
	_GSteepness ("Wave Steepness", Vector) = (1.0, 2.0, 1.5, 1.0)
	_GSpeed ("Wave Speed", Vector) = (-0.23, -1.25, -3.0, 1.5)
	_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.5, 0.85, 0.25)
	_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)	

	_WaveScale("Waves Scale", float) = 1
	_TexturesScale("Textures Scale", Float) = 1
}

Category {

	Tags { "Queue"="Transparent" "RenderType"="Transparent" }
	Cull Off ZWrite Off
	
	SubShader {
 		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Pass {
		
			
			
CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma glsl_no_auto_normalization
#pragma target 3.0
#pragma multi_compile _ ripples_on
//#pragma glsl

#include "UnityCG.cginc"
fixed _WaveScale;
float _TexturesScale;
fixed4 _Color;
fixed4 _FadeColor;
fixed4 _GlareColor;
half _FadeDepth;
fixed4 _ReflectionColor;
sampler2D _GrabTexture;
fixed4 _GrabTexture_TexelSize;
sampler2D _Wave1;
sampler2D _Wave2;
samplerCUBE _Cube;   
fixed4 _Direction;
fixed _FPOW;
fixed _R0;
fixed _OffsetFresnel;
fixed _Distortion;
fixed _DistortionVert;
sampler2D _ReflectionTex;
fixed _Bias;
fixed _Scale;
fixed _Power;
fixed _Shininess;
fixed4 _LightColor0; 

fixed4 _GAmplitude;
fixed4 _GFrequency;
fixed4 _GSteepness; 									
fixed4 _GSpeed;					
fixed4 _GDirectionAB;		
fixed4 _GDirectionCD;

fixed4 _Wave1_ST;
fixed4 _Wave2_ST;
fixed4 _Wave3_ST;

sampler2D _WaterDisplacementTexture;

float4x4 _projectiveMatrWaves;

struct appdata_t {
	float4 vertex : POSITION;
	fixed2 texcoord: TEXCOORD0;
	fixed3 normal : NORMAL;
};

struct v2f {
	half4 vertex : POSITION;
	half4 uvgrab : TEXCOORD0;
	fixed4 uvWave12 : TEXCOORD1;
	fixed4 offset : TEXCOORD2;
	#if ripples_on
	fixed ripple : TEXCOORD3;
	#endif
	half4 screenPos : TEXCOORD4;
	half2 fadeDepth : TEXCOORD5;
};


v2f vert (appdata_t v)
{
	v2f o;
	
	float2 time1 = fixed2(fmod(_Time.x*_Direction.x, 1), fmod(_Time.x*_Direction.y, 1));
	float2 time2 = fixed2(fmod(_Time.x*_Direction.z, 1), fmod(_Time.x*_Direction.w, 1));
	
	float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
	half2 scaleeUv = -posWorld.xz / _TexturesScale;
	o.uvWave12.xy = scaleeUv * _Wave1_ST.xy + _Wave1_ST.w  + time1;
	o.uvWave12.zw = scaleeUv * _Wave2_ST.xy + _Wave2_ST.w  + time2;
	

	//--------------------Gerstner waves-------------
	half2 vtxForAni = posWorld.xz / _WaveScale; 	

	half3 offsets;
	half4 AB = _GSteepness.xxyy * _GAmplitude.xxyy * _GDirectionAB.xyzw;
	fixed4 CD = _GSteepness.zzww * _GAmplitude.zzww * _GDirectionCD.xyzw;
		
	half4 dotABCD = _GFrequency.xyzw * half4(dot(_GDirectionAB.xy, vtxForAni), dot(_GDirectionAB.zw, vtxForAni), dot(_GDirectionCD.xy, vtxForAni), dot(_GDirectionCD.zw,  vtxForAni));
	half4 TIME = fmod(_Time.y * _GSpeed, 6.2831);
		
	half4 COS = cos (dotABCD + TIME);
	half4 SIN = sin (dotABCD + TIME);
		
	offsets.x = dot(COS, half4(AB.xz, CD.xz));
	offsets.z = dot(COS, half4(AB.yw, CD.yw));
	offsets.y = dot(SIN, _GAmplitude);

	//------------------------------------------------


	
	#if ripples_on
	half2 tileableUv = mul(_projectiveMatrWaves,v.vertex).xz;;
	half2 texDisp = tex2Dlod(_WaterDisplacementTexture, half4(tileableUv,0,0)).rg;
	half displ = texDisp.r - texDisp.g;
	texDisp.g = -texDisp.g;
	v.vertex.y += displ;
	o.ripple = clamp(displ  +  displ * displ*2, 0, 0.5) ;
	
	#endif

	v.vertex.xyz += offsets;	
	float4 oPos = UnityObjectToClipPos(v.vertex);	

	#if UNITY_UV_STARTS_AT_TOP
	fixed scale = -1.0;
	#else
	fixed scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(oPos.x, oPos.y*_ProjectionParams.x) + oPos.w) * 0.5;
	o.uvgrab.zw = oPos.w;
	o.uvgrab.xy += (offsets.xz + offsets.y*offsets.y)/_DistortionVert;
	
	half3 normWorld = normalize(mul((fixed3x3)(unity_ObjectToWorld), v.normal).xyz);


	o.offset.xy = _Distortion*fixed2(0.001, 0.001);
	o.offset.zw = o.offset.xy/30;
	o.offset.xy = o.offset.xy*o.offset.xy*o.offset.xy;
	
	o.vertex = UnityObjectToClipPos(v.vertex);

	o.screenPos = ComputeScreenPos(o.vertex);
	COMPUTE_EYEDEPTH(o.screenPos.z);
	half3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
	o.fadeDepth.x = viewDir.y * _FadeDepth;
	o.fadeDepth.y = 1- dot(viewDir, half3(0, 1, 0));
	o.fadeDepth.y = o.fadeDepth.y * o.fadeDepth.y;
	return o;
}
sampler2D _CameraDepthTexture;

fixed4 frag( v2f i ) : COLOR
{	
	fixed2 normal1 = UnpackNormal(tex2D(_Wave1, i.uvWave12.xy)).rg;
	fixed2 normal2 = UnpackNormal(tex2D(_Wave2, i.uvWave12.zw + normal1)).rg;
	
	fixed2 offset = normal2 * normal2 * normal2 * i.offset.xy + normal2 * i.offset.zw;
	#if ripples_on
	offset += i.ripple;
	#endif
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	fixed3 grab = tex2Dproj(_GrabTexture, i.uvgrab).rgb;
	i.uvgrab.xy += offset * i.uvgrab.z * 2;
	fixed3 reflection = tex2Dproj(_ReflectionTex, i.uvgrab).rgb * _ReflectionColor.rgb;


	float fade = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos))));
	fade = saturate((fade - i.screenPos.z) * i.fadeDepth);
	fade = saturate(1 - ((exp(-fade)) * 2 - 1));
	//return fade;
	fixed3 col = (lerp(grab * _Color.rgb, _FadeColor * reflection, fade));
	col = lerp(col, reflection, i.fadeDepth.y);
	col = lerp(col, grab, 1-fade);
	return fixed4(col, 1);
}
ENDCG
		}
	}
	

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}
}