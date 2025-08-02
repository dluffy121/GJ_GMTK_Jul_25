Shader "Custom/ProceduralCloudsAndStars"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _ColorMultiplier ("Color Multiplier", Color) = (1,1,1,1)
        _ColorMultiplier2 ("Color Multiplier 2", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(0,3)) = 1.2
        _CloudsResolution ("Clouds Resolution", Range(0,20)) = 10.0
        _CloudsIntensity ("Clouds Intensity", Range(-0.06, 0.5)) = 0.0
        _Waveyness ("Waveyness", Range(0,10)) = 0.5
        _Fragmentation ("Fragmentation", Range(0,100)) = 7.0
        _Distortion ("Distortion", Range(0,10)) = 1.5
        _CloudsAlpha ("Clouds Alpha", Range(0.4, 0.6)) = 0.5
        _Movement ("Movement", Range(0.7, 2.0)) = 1.3
        _Blur ("Blur", Range(0,10)) = 1.4
        _Blur2 ("Blur2", Range(0,0.01)) = 0.01
        _TimeScaleFactor ("Time Scale Factor", Float) = 0.04
        _StarsOn ("Stars On", Float) = 1
        _StarsFlicker ("Stars Flicker", Float) = 1
        _BGSpeed ("Background Speed", Float) = 0.1
        _MidSpeed ("Midground Speed", Float) = 0.5
        _FGSpeed ("Foreground Speed", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float4 _ColorMultiplier;
            float4 _ColorMultiplier2;
            float _Brightness, _CloudsResolution, _CloudsIntensity, _Waveyness;
            float _Fragmentation, _Distortion, _CloudsAlpha, _Movement;
            float _Blur, _Blur2, _TimeScaleFactor;
            float _StarsOn, _StarsFlicker;
            float _BGSpeed, _MidSpeed, _FGSpeed;

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

            float localTime()
            {
                return _Time.y * _TimeScaleFactor;
            }

            float noise(float2 x)
            {
                return tex2D(_NoiseTex, x * _Blur2).r;
            }

            float noisey(float2 y)
            {
                return tex2D(_NoiseTex, y * _Blur2).g;
            }

            float2 gradn(float2 p)
            {
                float ep = 0.09;
                float gradx = noise(float2(p.x + ep, p.y)) - noise(float2(p.x - ep, p.y));
                float grady = noisey(float2(p.x, p.y + ep)) - noisey(float2(p.x, p.y - ep));
                return float2(gradx, grady);
            }

            float2 rotate(float theta, float2 v)
            {
                float c = cos(theta);
                float s = sin(theta);
                return float2(c * v.x - s * v.y, s * v.x + c * v.y);
            }

            float flow(float2 p)
            {
                float z = 2.0;
                float rz = _CloudsIntensity;
                float2 bp = p;

                for (int i = 1; i < 7; ++i)
                {
                    p += localTime() * 0.6;
                    bp += localTime() * 5.9;
                    float2 gr = gradn(i * p * 0.34 + localTime());
                    gr = rotate(localTime() * 6.0 - (.05 * p.x + .03 * p.y) * 90.0, gr);
                    p += gr * _Waveyness;
                    rz += (sin(noise(p) * _Fragmentation) * _Distortion + _CloudsAlpha) / z;
                    p = lerp(bp, p, _Movement);
                    z *= _Blur;
                    p *= 2.0;
                    bp *= 1.9;
                }
                return rz;
            }

            float rand(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.uv * _CloudsResolution;
                p *= _CloudsResolution;

                float2 bg_p = p * _BGSpeed;
                float2 mid_p = p * _MidSpeed;
                float2 fg_p = p * _FGSpeed;

                float red = 0.2 * sin(_Time.y * 0.1);
                float blue = 0.1;
                float green = 0.1;

                float bg_rz = flow(bg_p);
                float mid_rz = flow(mid_p);
                float fg_rz = flow(fg_p);

                float3 col_bg = float3(red, blue, green) / bg_rz;
                float3 col_mid = float3(red, blue, green) / mid_rz;
                float3 col_fg = float3(red, blue, green) / fg_rz;

                col_bg = pow(col_bg, _Brightness);
                col_mid = pow(col_mid, _Brightness);
                col_fg = pow(col_fg, _Brightness);

                float stars = 0.0;
                float2 star_uv = i.uv * 100.0 + localTime() * _BGSpeed * 20.0;
                float2 grid_pos = floor(star_uv);
                float2 local_pos = frac(star_uv);

                float rand_val = rand(grid_pos);
                if (rand_val > 0.965)
                {
                    float dist = distance(local_pos, float2(0.5, 0.5));
                    float intensity = smoothstep(0.05, 0.0, dist);
                    float flicker = (_StarsFlicker > 0.5) ? (0.5 + 0.5 * sin(_Time.y * (rand_val * 3.0) + rand_val * 100.0)) : 1.0;
                    stars = intensity * flicker * rand_val;
                }

                float3 final_col = col_bg + col_mid + col_fg;
                if (_StarsOn > 0.5)
                {
                    final_col += float3(stars, stars, stars);
                }

                return float4(final_col * _ColorMultiplier.rgb, 1.0) * _ColorMultiplier2;
            }
            ENDCG
        }
    }
}
