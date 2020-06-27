Shader "Unlit/Base"
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
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "DataStructs.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 position : TEXCOORD2;
                float3 direction : TEXCOORD3;
                float3 intensity : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            StructuredBuffer<FlowFieldPointData> _FlowFieldBuffer;

            v2f vert (appdata v,uint id : SV_VertexID)
            {
                v2f o;
                o.position = _FlowFieldBuffer[id].position;
                o.vertex = UnityObjectToClipPos(o.position);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.direction = _FlowFieldBuffer[id].direction;
                o.intensity = _FlowFieldBuffer[id].intensity;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
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
