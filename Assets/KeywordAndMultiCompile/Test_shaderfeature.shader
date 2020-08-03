Shader "MJ/Test_shaderfeature"
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
            
            #pragma shader_feature _ _SF_RED _SF_GREEN _SF_BLUE
            
            #pragma shader_feature _ TEST_KW1 TEST_KW2 TEST_KW3
            
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
                #if defined(_SF_RED)
                {
                    return                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     float4(1,0,0,1);
                }
                #endif
                
                #if defined(_SF_GREEN)
                {
                    return float4(0,1,0,1);
                }
                #endif
                
                #if defined(_SF_BLUE)
                {
                    return float4(0,0,1,1);
                }
                #endif
                
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
    
    CustomEditor "KWAndMCShaderGUI"
}
