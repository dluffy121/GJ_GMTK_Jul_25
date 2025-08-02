Shader "Custom/SpaceBackground"
{
    Properties
    {
        _TimeSpeed ("Time Speed", Float) = 1
        _PlanetCount ("Planet Count", Int) = 3
        _StarCount ("Star Count", Int) = 100
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TimeSpeed;
            int _PlanetCount;
            int _StarCount;
            float4 _MainTex_ST;

            float _TimeValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Simple random function
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            // Star blinking
            float starBlink(float2 uv, float seed, float time)
            {
                float blink = abs(sin(time * (1.0 + seed * 2.0) + seed * 10.0));
                float star = smoothstep(0.0, 0.01, 1.0 - length(uv - seed));
                return blink * star;
            }

            // Planet movement
            float3 planet(float2 uv, float2 center, float radius, float time, float3 color)
            {
                float2 pos = center + float2(sin(time * 0.2 + center.x * 10.0), cos(time * 0.2 + center.y * 10.0)) * 0.2;
                float d = length(uv - pos);
                float p = smoothstep(radius, radius - 0.01, d);
                return color * p;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _TimeSpeed;
                float3 baseCol = tex2D(_MainTex, uv).rgb;
                float3 col = baseCol * 0.7 + float3(0.05, 0.07, 0.15) * 0.3; // blend texture and base space color

                // Nebula color movement
                float3 nebula = float3(sin(time + uv.x * 3.0), cos(time + uv.y * 2.0), sin(time * 0.5 + uv.x * uv.y * 5.0));
                nebula = nebula * 0.08 + 0.08;
                col += nebula;

                // Planets
                for (int p = 0; p < _PlanetCount; ++p)
                {
                    float2 center = float2(rand(float2(p, p + 1)), rand(float2(p + 2, p + 3)));
                    float radius = 0.08 + rand(float2(p + 4, p + 5)) * 0.05;
                    float3 pColor = float3(rand(float2(p + 6, p + 7)), rand(float2(p + 8, p + 9)), rand(float2(p + 10, p + 11)));
                    col = lerp(col, planet(uv, center, radius, time, pColor * 0.5), 0.12);
                }

                // Stars
                for (int s = 0; s < _StarCount; ++s)
                {
                    float seed = rand(float2(s * 13.0, s * 17.0));
                    float starVal = starBlink(uv, seed, time);
                    col += float3(1,1,1) * starVal * 0.07;
                }

                col = saturate(col); // Clamp color to [0,1]
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
