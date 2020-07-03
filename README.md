# GPU-FlowField
3D Compute shader directional flow field created on Unity's newest Universal Rendering Pipeline.

### Summary

---
Flow fields are a great way to simulate forces on any given object in 2D or 3D space. The concept is simple.
You create a grid of points that each represent a position with a direction in space. As your agents/particles
travel through this grid space, they will read the nearest point within that flow field to determine it's current
direction. For this example, I am showing how you can achieve these result with a compute shader operation in 
Unity's Universal Rendering Pipeline.

<img src="Media/flow2.gif" width=100%>

### Setup Process (C# side)

---

1. We first generate our buffers for our compute shader on the C# side. We basically initialize our 3D grid
```c#
protected void GenerateFlowFieldBuffer()
{
    flowFieldBuffer = new ComputeBuffer(FLOWFIELD_POINTS_NUM, Marshal.SizeOf(typeof(FlowFieldPointData)));
    var flowPoints = new FlowFieldPointData[FLOWFIELD_POINTS_NUM];

    int iterations = 0;
    for (int x = 0; x < xPointCount; x++)
    {
        for (int y = 0; y < yPointCount; y++)
        {
            for (int z = 0; z < zPointCount; z++)
            {
                var index =  (x * yPointCount + y) * zPointCount + z;

                Vector3 position = new Vector3(
                    simulationSpace.x / xPointCount * x + cellSize/2,
                    simulationSpace.y / yPointCount * y + cellSize/2,
                    simulationSpace.z / zPointCount * z + cellSize/2
                );
                position += this.transform.position - simulationSpace/2;
                
                flowPoints[index] = CreateFlowFieldPoint(position);
                iterations++;
            }
        }
    }
    flowFieldBuffer.SetData(flowPoints);
}
```

2. Now that we have our buffer, we set it in the compute shader and dispatch our shader operation (basically calling it).
```c#
protected void DispatchFlowField()
{
    flowFieldCS.SetBuffer(flowFieldKernelIndex, "_FlowFieldPointBuffer", flowFieldBuffer);
    flowFieldCS.Dispatch(flowFieldKernelIndex, (Mathf.CeilToInt(FLOWFIELD_POINTS_NUM / (int)TCOUNT_X) + 1), 1, 1);
}

```

3. Using DrawProceduralNow to draw points to screen:
```c#
protected void RenderFlowField()
{
    flowFieldMat.SetPass(0); 
    flowFieldMat.SetBuffer("_FlowFieldBuffer", flowFieldBuffer);
    Graphics.DrawProceduralNow(MeshTopology.Points, flowFieldBuffer.count);
}
```

### Compute Shader-Side

---

1. Now that our flow field is set up, we just grab the nearest flow field point based on the current particle position with this function
(it's the same function from our c# script, just reversed and optimized for shaders):

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

2. Now that you have the flow field point index, just access it directly from the buffer and get it's direction. Like so:

```glsl
uint flowIndex = GrabGridIndex(particle.position);
float3 flowDirection = _FlowFieldPointBuffer[flowIndex].direction;
```

### References

---
- [Ronja Voronoi Noise](https://www.ronja-tutorials.com/2018/09/29/voronoi-noise.html)
- [Erkaman Worley Noise](https://github.com/Erkaman/glsl-worley)

### License
Open-source! Use for whatever you want.

### Support
If you enjoy these open-source projects, the most direct way to support the development of future releases
is through patreon.

<a href="https://www.patreon.com/bePatron?u=17790833" data-patreon-widget-type="become-patron-button">Become a Patron!</a><script async src="https://c6.patreon.com/becomePatronButton.bundle.js"></script>