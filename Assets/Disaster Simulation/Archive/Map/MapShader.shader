// This shader combines the outputs of all active managers to present the player with a complete view of the map

Shader "Custom/MapShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ViewMap("Unit View", 2D) = "black" {}
		_FireMap("Fire Map", 2D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _ViewMap;
		sampler2D _FireMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			half grayscale = (c.r + c.g + c.b) * 0.333;
			fixed3 c_g = fixed3(grayscale, grayscale, grayscale);

			fixed4 lerpColor = lerp(fixed4(c_g, 1), c, tex2D(_ViewMap, IN.uv_MainTex).r);
			o.Albedo = lerpColor.rgb;

			o.Albedo += tex2D(_FireMap, IN.uv_MainTex).r * fixed3(1, 0, 0) * tex2D(_ViewMap, IN.uv_MainTex).r;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
