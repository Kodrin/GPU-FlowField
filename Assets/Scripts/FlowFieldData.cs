using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowField
{

    //always make sure your data on the c# side matches with the data on the sahder side (wont work otherwise)
    [System.Serializable]
    public struct FlowFieldPointData
    {
        public Vector3 position;
        public Vector3 direction;
        public float intensity;
    }        
    
    [System.Serializable]
    public struct ParticleData
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
    }

}
