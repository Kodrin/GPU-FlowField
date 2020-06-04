using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace FlowField
{
    [System.Serializable]
    public enum ParticleCounter
    {	
        N_128 = 128,
        N_256 = 256,
        N_512 = 512,
        N_1K = 1024,
        N_2K = 2048,
        N_4K = 4096,
        N_8K = 8192,
        N_16K = 16384,
        N_24k = 24576,
        N_32K = 32768,
        N_64K = 65536,
        N_128K = 131072
    }
    
    [System.Serializable]
    public class ComputeOperation
    {
        public uint XNumThread;
        public uint YNumThread;
        public uint ZNumThread;
        public ComputeBuffer buffer;
        
    }
    
    public class FlowFieldMaster : ComputeBase
    {
        [Header("SHADERS")]
        [SerializeField] protected ComputeShader flowFieldCS;
        [SerializeField] protected Shader flowFieldShader;
        [SerializeField] protected Shader particlesShader;

        [Header("FLOWFIELD PARAMETERS")] 
        [SerializeField] protected float cellSize = 2.5f;

        protected const string KERNEL_FLOWFIELD = "FlowField";
        protected const string KERNEL_PARTICLES = "Particles";

        [SerializeField] protected int FLOWFIELD_POINTS_NUM;
        [SerializeField] protected ParticleCounter PARTICLES_NUM;

        protected int xPointCount;
        protected int yPointCount;
        protected int zPointCount;
        
        protected int flowFieldKernelIndex;
        protected int particlesKernelIndex;

        protected uint XNumThreadFlowField;
        protected uint YNumThreadFlowField;
        protected uint ZNumThreadFlowField;
       
        protected uint XNumThreadParticles;
        protected uint YNumThreadParticles;
        protected uint ZNumThreadParticles;

        protected ComputeBuffer flowFieldBuffer;
        protected ComputeBuffer particlesBuffer;

        protected Material flowFieldMat;
        protected Material particlesMat;


        #region CALL METHODS
        protected override void Start()
        {
            base.Start();
            ComputeInit();
            GenerateBuffers();
            RenderInit();
        }

        protected override void Update()
        {
            base.Update();
            SetConstants();
            Dispatch();
        }

        protected override void OnRenderObject()
        {
            //RenderFlowField();
            RenderParticles();
        }
        
        protected override void OnDestroy()
        {
            DeleteBuffers();
            DeleteMaterials();
        }
        
        #endregion

        protected override void DeleteBuffers()
        {
            if (flowFieldBuffer != null)
            {
                flowFieldBuffer.Release();
                // flowFieldBuffer = null;
            }

            if(particlesBuffer != null)
            {
                particlesBuffer.Release();
                // particlesBuffer = null;
            }
        }
        
        protected override void DeleteMaterials()
        {
            if (flowFieldMat != null)
            {
                DestroyImmediate(flowFieldMat);
                // flowFieldMat = null;
            }
            if (particlesMat != null)
            {
                DestroyImmediate(particlesMat);
                // particlesMat = null;
            }
        }

        #region GENERATE BUFFER
        
        Vector3 RandomWithinSpace()
        {
            Vector3 rand = Random.insideUnitSphere;
            Vector3 position = new Vector3(simulationSpace.x/2 * rand.x, simulationSpace.y/2 * rand.y, simulationSpace.z/2 * rand.z);
            position += this.transform.position;

            return position;
        }

        void GetFlowFieldPointAmount()
        {
            xPointCount = (int)Mathf.Floor(simulationSpace.x/cellSize);
            yPointCount = (int)Mathf.Floor(simulationSpace.y/cellSize);
            zPointCount = (int)Mathf.Floor(simulationSpace.z/cellSize);

            FLOWFIELD_POINTS_NUM = xPointCount * yPointCount * zPointCount;

        }

        FlowFieldPointData CreateFlowFieldPoint(Vector3 position)
        {
            FlowFieldPointData point = new FlowFieldPointData();

            point.position = position;
            point.direction = Random.insideUnitSphere;
            point.intensity = Random.value;
            
            return point;
        }

        ParticleData CreateParticle()
        {
            ParticleData particle = new ParticleData();

            particle.position = RandomWithinSpace();
            particle.direction = Random.insideUnitSphere;
            particle.speed = Random.value;
            
            return particle;
        }

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
			
            Debug.Log(iterations);
			
            flowFieldBuffer.SetData(flowPoints);
        }

        protected void GenerateParticlesBuffer()
        {
            particlesBuffer = new ComputeBuffer((int)PARTICLES_NUM, Marshal.SizeOf(typeof(ParticleData)));
            var particles = new ParticleData[(int)PARTICLES_NUM];
		
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = CreateParticle();
            }
			
            particlesBuffer.SetData(particles);
        }
        
        #endregion


        #region COMPUTE FUNCTIONS

        protected override void ComputeInit()
        {
            //THREAD GROUPS 
            XNumThreadFlowField = (uint)Mathf.Max( 1, (int)FLOWFIELD_POINTS_NUM / (int)TCOUNT_X);	
            XNumThreadParticles = (uint)Mathf.Max( 1, (int)PARTICLES_NUM / (int)TCOUNT_X);

            //KERNELS 
            flowFieldKernelIndex = flowFieldCS.FindKernel(KERNEL_FLOWFIELD);
            particlesKernelIndex = flowFieldCS.FindKernel(KERNEL_PARTICLES);
            // flowFieldCS.GetKernelThreadGroupSizes(flowFieldKernelIndex, out XNumThreadFlowField,
                // out YNumThreadFlowField, out ZNumThreadFlowField);
            // flowFieldCS.GetKernelThreadGroupSizes(particlesKernelIndex, out XNumThreadParticles,
                // out YNumThreadParticles, out ZNumThreadParticles);
        }

        protected override void GenerateBuffers()
        {
            GetFlowFieldPointAmount();
            GenerateFlowFieldBuffer();
            GenerateParticlesBuffer();
        }

        protected override void SetConstants()
        {
            flowFieldCS.SetVector("_BoundsDimensions", simulationSpace);
            flowFieldCS.SetVector("_BoundsPosition", this.transform.position);
            flowFieldCS.SetInt("_XCellCount", xPointCount);
            flowFieldCS.SetInt("_YCellCount", yPointCount);
            flowFieldCS.SetInt("_ZCellCount", zPointCount);
        }

        protected override void Dispatch()
        {
            DispatchParticles();
            // DispatchFlowField();
        }

        protected void DispatchFlowField()
        {
            flowFieldCS.SetBuffer(flowFieldKernelIndex, "_FlowFieldPointBuffer", flowFieldBuffer);
            flowFieldCS.Dispatch(flowFieldKernelIndex, (int)XNumThreadFlowField, (int)1, (int)1);
        }

        protected void DispatchParticles()
        {
            flowFieldCS.SetBuffer(particlesKernelIndex, "_FlowFieldPointBuffer", flowFieldBuffer);
            flowFieldCS.SetBuffer(particlesKernelIndex, "_ParticleBuffer", particlesBuffer);
            flowFieldCS.Dispatch(particlesKernelIndex, (int) XNumThreadParticles, (int) 1,
                (int) 1);
        }
        
        #endregion


        #region RENDER/MATERIALS

        protected void InitFlowFieldMaterial()
        {
            // Material creation
            flowFieldMat = new Material(flowFieldShader);
            flowFieldMat.hideFlags = HideFlags.HideAndDontSave;
        }

        protected void InitParticlesMaterial()
        {
            // Material creation
            particlesMat = new Material(particlesShader);
            particlesMat.hideFlags = HideFlags.HideAndDontSave;
        }

        protected void RenderInit()
        {
            InitFlowFieldMaterial();
            InitParticlesMaterial();
        }

        protected void RenderFlowField()
        {
            flowFieldMat.SetPass(0); 
            flowFieldMat.SetBuffer("_FlowFieldBuffer", flowFieldBuffer);
            Graphics.DrawProceduralNow(MeshTopology.Points, (int)FLOWFIELD_POINTS_NUM);
        }

        protected void RenderParticles()
        {
            particlesMat.SetPass(0); 
            particlesMat.SetBuffer("_ParticleBuffer", particlesBuffer);
            Graphics.DrawProceduralNow(MeshTopology.Points, (int)PARTICLES_NUM);
        }
        
        #endregion
    }

}
