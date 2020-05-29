using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowField
{
    
    public class FlowFieldMaster : ComputeBase
    {
        
        [SerializeField] protected ComputeShader flowFieldCS;
        [SerializeField] protected ComputeShader particlesCS;
        [SerializeField] protected Shader flowFieldShader;
        [SerializeField] protected Shader particlesShader;

        protected const string KERNEL_FLOWFIELD = "FlowField";
        protected const string KERNEL_PARTICLES = "Particles";

        protected int FLOWFIELD_POINTS_NUM;
        protected int PARTICLES_NUM;
        
        protected int flowFieldKernelIndex;
        protected int particlesKernelIndex;

        protected int flowFieldThreadGroups;
        protected int particlesThreadGroups;

        protected ComputeBuffer flowFieldBuffer;
        protected ComputeBuffer particlesBuffer;

        protected Material flowFieldMat;
        protected Material particlesMat;


        #region CALL METHODS
        protected override void Start()
        {
            base.Start();
            ComputeInit();
            RenderInit();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnRenderObject()
        {
            RenderFlowField();
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
                flowFieldBuffer = null;
            }

            if(particlesBuffer != null)
            {
                particlesBuffer.Release();
                particlesBuffer = null;
            }
        }
        
        protected override void DeleteMaterials()
        {
            if (flowFieldMat != null)
            {
                DestroyImmediate(flowFieldMat);
                flowFieldMat = null;
            }
            if (particlesMat != null)
            {
                DestroyImmediate(particlesMat);
                particlesMat = null;
            }
        }

        #region GENERATE BUFFER

        FlowFieldPointData CreateFlowFieldPoint()
        {
            FlowFieldPointData point = new FlowFieldPointData();

            point.position = Random.insideUnitSphere;
            point.direction = Random.insideUnitSphere;
            point.intensity = Random.value;
            
            return point;
        }

        ParticleData CreateParticle()
        {
            ParticleData particle = new ParticleData();

            particle.position = Random.insideUnitSphere;
            particle.direction = Random.insideUnitSphere;
            particle.speed = Random.value;
            
            return particle;
        }

        protected void GenerateFlowFieldBuffer()
        {
            
        }

        protected void GenerateParticlesBuffer()
        {
            
        }
        
        #endregion


        #region COMPUTE FUNCTIONS
        
        protected override void ComputeInit()
        {
            //THREAD GROUPS 
            flowFieldThreadGroups = Mathf.Max( 1, (int)FLOWFIELD_POINTS_NUM / (int)TCOUNT_X);	
            particlesThreadGroups = Mathf.Max( 1, (int)PARTICLES_NUM / (int)TCOUNT_X);

            //KERNELS 
            flowFieldKernelIndex = flowFieldCS.FindKernel(KERNEL_FLOWFIELD);
            particlesKernelIndex = particlesCS.FindKernel(KERNEL_PARTICLES);
        }
        
        protected override void GenerateBuffers()
        {
            
        }

        protected override void SetConstants()
        {
            
        }

        protected override void Dispatch()
        {
            
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
            particlesMat.SetBuffer("_ParticlesBuffer", particlesBuffer);
            Graphics.DrawProceduralNow(MeshTopology.Points, (int)PARTICLES_NUM);
        }
        
        #endregion
    }

}
