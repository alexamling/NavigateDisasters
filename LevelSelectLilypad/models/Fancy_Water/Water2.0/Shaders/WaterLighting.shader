// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "RealisticWater/WaterLighting" {
	Properties {
	_Color ("Color", Color) = (0.5,0.5,0.5,0.5)
	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
    _Shininess ("Shininess", Float) = 10
	_Wave1 ("Wave1 Texture", 2D) = "white" {}
	_Wave2 ("Wave2 Texture", 2D) = "white" {}
	_Direction ("Waves Direction 1 & 2", Vector) = (1.0 ,1.0, -1.0, -1.0)
	_FPOW("FPOW Fresnel", Float) = 5.0
    _R0("R0 Fresnel", Float) = 0.05
	_OffsetFresnel("Offset Fresnel", Float) = 0.1
	_WorldLightDir ("Specular light direction", Vector) = (0.0, 0.1, -0.5, 0.0)
	_TexturesScale("Textures Scale", Float) = 1
}

SubShader {
        Tags { "Queue"="Transparent"  "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 400

CGPROGRAM

#pragma surface surf BlinnPhong vertex:vert noambient decal:add exclude_path:prepass 
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest

//#pragma glsl


float _Shininess;
sampler2D _Wave1;
sampler2D _Wave2;
float4 _Color;
float4 _Direction;
float _FPOW;
float _R0;
float _OffsetFresnel;
float4 _WorldLightDir;
float4x4 _projectiveMatrLightScale;
float4 _Wave1_ST;
float4 _Wave2_ST;
float _TexturesScale;

struct Input {
		float2 Wave1 : TEXCOORD0;
		float2 Wave2 : TEXCOORD1;
		float3 viewDir;
		float4 proj : TEXCOORD2;
	};


	void vert (inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input,o);
		float4 oPos = UnityObjectToClipPos(v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
		#else
			float scale = 1.0;
		#endif
		o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
		o.proj.zw = oPos.zw;
		half2 scaleeUv = -mul(unity_ObjectToWorld,(v.vertex)).xz * _TexturesScale;
		o.Wave1 =  scaleeUv * _Wave1_ST.xy + _Wave1_ST.w; 
		o.Wave2 =  scaleeUv * _Wave2_ST.xy + _Wave2_ST.w; 
	}
 
	void surf (Input IN, inout SurfaceOutput o) {
        o.Specular = _Shininess;
		
		float3 normal1 = UnpackNormal(tex2D(_Wave1, IN.Wave1 + _Time.xx * _Direction.xy));
		float3 normal2 = UnpackNormal(tex2D(_Wave2,  IN.Wave2 + normal1.rg + _Time.xx * _Direction.zw));
		o.Normal = normal2;
		
		float fresnel = saturate(1.0 - dot(o.Normal, normalize(IN.viewDir)));
        fresnel = pow(fresnel, _FPOW);
        fresnel = _R0 + (1.0 - _R0) * fresnel;
		float strColor;
		//if(o.Normal.r > 0) strColor = fresnel * o.Normal.r;
		//else strColor = fresnel * -o.Normal.r;
		strColor = fresnel * o.Normal.r;

		strColor = pow(strColor*10, 1.5);
		o.Albedo = saturate(strColor * _Color.rgb*_Color.a);
	    o.Gloss = 1;
        o.Alpha = _Color.a;
	}
ENDCG		}
		
}