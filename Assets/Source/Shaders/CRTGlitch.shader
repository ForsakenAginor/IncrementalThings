Shader "Custom/CRTGlitch"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.08
        _ScanlineCount ("Scanline Count", Float) = 400
        _GlitchSpeed ("Glitch Speed", Float) = 1.5
        _GlitchIntensity ("Glitch Intensity", Range(0, 1)) = 0.25
        _GlitchFrequency ("Glitch Frequency", Range(0, 1)) = 0.03
        _ChromaShift ("Chroma Shift", Range(0, 0.02)) = 0.002
        _VignetteStrength ("Vignette", Range(0, 1)) = 0.25
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.04
        _NoiseSize ("Noise Size", Range(1, 10)) = 2.0
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.35
        _GlowSpread ("Glow Spread", Range(0.001, 0.02)) = 0.005
        _GlowColor ("Glow Color", Color) = (0.3, 0.8, 0.5, 1.0)
        _CurvatureStrength ("Curvature Strength", Range(0, 1)) = 0.15
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off Blend Off

        Pass
        {
            Name "CRTGlitch"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float  _ScanlineStrength;
            float  _ScanlineCount;
            float  _GlitchSpeed;
            float  _GlitchIntensity;
            float  _GlitchFrequency;
            float  _ChromaShift;
            float  _VignetteStrength;
            float  _NoiseStrength;
            float  _NoiseSize;
            float  _GlowStrength;
            float  _GlowSpread;
            float4 _GlowColor;
            float  _CurvatureStrength;

            float hash(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            // Barrel Distortion — выпуклость как у ЭЛТ телека
            float2 CRTCurve(float2 uv, float strength)
            {
                // Переводим UV в диапазон -1..1 (центр экрана = 0,0)
                float2 curved = uv * 2.0 - 1.0;

                // Смещаем каждую ось на основе квадрата противоположной
                float2 offset = curved.yx * curved.yx * strength;
                curved += curved * offset;

                // Возвращаем обратно в 0..1
                return curved * 0.5 + 0.5;
            }

            half3 SampleBlurred(float2 uv, float spread)
            {
                half3 col = 0;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( spread,  0)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-spread,  0)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0,  spread)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0, -spread)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( spread,  spread)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-spread,  spread)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( spread, -spread)).rgb;
                col += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-spread, -spread)).rgb;
                return col / 8.0;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float t = _Time.y * _GlitchSpeed;

                // ── Barrel Distortion ────────────────────────────────
                float2 curvedUV = CRTCurve(uv, _CurvatureStrength);

                // Всё за пределами экрана (углы) рисуем чёрным
                float edgeMask = step(0.0, curvedUV.x) * step(curvedUV.x, 1.0)
                               * step(0.0, curvedUV.y) * step(curvedUV.y, 1.0);

                // ── Глитч ────────────────────────────────────────────
                float glitchSeed = floor(t * 6.0);
                float lineNoise  = hash(float2(curvedUV.y * 60.0, glitchSeed));
                float globalTrig = hash(float2(glitchSeed * 0.17, 3.33));
                float glitchBand = step(1.0 - _GlitchFrequency, globalTrig)
                                 * step(0.9, lineNoise);

                float xShift = (hash(float2(curvedUV.y * 120.0, glitchSeed)) - 0.5)
                               * 0.06 * _GlitchIntensity * glitchBand;
                float2 uvShifted = curvedUV + float2(xShift, 0);

                // ── Хроматическая аберрация ──────────────────────────
                float ca = _ChromaShift + glitchBand * _ChromaShift * 4.0;
                float r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,
                              uvShifted + float2( ca, 0)).r;
                float g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,
                              uvShifted).g;
                float b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,
                              uvShifted - float2( ca, 0)).b;
                half4 col = half4(r, g, b, 1.0);

                // ── Glow ─────────────────────────────────────────────
                half3 blurred  = SampleBlurred(uvShifted, _GlowSpread);
                half3 blurred2 = SampleBlurred(uvShifted, _GlowSpread * 2.5);
                half3 bright   = max(0, blurred  - 0.5) * 2.0;
                half3 bright2  = max(0, blurred2 - 0.4) * 1.5;
                col.rgb += bright  * _GlowStrength * _GlowColor.rgb;
                col.rgb += bright2 * _GlowStrength * 0.4 * _GlowColor.rgb;

                // ── Scanlines ─────────────────────────────────────────
                float scan = sin(curvedUV.y * _ScanlineCount * 3.14159) * 0.5 + 0.5;
                col.rgb *= 1.0 - scan * _ScanlineStrength;

                // ── Film Grain ────────────────────────────────────────
                float2 noiseUV = floor(curvedUV * (1000.0 / _NoiseSize)) / (1000.0 / _NoiseSize);
                float grain = hash(noiseUV + frac(float2(t * 0.07, t * 0.031)));
                col.rgb += (grain - 0.5) * _NoiseStrength;

                // ── Редкая вспышка ────────────────────────────────────
                float fSeed = floor(t * 2.5);
                float fY    = hash(float2(fSeed, 0.42));
                float fTrig = step(0.97, hash(float2(fSeed * 0.2, 1.11)));
                float flash = smoothstep(0.01, 0.0, abs(curvedUV.y - fY)) * fTrig;
                col.rgb += flash * 0.3;

                // ── Виньетка ──────────────────────────────────────────
                float2 vig = curvedUV * (1.0 - curvedUV.yx);
                col.rgb *= pow(vig.x * vig.y * 16.0, _VignetteStrength);

                // ── Чёрные углы от искажения ──────────────────────────
                col.rgb *= edgeMask;

                return col;
            }
            ENDHLSL
        }
    }
}