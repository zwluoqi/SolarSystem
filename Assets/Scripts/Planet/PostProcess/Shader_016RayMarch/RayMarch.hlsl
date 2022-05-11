#ifndef QINGZHU_TOOLS_RAYMARCH
#define QINGZHU_TOOLS_RAYMARCH

#define maxFloat 10e9

//https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
//
///return distToSphere,distThroughSphere
///inside sphere disttosphere = 0
///missed sphere disttosphere = max,and distThroughSphere =0
///rayDir must normalise
float2 RaySphere(float3 center,float radius,float3 rayOrigin,float3 rayDir)
{
    float3 offset = rayOrigin - center;
    const float a=1;
    float b = 2*dot(offset,rayDir);
    float c = dot(offset,offset) - radius*radius;

    float discriminant = b*b-4*a*c;
    if(discriminant >0)
    {
        float s = sqrt(discriminant);
        float distToSphereNear = max(0,(-b-s)/(2*a));
        float distToSphereFar = (-b+s)/(2*a);
        if(distToSphereFar>=0)
        {
            return float2(distToSphereNear,distToSphereFar-distToSphereNear);
        }
    }

    return float2(maxFloat,0);
}

#endif