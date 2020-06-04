Shader "FlowField/RenderParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
           CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "DataStructs.cginc"

            struct v2g
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 position : TEXCOORD1;
                float3 direction : TEXCOORD2;
                float3 speed : TEXCOORD3;
            };

            struct g2f
            {
                float4 position : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            StructuredBuffer<ParticleData> _ParticleBuffer;

            v2g vert (uint id : SV_VertexID)
            {
                v2g o = (v2g)0;
                o.position = _ParticleBuffer[id].position;
                o.direction = _ParticleBuffer[id].direction;
                o.speed = _ParticleBuffer[id].speed;
                return o;
            }

            [maxvertexcount(4)]
            void geom(point v2g In[1], inout LineStream<g2f> linestream)
            {
                g2f o = (g2f)0;
                
                float distance = 0.2;
                float directionLength = .1;
                
                float3 position = In[0].position;
                float3 direction = In[0].direction;
                float speed = In[0].speed;
                
                float4 color;
                fixed4 white = fixed4(1,1,1,1);
                fixed4 black = fixed4(0,0,0,0);
                color = float4(direction, 1);
                //color = length(direction) * speed;
                
                //direction
                o.position = UnityObjectToClipPos(float4(position, 1.0));
                o.color = color; 
                linestream.Append(o); 
                o.position = UnityObjectToClipPos(float4(position + (direction * directionLength), 1.0));
                o.color = color; 
                linestream.Append(o); 
                linestream.RestartStrip();     
        


            }

            fixed4 frag (g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
