
Shader "Shader/Shader_016RayMarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Assets/UnityTools/Shaders/RayMarch/RayMarch.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Water.hlsl"

            sampler2D _MainTex;
            sampler2D _BlurTex;            
            CBUFFER_START(UnityPerMaterial) // Required to be compatible with SRP Batcher
            float4 _MainTex_ST;
            float3 centerPos;
            float radius;
            float _alphaMultiplier;
            float _colorMultiplier;
            float4 depthColor;
            float4 surfaceColor;
            float _waterSmoothness;
            float waveLen;
            float4 waves[12];
            CBUFFER_END
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 screenPos :TEXCOORD1;
            };
            
            Varyings FullscreenVert(Attributes input)
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(input.vertex.xyz);
                o.uv = TRANSFORM_TEX(input.uv, _MainTex);
                o.color = input.color;
                               //vertex
                o.screenPos = ComputeScreenPos(o.vertex);//o.vertex是裁剪空间的顶点
                return o;
            }
            
            half4 DepthShow(Varyings input) : SV_Target
            {
              float4 ndcPos = (input.screenPos / input.screenPos.w);

              float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                

              float sceneDepth =  LinearEyeDepth(deviceDepth,_ZBufferParams)*0.001;
              return float4(sceneDepth,sceneDepth,sceneDepth,1);
            }

            float4 GetOceanColor(float oceanViewDepth,float3 oceanWorldPos,float3 normalWS)
            {
                float alpha=1;
                float distToOcean = length(_WorldSpaceCameraPos.xyz - oceanWorldPos);
                float3 color;
                half depthAlpha = 1 - exp(-oceanViewDepth/radius *_alphaMultiplier);
                alpha *= depthAlpha;
                float opticalDepth01 = 1-exp(-(oceanViewDepth/radius)*(distToOcean/radius)*_colorMultiplier);
                //diffuse
                color = lerp(surfaceColor,depthColor,opticalDepth01);

                //specular
                half3 viewDirWS = _WorldSpaceCameraPos.xyz - oceanWorldPos;
                
                half3 oceanNormal = normalize(normalWS);
                Light mainLight = GetMainLight();
                
                half3 halfVec = normalize(normalize(viewDirWS) + normalize(mainLight.direction));
                half specularAngle = acos(dot(oceanNormal,halfVec));
                half specularExponent = specularAngle/_waterSmoothness;
                half kspec = exp(-specularExponent*specularExponent);
                color += kspec;
                return float4(color,alpha);
            }

                
            float3 MultipleWavePositoin(float3 pos,out float3 normal,out float3 tangent){
                float3 newPos = pos;
                normal = 0;
                tangent = 0;

                float3 tmpNormal;
                float3 tmpTangent;
                
                float tmpOffset;
                for(int i=0;i<waveLen;i+=1){
                    tmpOffset = GetWavePosition(pos,waves[i],radius,tmpNormal,tmpTangent);
                      
                    half3 offsetPos = tmpNormal*tmpOffset;
                
                    newPos += offsetPos;
                    normal += tmpNormal;
                    tangent += tmpTangent;
                }
                normal = normalize(normal);
                tangent = normalize(tangent);
                
                return newPos;
            }

                
            half4 FragBlurH(Varyings input) : SV_Target
            {
              float4 ndcPos = (input.screenPos / input.screenPos.w);
              float deviceDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, ndcPos.xy);                
              float3 colorWorldPos = ComputeWorldSpacePosition(ndcPos.xy, deviceDepth, UNITY_MATRIX_I_VP);

              float3 dirCameraToCenter = centerPos -  _WorldSpaceCameraPos.xyz;
              float distCameraToCenter = length(dirCameraToCenter);


                if(distCameraToCenter<radius)
                {
                    //水下
                    // TODO
                    // float3 posToCameraDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
                    // float dist = length(posToCameraDir);
                    // // dist = min(dist,radius);
                    // float fog = dist/20;//可见距离
                    // return float4(surfaceColor.rgb,fog);
                }else{
                        float3 cameraToPosDir = colorWorldPos - _WorldSpaceCameraPos.xyz;
                        float3 rayDir = normalize(cameraToPosDir);      
                        float2 distToSphere = RaySphere(centerPos,radius,_WorldSpaceCameraPos.xyz,rayDir);
                
                      //撞到球distToSphere.y>0,
                      //如果length(worldDir)-distToSphere.x>0海洋,否则,陆地
                      
                      float oceanViewDepth = min(distToSphere.y,length(cameraToPosDir)-distToSphere.x);
                      if(oceanViewDepth>0)
                      {
                            float3 hitSpherePos = _WorldSpaceCameraPos.xyz+ rayDir*distToSphere.x;
                            half3 normalWS = hitSpherePos - centerPos;
                            normalWS = normalize(normalWS);
                            //TODO 还是需要矩阵
                            // if(waveLen>0){
                            //     half3 tangentOS3;
                            //     half3 offsetNormal=0;
                            //     half3 offsetPos = hitSpherePos - centerPos;
                            //     hitSpherePos = MultipleWavePositoin(offsetPos.xyz,offsetNormal,tangentOS3);
                            //     normalWS += offsetNormal;
                            // }
                            // return float4(normalWS,1);
                            // float depth01 = lerp(oceanViewDepth/radius ,1,distToSphere.x/radius);
                            float4 oceanColor = GetOceanColor(oceanViewDepth,hitSpherePos,normalWS);
                            return float4(oceanColor.rgb,oceanColor.a);
                      }
             }
              
                
              return 0;
            }

    
            
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="TransParent" "Queue" = "TransParent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Off
        
        Pass
        {            
            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment FragBlurH           
            ENDHLSL
        }
    }
}