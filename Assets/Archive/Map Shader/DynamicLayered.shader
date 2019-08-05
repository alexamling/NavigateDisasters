// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DynamicLayered"
{
    Properties
    {
		[Toggle] _IsHidden ("Hide Bottom Layer", float) = 0
        _TopLayer ("Top Layer", 2D) = "grey" {}
		_BottomLayer("Bottom Layer", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "Render" = "Transparent" "IgnoreProjector" = "True"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
			#pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
            };

			// read in interaction points in an array
			bool _IsHidden = true;
			int _NumInteractions = 0;
			float _InteractionPoints[256]; // read in as [x, y, z, radius]

            sampler2D _TopLayer;
            float4 _TopLayer_ST;

			sampler2D _BottomLayer;
			float4 _BottomLayer_ST;

            v2f vert (appdata v)
			{
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _TopLayer);

                return o;
            }

            fixed4 frag (v2f input) : SV_Target
            {
				float3 pos = input.worldPos;
				
				float smallestDist = 1.0f;

				for (int i = 0; i < _NumInteractions * 4; i += 4) {

					float3 interactionPoint = float3(_InteractionPoints[i], _InteractionPoints[i + 1], _InteractionPoints[i + 2]);

					float dist = distance(pos, interactionPoint) / _InteractionPoints[i + 3];
					
					if (smallestDist > dist) {
						smallestDist = dist;
					}
				}

				smallestDist *= smallestDist * smallestDist * smallestDist;

                // sample the texture
				fixed4 col = tex2D(_TopLayer, input.uv) * (smallestDist);
				col = col + tex2D(_BottomLayer, input.uv) * (1 - smallestDist);
				


				col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}
