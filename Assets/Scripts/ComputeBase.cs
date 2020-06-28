using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowField
{
    
    [System.Serializable]
    public enum ThreadCount
    {
        T_1 = 1,
        T_4 = 4,
        T_8 = 8,
        T_16 = 16,
        T_32 = 32,
        T_64 = 64,
        T_128 = 128,
        T_256 = 256,
        T_512 = 512,
        T_1024 = 1024
    }
    
    
    public abstract class ComputeBase : MonoBehaviour
    {

        [Header("SIMULATION PARAMS")] 
        [SerializeField] protected Vector3 simulationSpace = Vector3.one;

        [SerializeField] protected ThreadCount TCOUNT_X = ThreadCount.T_256;
        [SerializeField] protected ThreadCount TCOUNT_Y = ThreadCount.T_1;
        [SerializeField] protected ThreadCount TCOUNT_Z = ThreadCount.T_1;
        
        protected virtual void Awake() {}

        protected virtual void Start() {}

        protected virtual void Update() {}

        protected abstract void OnRenderObject();
        protected abstract void OnDestroy();
        protected abstract void ReleaseBuffers();
        protected abstract void DeleteMaterials();

        protected abstract void ComputeInit();
        protected abstract void GenerateBuffers();
        protected abstract void SetConstants();
        protected abstract void Dispatch();
        

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(this.transform.position, simulationSpace);
        }
    }
}
