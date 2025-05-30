Shader "MK4/RainAlpha_URP"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Range(0,1)) = 0.5
        _Color0("Color 0", Color) = (0.5807742,0.7100198,0.9632353,0)
        _Albedo("Albedo", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _AO("AO", 2D) = "white" {}
        _SpecularGloss("Specular Gloss", 2D) = "white" {}
        _Specular("Specular", Range(0,1)) = 0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _RainMask("Rain Mask", Range(0,1)) = 0.5
        _RainDropsNormal("RainDrops Normal", 2D) = "bump" {}
        _Raindropsint("Raindrops int", Range(0,5)) = 0
        _RaindropsUVTile("Raindrops UV Tile", Range(0,1)) = 0
        _RainSpeed("Rain Speed", Range(0,50)) = 0
        _WaveNormal("Wave Normal", 2D) = "bump" {}
        _WaveNormalint("Wave Normal int", Range(0,5)) = 0
        _WaveSpeed("Wave Speed", Range(0,1)) = 0
        _WaveUVTile("Wave UV Tile", Range(0,1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="TransparentCutout" "Queue"="AlphaTest+0"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            // Properties
            TEXTURE2D(_Albedo);
            SAMPLER(sampler_Albedo);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_AO);
            SAMPLER(sampler_AO);
            TEXTURE2D(_SpecularGloss);
            SAMPLER(sampler_SpecularGloss);
            TEXTURE2D(_RainDropsNormal);
            SAMPLER(sampler_RainDropsNormal);
            TEXTURE2D(_WaveNormal);
            SAMPLER(sampler_WaveNormal);

            CBUFFER_START(UnityPerMaterial)
                float4 _Albedo_ST;
                float4 _NormalMap_ST;
                float4 _AO_ST;
                float4 _SpecularGloss_ST;
                float  _Cutoff;
                float4 _Color0;
                float  _Specular;
                float  _Smoothness;
                float  _RainMask;
                float  _Raindropsint;
                float  _RaindropsUVTile;
                float  _RainSpeed;
                float  _WaveNormalint;
                float  _WaveSpeed;
                float  _WaveUVTile;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _Albedo);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = float4(TransformObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);
                return OUT;
            }

            // 노멀 블렌딩 함수
            float3 BlendNormals(float3 n1, float3 n2)
            {
                return normalize(float3(n1.xy + n2.xy, n1.z * n2.z));
            }

            float3 UnpackScaleNormal(Texture2D tex, SamplerState samp, float2 uv, float scale)
            {
                float3 n = UnpackNormal(tex.Sample(samp, uv));
                n.xy *= scale;
                return normalize(n);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Albedo
                float4 albedo = _Albedo.Sample(sampler_Albedo, IN.uv);
                // Normal
                float2 uv_NormalMap = IN.uv * _NormalMap_ST.xy + _NormalMap_ST.zw;
                float3 normalTS = UnpackNormal(_NormalMap.Sample(sampler_NormalMap, uv_NormalMap));
                // Flipbook UV 애니메이션
                float  temp_output_254_0 = lerp(0.05, 3.0, _RaindropsUVTile);
                float2 appendResult75 = float2(frac(IN.worldPos.x * temp_output_254_0), frac(IN.worldPos.z * temp_output_254_0));
                float  fbtotaltiles4 = 8.0 * 8.0;
                float  fbcolsoffset4 = 1.0 / 8.0;
                float  fbrowsoffset4 = 1.0 / 8.0;
                float  fbspeed4 = _Time.y * _RainSpeed;
                float  fbcurrenttileindex4 = round(fmod(fbspeed4, fbtotaltiles4));
                if(fbcurrenttileindex4 < 0) fbcurrenttileindex4 += fbtotaltiles4;
                float fblinearindextox4 = round(fmod(fbcurrenttileindex4, 8.0));
                float fboffsetx4 = fblinearindextox4 * fbcolsoffset4;
                float fblinearindextoy4 = round(fmod((fbcurrenttileindex4 - fblinearindextox4) / 8.0, 8.0));
                fblinearindextoy4 = 7.0 - fblinearindextoy4;
                float  fboffsety4 = fblinearindextoy4 * fbrowsoffset4;
                float2 fboffset4 = float2(fboffsetx4, fboffsety4);
                float2 fbuv4 = appendResult75 * float2(fbcolsoffset4, fbrowsoffset4) + fboffset4;
                // Wave
                float  temp_output_241_0 = _Time.y * lerp(0.0, 2.0, _WaveSpeed);
                float2 appendResult183 = float2(IN.worldPos.x, IN.worldPos.z);
                float2 panner245 = appendResult183 + temp_output_241_0 * float2(0.27, -0.25);
                float  temp_output_222_0 = lerp(0.05, 3.0, _WaveUVTile);
                float2 panner231 = appendResult183 + temp_output_241_0 * float2(-0.15, -0.23);
                float2 panner91 = appendResult183 + temp_output_241_0 * float2(0.18, 0.2);
                // SpecularGloss
                float2 uv_SpecularGloss = IN.uv * _SpecularGloss_ST.xy + _SpecularGloss_ST.zw;
                float4 tex2DNode210 = _SpecularGloss.Sample(sampler_SpecularGloss, uv_SpecularGloss);
                float  clampResult228 = clamp(lerp(-1.0 + _RainMask * 2.0, tex2DNode210.a, 1.0), 0.0, 1.0);
                // Normal 합성
                float3 rainNormal = UnpackScaleNormal(_RainDropsNormal, sampler_RainDropsNormal, fbuv4, _Raindropsint);
                float3 waveNormal1 = UnpackScaleNormal(_WaveNormal, sampler_WaveNormal, panner245 * temp_output_222_0, _WaveNormalint);
                float3 waveNormal2 = UnpackScaleNormal(_WaveNormal, sampler_WaveNormal, (panner231 * 0.9) * temp_output_222_0, _WaveNormalint);
                float3 waveNormal3 = UnpackScaleNormal(_WaveNormal, sampler_WaveNormal, panner91 * temp_output_222_0, _WaveNormalint);
                float3 blendedNormal = BlendNormals(normalTS, rainNormal + waveNormal1 + waveNormal2 + waveNormal3);
                float3 finalNormal = normalize(lerp(normalTS, blendedNormal, clampResult228));
                // Albedo 색상
                float4 lerpResult120 = lerp(float4(1, 1, 1, 0), _Color0, clampResult228);
                float3 finalAlbedo = (lerpResult120 * albedo).rgb;
                // Specular, Smoothness
                float clampResult214 = clamp(lerp(-1.0 + _Specular * 2.0, tex2DNode210.r, 1.0), 0.0, 1.0);
                float clampResult212 = clamp(lerp(-1.0 + _Smoothness * 2.0, tex2DNode210.a, 1.0), 0.0, 1.0);
                // AO
                float2 uv_AO = IN.uv * _AO_ST.xy + _AO_ST.zw;
                float  ao = _AO.Sample(sampler_AO, uv_AO).r;
                // Alpha
                float alpha = 1.0;
                // Alpha Cutoff
                clip(albedo.a - _Cutoff);

                // Lighting
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.worldPos;
                inputData.normalWS = finalNormal;
                inputData.viewDirectionWS = GetWorldSpaceViewDir(IN.worldPos);
                inputData.shadowCoord = 0;
                inputData.tangentWS = IN.tangentWS;
                inputData.vertexLighting = 0;
                inputData.fogCoord = 0;

                SurfaceData surfaceData;
                surfaceData.albedo = finalAlbedo;
                surfaceData.metallic = 0;
                surfaceData.specular = clampResult214;
                surfaceData.smoothness = clampResult212;
                surfaceData.normalTS = finalNormal;
                surfaceData.occlusion = ao;
                surfaceData.emission = 0;
                surfaceData.alpha = alpha;

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}