// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "RealisticWater/WaterPC" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_GlareColor ("Glare Color", Color) = (1,1,1,1)
	_FadeColor ("Fade Color", Color) = (.9,.9,1,1)
	_ReflectionColor ("Reflection Color", Color) = (1,1,1,1)
	_ReflectionBrightness("Reflection Brightness", Range(0, 1)) = 0.5 

	_Wave1 ("Wave1 Distortion Texture", 2D) = "bump" {}
	_Wave2 ("Wave2 Distortion Texture", 2D) = "bump" {}
	_Foam ("Foam Texture", 2D) = "white" {}
	_Direction ("Waves Direction 1 & 2", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_FoamDirection ("Foam Direction R & G Chanell", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_FPOW("FPOW Fresnel", Float) = 5.0
    _R0("R0 Fresnel", Float) = 0.05
	_OffsetFresnel("Offset Fresnel", Float) = 0.1
	
	_FoamIntensity("Foam Intensity", Float) = 1
	_FadeBlend  ("Fade Blend Foam", Float) = 1
	_FadeBlend2  ("Fade Blend Transparency", Float) = 1
	_FadeDepth  ("Fade Depth", Float) = 1
	_DepthTransperent  ("Depth Transperent", Range(0, 1)) = 0
	_Distortion  ("Distortion Normal", Float) = 400
	_DistortionVert  ("Per Vertex Distortion", Float) = 1
	_EdgeDistortion("EdgesDistortion", Float) = 1
	_Bias("Bias Glare", Float) = -1
	_Scale("Scale Glare", Float) = 10
	_Power("Power Glare", Float) = 2

	//_GerstnerIntensity("Per vertex displacement", Float) = 1.0
	_GAmplitude ("Wave Amplitude", Vector) = (0.1 ,0.3, 0.2, 0.15)
	_GFrequency ("Wave Frequency", Vector) = (0.6, 0.5, 0.5, 1.8)
	_GSteepness ("Wave Steepness", Vector) = (1.0, 2.0, 1.5, 1.0)
	_GSpeed ("Wave Speed", Vector) = (-0.23, -1.25, -3.0, 1.5)
	_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.5, 0.85, 0.25)
	_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)	
	_WaveScale("Waves Scale", Float) = 1
	_TexturesScale("Textures Scale", Float) = 1
}

Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off 
	ZWrite Off
	
	SubShader {

		GrabPass {							
			"_GrabTextureWater"
			//Tags { "LightMode" = "Always" "IgnoreProjector"="True" }
 		}
 		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		Pass {
			
CGPROGRAM

#pragma vertex vert 
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma multi_compile_fog
#pragma multi_compile ripples_off ripples_on
#include "UnityCG.cginc"
#pragma glsl

float4 _Color;
float4 _GlareColor;
float4 _FadeColor;
float4 _ReflectionColor;
float _ReflectionBrightness;
sampler2D _GrabTextureWater;
float4 _GrabTextureWater_TexelSize;
sampler2D _Wave1;
sampler2D _Wave2;
sampler2D _Foam;
float4 _Direction;
float4 _FoamDirection;
float _FPOW;
float _R0;
float _OffsetFresnel;
float _SunIntensity;
float _FoamIntensity;
float _FadeBlend;
float _FadeBlend2;
float _FadeDepth;
float _DepthTransperent;
float _Distortion;
float _DistortionVert;
float _Bias;
float _Scale;
float _Power;
sampler2D _ReflectionTex;
float4 _LightColor0; 
float _WaveScale;
float _TexturesScale;

//float _GerstnerIntensity;
float4 _GAmplitude;
float4 _GFrequency;
float4 _GSteepness; 									
float4 _GSpeed;					
float4 _GDirectionAB;		
float4 _GDirectionCD;

float4 _Wave1_ST;
float4 _Wave2_ST;
float4 _Foam_ST;

float _EdgeDistortion;

sampler2D _WaterDisplacementTexture;
float4x4 _projectiveMatrWaves;

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	float3 normal : NORMAL;
	float4 tangent  : TANGENT;
};

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float3 uvWave1 : TEXCOORD1;
	float3 uvWave2 : TEXCOORD2;
	float4 viewDir : TEXCOORD3;
	float4 screenPos : TEXCOORD4;
	float2 uvFoam : TEXCOORD5;	
	float4 uvgrabDefault : TEXCOORD6;
	float4 screenPosWithoutVert : TEXCOORD7;
	float4 screenPosRefl : TEXCOORD8;
	#if UNITY_VERSION >= 500
	UNITY_FOG_COORDS(98)
	#endif
};


