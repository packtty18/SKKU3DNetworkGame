Shader "LowPoly/SimpleWater"
{
    Properties
    {
        _WaterNormal("Water Normal", 2D) = "bump" {}
        _NormalScale("Normal Scale", Float) = 0
        _DeepColor("Deep Color", Color) = (0,0,0,0)
        _ShalowColor("Shalow Color", Color) = (1,1,1,0)
        _WaterDepth("Water Depth", Float) = 0
        _WaterFalloff("Water Falloff", Float) = 0
        _WaterSpecular("Water Specular", Float) = 0
        _WaterSmoothness("Water Smoothness", Float) = 0
        _Distortion("Distortion", Float) = 0.5
        _Foam("Foam", 2D) = "white" {}
        _FoamDepth("Foam Depth", Float) = 0
        _FoamFalloff("Foam Falloff", Float) = 0
        _FoamSpecular("Foam Specular", Float) = 0
        _FoamSmoothness("Foam Smoothness", Float) = 0
        _WavesAmplitude("WavesAmplitude", Float) = 0.01
        _WavesAmount("WavesAmount", Float) = 8.87
        [HideInInspector] _texcoord( "", 2D ) = "white" {}
        [HideInInspector] __dirty( "", Int ) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 300
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float4 _WaterNormal_ST;
                float4 _Foam_ST;
                float4 _DeepColor;
                float4 _ShalowColor;
                float _NormalScale;
                float _WaterDepth;
                float _WaterFalloff;
                float _WaterSpecular;
                float _WaterSmoothness;
                float _Distortion;
                float _FoamDepth;
                float _FoamFalloff;
                float _FoamSpecular;
                float _FoamSmoothness;
                float _WavesAmount;
                float _WavesAmplitude;
            CBUFFER_END
            TEXTURE2D(_WaterNormal);
            SAMPLER(sampler_WaterNormal);
            TEXTURE2D(_Foam);
            SAMPLER(sampler_Foam);
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float eyeDepth : TEXCOORD5;
            };
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Wave animation
                float3 positionOS = input.positionOS.xyz;
                float waveOffset = sin((_WavesAmount * positionOS.z) + _Time.y) * _WavesAmplitude;
                positionOS += input.normalOS * waveOffset;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _WaterNormal);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.eyeDepth = -TransformWorldToView(output.positionWS).z;
                
                return output;
            }
            float4 frag(Varyings input) : SV_Target
            {
                // Screen space calculations
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = SampleSceneDepth(screenUV);
                float sceneZ = LinearEyeDepth(sceneDepth, _ZBufferParams);
                float depthDifference = abs(sceneZ - input.eyeDepth);
                
                // Water normal calculation with panning
                float2 uv1 = input.uv + _Time.y * float2(-0.03, 0);
                float2 uv2 = input.uv + _Time.y * float2(0.04, 0.04);
                
                float3 normal1 = UnpackNormalScale(SAMPLE_TEXTURE2D(_WaterNormal, sampler_WaterNormal, uv1), _NormalScale);
                float3 normal2 = UnpackNormalScale(SAMPLE_TEXTURE2D(_WaterNormal, sampler_WaterNormal, uv2), _NormalScale);
                float3 waterNormal = BlendNormal(normal1, normal2);
                
                // Convert normal to world space
                float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w;
                float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(waterNormal, tangentToWorld));
                
                // Water depth and color
                float waterDepthMask = saturate(pow((depthDifference + _WaterDepth), _WaterFalloff));
                float4 waterColor = lerp(_DeepColor, _ShalowColor, waterDepthMask);
                
                // Foam calculation
                float2 foamUV = TRANSFORM_TEX(input.uv, _Foam) + _Time.y * float2(-0.01, 0.01);
                float foamNoise = SAMPLE_TEXTURE2D(_Foam, sampler_Foam, foamUV).r;
                float foamMask = saturate(pow((depthDifference + _FoamDepth), _FoamFalloff)) * foamNoise;
                float4 finalColor = lerp(waterColor, float4(1, 1, 1, 1), foamMask);
                
                // Screen distortion for refraction
                float2 distortedScreenUV = screenUV + waterNormal.xy * _Distortion * 0.01;
                distortedScreenUV = clamp(distortedScreenUV, 0, 1);
                float3 sceneColor = SampleSceneColor(distortedScreenUV);
                
                // Blend with scene
                finalColor.rgb = lerp(finalColor.rgb, sceneColor, waterDepthMask * 0.8);
                
                // Lighting
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                
                // Specular
                float specular = lerp(_WaterSpecular, _FoamSpecular, foamMask);
                float smoothness = lerp(_WaterSmoothness, _FoamSmoothness, foamMask);
                
                float3 halfVector = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(normalWS, halfVector));
                float specularReflection = pow(NdotH, (1 - smoothness) * 128) * specular;
                
                finalColor.rgb += specularReflection * mainLight.color;
                finalColor.a = lerp(0.8, 1.0, foamMask);
                
                return finalColor;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _WavesAmount;
                float _WavesAmplitude;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                // Wave animation
                float3 positionOS = input.positionOS.xyz;
                float waveOffset = sin((_WavesAmount * positionOS.z) + _Time.y) * _WavesAmplitude;
                positionOS += input.normalOS * waveOffset;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
                output.positionCS = vertexInput.positionCS;
                
                return output;
            }
            float4 DepthOnlyFragment(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}