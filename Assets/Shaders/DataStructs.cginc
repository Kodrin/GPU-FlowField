#ifndef FLOWFIELD_DATA_INCLUDED
#define FLOWFIELD_DATA_INCLUDED

// always make sure your data on the shader side matches with the data you used on c# side
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
    float speed;
};

#endif