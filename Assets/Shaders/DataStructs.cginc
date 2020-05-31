#ifndef FLOWFIELD_DATA_INCLUDED
#define FLOWFIELD_DATA_INCLUDED

struct FlowFieldPointData
{
    float3 position;
    float3 direction;
    float intensity;
};

struct ParticleData
{
    float3 position;
    float3 direction;
    float3 speed;
};

#endif