Shader "Rune Studios/Rune's Vehicle Shader"
{
    Properties
    {
        _Diffuse("Diffuse", 2D) = "white" {}
        _Specular("Specular", 2D) = "white" {}
        _Reflection1Force("Reflection 1 Force", Range(0, 1)) = 0
        _Colormap("Colormap", 2D) = "white" {}
        _Color("Car Color", Color) = (1,1,1,1)
    }
        SubShader{
           Tags { "RenderType" = "Opaque" }

        Pass {
                Name "Forward"
                Tags
                {
                    "LIGHTMODE" = "ForwardBase"
                    "SHADOWSUPPORT" = "true"
                    "RenderType" = "Opaque"
                }
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment main
                #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct VertexInput
                {
                        float4 vertex : POSITION;
                        float3 normal : NORMAL;
                        float2 texcoord0 : TEXCOORD0;
                };

                struct VertexOutput
                {
                        float4 pos : SV_POSITION;
                        float3 normalDir : TEXCOORD0;
                        float4 posWorld : TEXCOORD1;
                        float2 uv0 : TEXCOORD2;
                        UNITY_FOG_COORDS(3)
                };

                VertexOutput vert(VertexInput a) 
                {
                        VertexOutput b;
                        b.uv0 = a.texcoord0;
                        b.normalDir = mul(float4(a.normal,0), unity_WorldToObject).xyz;
                        b.posWorld = mul(unity_ObjectToWorld, a.vertex);
                        b.pos = UnityObjectToClipPos(a.vertex);
                        UNITY_TRANSFER_FOG(b,b.pos);
                        return b;
                }


                float4 _Color;
                float _Reflection1Force;
                sampler2D _Diffuse;
                sampler2D _Specular;
                sampler2D _Colormap;

                uniform float bloomSpecular;

                fixed4 main(VertexOutput i) : COLOR
                {
                    //diff
                    float4 diffmap = tex2D(_Diffuse, i.uv0);

                    //light
                    float dott = max(0, dot( i.normalDir, normalize(_WorldSpaceLightPos0.xyz) ) );
                    float3 diff_light = dott * _LightColor0.xyz + UNITY_LIGHTMODEL_AMBIENT.rgb;

                    float4 colormap = tex2D(_Colormap, i.uv0);
                    float3 output = diff_light * ((colormap.rgb * _Color.rgb) + (diffmap.rgb - colormap.rgb));

                    // reflection
                    float4 specmap = tex2D(_Specular, i.uv0);
                    float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                    float3 cube = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(i.normalDir, viewDirection)).rgb;
                    float3 specamb = specmap.rgb * cube * _Reflection1Force;
                    output += specamb;

                    output.rgb *= _LightColor0.a;
                    UNITY_APPLY_FOG(i.fogCoord, output);

                    return float4(output, bloomSpecular);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