v2f vert (appdata_t v)
{
	v2f o;
	float4 oPos = UnityObjectToClipPos(v.vertex);
	
	float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;

	//--------------------Gerstner waves-------------
	half2 vtxForAni = posWorld.xz /_WaveScale; 		

	half3 offsets;
	half4 AB = _GSteepness.xxyy * _GAmplitude.xxyy * _GDirectionAB.xyzw;
	half4 CD = _GSteepness.zzww * _GAmplitude.zzww * _GDirectionCD.xyzw;
		
	half4 dotABCD = _GFrequency.xyzw * half4(dot(_GDirectionAB.xy, vtxForAni), dot(_GDirectionAB.zw, vtxForAni), dot(_GDirectionCD.xy, vtxForAni), dot(_GDirectionCD.zw,  vtxForAni));
	half4 TIME = fmod(_Time.y * _GSpeed, 6.2831);
		
	half4 COS = cos (dotABCD + TIME);
	half4 SIN = sin (dotABCD + TIME);
		
	offsets.x = dot(COS, half4(AB.xz, CD.xz));
	offsets.z = dot(COS, half4(AB.yw, CD.yw));
	offsets.y = dot(SIN, _GAmplitude);

	//------------------------------------------------
	
	half2 tileableUv = mul(_projectiveMatrWaves,v.vertex).xz;
	half2 scaleeUv = -posWorld.xz / _TexturesScale;
	
	o.uvWave2.z = 0;
	#if ripples_on
	float2 texDisp = tex2Dlod(_WaterDisplacementTexture, float4(tileableUv,0,0)).rg;
	float2 displ = texDisp.r - texDisp.g;
	v.vertex.y += displ;
	o.uvWave2.z = (displ*displ*2 + displ) * 1.5 + offsets.y/10;
	#endif
	
	v.vertex.xyz += offsets;
	o.vertex = UnityObjectToClipPos(v.vertex);
	
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	
	//o.uvgrabDefault.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	//o.uvgrabDefault.zw = o.vertex.w;
	o.uvgrabDefault = ComputeGrabScreenPos(o.vertex);
	//o.uvgrabDefault.zw = o.uvgrabDefault.w;

	oPos += o.vertex*_DistortionVert;

	//o.uvgrab.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
	//o.uvgrab.zw = oPos.w;
	//#ifdef UNITY_SINGLE_PASS_STEREO
	//o.uvgrab.xy = TransformStereoScreenSpaceTex(o.uvgrab.xy, o.uvgrab.w);
	//#endif
	o.uvgrab = ComputeGrabScreenPos(oPos);
	//o.uvgrab.zw = o.uvgrab.w;

	float3 normWorld = normalize(mul((float3x3)(unity_ObjectToWorld), v.normal).xyz);

	float3 I = normalize(posWorld - _WorldSpaceCameraPos.xyz);
	float angle = dot(I, normWorld);
	if(angle > 0) angle = -1;
	o.viewDir.w = _Bias + _Scale * pow(abs(1.0 + angle), _Power);
	
	o.viewDir.xyz=normalize(WorldSpaceViewDir(o.vertex));
	//o.viewDir.xyz=posWorld;
	o.screenPos = ComputeScreenPos (oPos);
	//o.screenPos.zw = o.screenPos.w;
	o.screenPosWithoutVert = ComputeScreenPos (o.vertex);
	//o.screenPosWithoutVert.zw = o.screenPosWithoutVert.w;
	o.screenPosRefl = ComputeNonStereoScreenPos (oPos);
	//o.screenPosRefl.zw = o.screenPosRefl.w;
	float2 time1 = _Time.xx * _Direction.xy;
	float2 time2 = _Time.xx * _Direction.zw;
	float2 time3 = _Time.xx * _FoamDirection.xy;
	
	o.uvWave1.xy = scaleeUv * _Wave1_ST.xy + _Wave1_ST.w  + time1;
	o.uvWave2.xy = scaleeUv * _Wave2_ST.xy + _Wave2_ST.w  + time2;
	o.uvFoam = scaleeUv * _Foam_ST.xy + _Foam_ST.w  + time3;

	o.uvWave1.z = offsets.y;

	COMPUTE_EYEDEPTH (o.screenPos.z);
	#if UNITY_VERSION >= 500
	UNITY_TRANSFER_FOG(o, o.vertex);
	#endif
	return o;
}

