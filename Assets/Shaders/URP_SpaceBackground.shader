Shader "Custom/URP_ProceduralNebula"
{
    Properties
    {
        _MainTex   ("Noise Texture",           2D)    = "white" {}
        _Tex2      ("Distorted Noise Texture", 2D)    = "white" {}
        _MaskTex   ("Mask Texture",            2D)    = "white" {}

        _Distort   ("Distortion Amount",       Float) = 0.3
        _Color     ("Nebula Color",            Color) = (0.5, 0.7, 1, 1)
        _HighLight ("Highlight Color",         Color) = (1, 1, 0.8, 1)
        _Pow       ("Alpha Power",             Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"            = "Transparent"
            "Queue"                 = "Transparent"
            "IgnoreProjector"       = "True"
            "PreviewType"           = "Plane"
            "CanUseSpriteAtlas"     = "True"
            "RenderPipeline"        = "UniversalPipeline"
        }

        Pass
        {
            Name        "Unlit"
            Tags        { "LightMode" = "Unlit" }
            Blend       SrcAlpha OneMinusSrcAlpha
            ZWrite      Off

            HLSLPROGRAM
            #pragma vertex                  vert
            #pragma fragment                frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma target                  3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_Tex2);      SAMPLER(sampler_Tex2);
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex);

            float _Distort;
            float4 _Color;
            float4 _HighLight;
            float _Pow;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Base noise
                float4 col  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // Distort lookup into second noise
                float2 offset = float2(_Distort * (col.r - 0.5), _Distort * (col.g - 0.5));
                float4 col2 = SAMPLE_TEXTURE2D(_Tex2, sampler_Tex2, uv + offset);

                // Circular radial mask
                float dist = length(float2(0.5, 0.5) - uv);
                float radA = saturate((0.5 - max(dist - 0.25, 0)) / 0.25);

                // User-supplied mask
                float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv);

                // Combine highlight & base colors
                float3 finalColor = lerp(_HighLight.rgb, _Color.rgb, col2.b * 0.5);

                // Alpha channel stacking
                float finalAlpha = col2.a * col2.g * col.b * mask.r * mask.g * radA;
                finalAlpha = pow(finalAlpha, _Pow);
                finalAlpha = min(finalAlpha, 0.9);

                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
