#ifndef COLOR_GENERATE_INCLUDED
#define COLOR_GENERATE_INCLUDED

#include "NoiseGenerate.hlsl"


struct LatitudeSettingBuffer{
    float startHeight;
};

struct ColorSettting{
    NoiseLayer noiseLayer;       
};



float ColorGenerateExeculate(ColorSettting colorSetting,
StructuredBuffer<NoiseLayer> noiseLayerSettings,int layerCount,
StructuredBuffer<LatitudeSettingBuffer> latitudes,int latitudeCount,
float3 vertex,float height){

    height = (height + 1) * 0.5f;
    int latitudeIndex = 0;
    for (int i = 0; i < latitudeCount; i++)
    {
        float2 noise = NoiseLayerExcute(vertex,colorSetting.noiseLayer,noiseLayerSettings,layerCount);

        float noiseHeight = noise.y + latitudes[i].startHeight;
        if (height < noiseHeight)
        {
            break;
        }
        else
        {
            latitudeIndex = i;
        }
    }

    float offset = .5f / latitudeCount;
    return (latitudeIndex*1.0f) / (latitudeCount) + offset;            
}


#endif