sampler2D _CameraDepthTexture;

half4 frag( v2f i ) : COLOR
{	
	//o.uvWave1.z is WavesOffsetColor
	//o.uvWave2.z is RippleColor

	half4 grabDefault = tex2Dproj(_GrabTextureWater, i.uvgrabDefault);
	float sceneZDefault = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosWithoutVert))));
	
	half2 normal1 = UnpackNormal(tex2D(_Wave1, i.uvWave1.xy)).rg;
	half3 normal2 = UnpackNormal(tex2D(_Wave2, i.uvWave2.xy + normal1));
	half3 normal3 = UnpackNormal(tex2D(_Wave2, i.uvWave2.xy/2-_Time.xx * _Direction.zw + normal2.xy));
	half foam1 = tex2D(_Foam, i.uvFoam);

	half fresnel = saturate(_OffsetFresnel - dot(normal2, i.viewDir.xyz));
	fresnel = pow(fresnel, _FPOW);
	fresnel = _R0 + (1.0 - _R0) * fresnel;

	float2 offset = normal2.xy * _Distortion * half2(0.001, 0.001);
	#if ripples_on
	offset += clamp(i.uvWave2.z*normal1*4 +  i.uvWave2.z* i.uvWave2.z*5, 0, 0.4);
	//offset = clamp(-0.4, 0.4, offset);
	#endif
	float offsetFadeBlend =saturate ((sceneZDefault - i.screenPos.z + _EdgeDistortion));
	
	offset = offset*offset*offset  + offset/30 + normal1.xy * i.uvWave1.z/8;
	offset *= offsetFadeBlend * offsetFadeBlend;
	i.uvgrab.xy = offset * i.uvgrab.w + i.uvgrab.xy;

	float4 screenPosOffset;
	screenPosOffset.xy = offset * i.screenPos.w + i.screenPos.xy;
	screenPosOffset.zw = i.screenPos.zw;
	
	
	i.screenPosRefl.xy = (offset +  i.uvWave2.z/3) * i.screenPosRefl.w + i.screenPosRefl.xy;


	half4 reflection = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.screenPosRefl));
	
	reflection = lerp((reflection+_ReflectionBrightness)/2, reflection, 1-_LightColor0.w);
	#if ripples_on
	reflection -= i.uvWave2.z*_LightColor0.w/3;
	#endif
	
	
	half4 grab = tex2Dproj(_GrabTextureWater, i.uvgrab);

	half4 col = grab;

	float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPosOffset)));
	
	float deltaZ =  sceneZ-i.screenPos.z;
	float fadeDepth = saturate (_FadeDepth * deltaZ);
	float fadeBlend = saturate (_FadeBlend * deltaZ);
	float fadeBlend2 = saturate (_FadeBlend2 * deltaZ);
	
	if(sceneZ < i.screenPos.z)
	{	
		fadeDepth = saturate (_FadeDepth * (sceneZDefault - i.screenPos.z));
		fadeBlend = saturate (_FadeBlend * (sceneZDefault - i.screenPos.z));
		fadeBlend2 = saturate (_FadeBlend2 * (sceneZDefault - i.screenPos.z));
		col = grabDefault;
		grab = grabDefault;
	}
	fadeBlend = 1 - fadeBlend;
	
	half glare2 = lerp(normal3.r, normal3.g, normal2.x * i.viewDir.w);
	half4 glareCol = _LightColor0*_GlareColor * (glare2*glare2);
	glareCol = (glareCol*glareCol/2 + glareCol/2);
	col = lerp(col * _Color, lerp(col * _FadeColor + col * i.uvWave1.z *_DepthTransperent, _FadeColor*_LightColor0, _DepthTransperent), fadeDepth);
	half4 refl = reflection * _ReflectionColor * fresnel;
	
	col += glareCol;
	col += refl;
	col += col * saturate(i.uvWave2.z)/3;

	half4 foam = foam1 * _LightColor0 * _FoamIntensity;
	foam = (1-foam)*col + foam;

	col = lerp (col, foam, fadeBlend);
	col.rgb = lerp(grab.rgb, col.rgb, fadeBlend2);
	#if UNITY_VERSION >= 500
 	UNITY_APPLY_FOG(i.fogCoord, col);
	#endif
	//col.a = saturate(col.a);
	return col;
}
ENDCG
		}
	}
}

}