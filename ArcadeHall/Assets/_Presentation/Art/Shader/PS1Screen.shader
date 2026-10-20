Shader "ArcadeHall/PS1Screen"
{
    Properties
    {
        _Resolution ("Vertical Resolution", Float) = 240
        _ColorLevels ("Color Levels", Range(4, 64)) = 32
        _DitherStrength ("Dither Strength", Range(0, 2)) = 1
        _Saturation ("Saturation", Range(0, 2)) = 1.1
        _Scanlines ("Scanlines", Range(0, 1)) = 0.15
        _Vignette ("Vignette", Range(0, 2)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "PS1Screen"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Resolution;
            float _ColorLevels;
            float _DitherStrength;
            float _Saturation;
            float _Scanlines;
            float _Vignette;

            static const float Bayer4x4[16] =
            {
                 0,  8,  2, 10,
                12,  4, 14,  6,
                 3, 11,  1,  9,
                15,  7, 13,  5
            };

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 lowRes = float2(_Resolution * _ScreenParams.x / _ScreenParams.y, _Resolution);
                float2 pixel = floor(uv * lowRes);
                float2 pixelUV = (pixel + 0.5) / lowRes;

                half3 color = saturate(SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, pixelUV).rgb);

                #if !UNITY_COLORSPACE_GAMMA
                color = LinearToSRGB(color);
                #endif

                half luma = dot(color, half3(0.299, 0.587, 0.114));
                color = saturate(lerp(luma.xxx, color, _Saturation));

                uint2 cell = (uint2)pixel % 4;
                float bayer = Bayer4x4[cell.y * 4 + cell.x] / 16.0 - 0.5;
                color += bayer * _DitherStrength / _ColorLevels;
                color = floor(saturate(color) * _ColorLevels + 0.5) / _ColorLevels;

                float scan = sin(uv.y * lowRes.y * PI);
                color *= 1.0 - _Scanlines * (1.0 - scan * scan);

                float2 offset = uv - 0.5;
                color *= saturate(1.0 - dot(offset, offset) * _Vignette);

                #if !UNITY_COLORSPACE_GAMMA
                color = SRGBToLinear(color);
                #endif

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
