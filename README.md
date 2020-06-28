# GPU-FlowField
 3D Compute shader directional flowfield created on Unity's newest Universal Rendering Pipeline.


#### Grab the nearest flowfield point based on current position
```glsl
uint GrabGridIndex(float3 position)
{
    float x = floor((position.x - (_BoundsPosition.x - _BoundsDimensions.x/2)) / ((_BoundsDimensions.x / _XCellCount)));
    float y = floor((position.y - (_BoundsPosition.y - _BoundsDimensions.y/2)) / ((_BoundsDimensions.y / _YCellCount)));
    float z = floor((position.z - (_BoundsPosition.z - _BoundsDimensions.z/2)) / ((_BoundsDimensions.z / _ZCellCount)));
    
    uint flowFieldIndex = (x * _YCellCount + y) * _ZCellCount + z;
    return flowFieldIndex;
}
```

#### Using DrawProceduralNow to draw points to screen
```c#
protected void RenderFlowField()
{
    flowFieldMat.SetPass(0); 
    flowFieldMat.SetBuffer("_FlowFieldBuffer", flowFieldBuffer);
    Graphics.DrawProceduralNow(MeshTopology.Points, flowFieldBuffer.count);
}
```