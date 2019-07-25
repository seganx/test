// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//Shader for low-poly vehicles. Made by ArsKvsh and TheROBONIK from Rune Studios. 
Shader "Rune Studios/Rune's Vehicle Shader" {
    Properties{
        //_Tint ("Tint", Color) = (1,1,1,1)
        _Diffuse("Diffuse", 2D) = "white" {}
        _Specular("Specular", 2D) = "white" {}
        _SpecularForce("Specular Force", Range(0, 5)) = 0
        _Gloss("Gloss", Range(0, 2)) = 0.5
        _Reflection1Force("Reflection 1 Force", Range(0, 1)) = 0
        _Reflection2Force("Reflection 2 Force", Range(0, 3)) = 0
        _Colormap("Colormap", 2D) = "white" {}
        _CarColor("Car Color", Color) = (1,1,1,1)
    }
        SubShader{
           Tags { "RenderType" = "Opaque" }

        Pass {
                Name "Forward"
                Tags { "LIGHTMODE" = "ForwardBase" "SHADOWSUPPORT" = "true" "RenderType" = "Opaque" }
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment main
                #pragma multi_compile_fog
                #pragma glsl
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                float4 _LightColor0;
                float4 _Diffuse_ST;
                float4 _Colormap_ST;
                float4 _CarColor;
                float4 _Specular_ST;
                float _Gloss;
                float _SpecularForce;
                float _Reflection1Force;
                float _Reflection2Force;
                sampler2D _Diffuse;
                sampler2D _Specular;
                sampler2D _Colormap;
                uniform float bloomSpecular;


                struct VertexInput {
                        float4 vertex : POSITION;
                        float3 normal : NORMAL;
                        float2 texcoord0 : TEXCOORD0;
                };

                struct VertexOutput {
                        float4 pos : SV_POSITION;
                        float3 normalDir : TEXCOORD2;
                        float4 posWorld : TEXCOORD1;
                        float2 uv0 : TEXCOORD0;
                        LIGHTING_COORDS(3,4)
                        UNITY_FOG_COORDS(5)
                };

                VertexOutput vert(VertexInput a) {
                        VertexOutput b;
                        b.uv0 = a.texcoord0;
                        b.normalDir = mul(float4(a.normal,0), unity_WorldToObject).xyz;
                        b.posWorld = mul(unity_ObjectToWorld, a.vertex);
                        b.pos = UnityObjectToClipPos(a.vertex);
                        TRANSFER_VERTEX_TO_FRAGMENT(b)
                        UNITY_TRANSFER_FOG(b,b.pos);
                        return b;
                }

                fixed4 main(VertexOutput i) : COLOR
                {
                        //diff
                        float4 diffmap = tex2D(_Diffuse, i.uv0);
                        clip(diffmap.a - 0.5);
                        
                        //light
                        float dott = abs(dot(i.normalDir, normalize(_WorldSpaceLightPos0.xyz)));
                        float3 diff_light = dott * _LightColor0.xyz + UNITY_LIGHTMODEL_AMBIENT.rgb;

                        float4 colormap = tex2D(_Colormap, i.uv0);
                        float3 output = diff_light * ((colormap.rgb * _CarColor.rgb) + (diffmap.rgb - colormap.rgb));

                        //spec and gloss
                        float4 specmap = tex2D(_Specular, i.uv0);
                        float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                        float3 cube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(i.normalDir, viewDirection)).rgb;
                        float3 specamb = specmap.rgb * cube * _Reflection1Force;// +(1.0 - specmap.rgb) * cube * _Reflection2Force;
                        output += specamb;
                        UNITY_APPLY_FOG(i.fogCoord, output);
                        return float4(output, bloomSpecular);
                    }
                    ENDCG
                }
        }
            FallBack "Diffuse"
}
