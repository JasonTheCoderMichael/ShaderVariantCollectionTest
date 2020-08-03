Shader "MJ/Test_multicompile"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
            
            
            #pragma multi_compile _ _MC_RED _MC_GREEN _MC_BLUE
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #if defined(_MC_RED)
                {
                    return float4(1,0,0,1);
                }
                #endif
                
                #if defined(_MC_GREEN)
                {
                    return float4(0,1,0,1);
                }
                #endif
                
                #if defined(_MC_BLUE)
                {
                    return float4(0,0,1,1);
                }
                #endif
                
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
    
//    CustomEditor "KWAndMCShaderGUI"
}
