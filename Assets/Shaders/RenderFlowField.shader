Shader "FlowField/RenderFlowField"
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2g
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2g vert (uint id : SV_VertexID)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            [maxvertexcount(4)]
            void geom(point v2g In[1], inout TriangleStream<g2f> SpriteStream)
            {
                g2f o = (g2f)0;
            /*
                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    float3 position = g_positions[i] * _ParticleSize;
                    position   = mul(_InvViewMatrix, position) + In[0].position;
                    o.position = UnityObjectToClipPos(float4(position, 1.0));
    
                    o.color    = In[0].color;
                    o.texcoord = g_texcoords[i];
                    SpriteStream.Append(o);
                }
                SpriteStream.RestartStrip();
            */
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
