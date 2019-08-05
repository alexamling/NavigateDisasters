Shader "Custom/SphericalMask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base Map", 2D) = "white" {}
		_FireMap("Fire Map", 2D) = "white" {}
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
		sampler2D _FireMap;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_FireMap;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		// Spherical mask
		int _NumInteractions = 0;
		float _InteractionPoints[500]; // read in as [x, y, z, radius, falloff]

		int _NumDestructions = 0;
		float _DestructionPoints[300]; // read in as [x, z, radius]

 		half _Radius;
		half _Softness;
		half _Scale;
		
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			// grayscale
			half grayscale = (c.r + c.g + c.b) * 0.333;
			fixed3 c_g = fixed3(grayscale, grayscale, grayscale);
            
			// find distance to nearest interaction point
			half greatestSum = 0;
			half sum = 0;
			half dist = 0;
			float3 interactionPoint;

			for (int i = 0; i < _NumInteractions * 5; i += 5) {
				interactionPoint = float3(_InteractionPoints[i], _InteractionPoints[i + 1], _InteractionPoints[i + 2]);

				dist = distance(IN.worldPos, interactionPoint);

				sum = saturate((dist - _InteractionPoints[i + 3]) / -_InteractionPoints[i + 4]);

				if (sum > greatestSum) {
					greatestSum = sum;
				}
			}

			// apply color according to distance from point
			fixed4 lerpColor = lerp(fixed4(c_g, 1), c, greatestSum);
			o.Albedo = lerpColor.rgb;

			// add the fire
			o.Albedo += tex2D(_FireMap, IN.uv_FireMap).r * greatestSum * fixed3(1,0,0);

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
