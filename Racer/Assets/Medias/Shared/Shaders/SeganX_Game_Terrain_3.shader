Shader "SeganX/Game/Terrain/3Texture"
{
    Properties
    {
        _BaseTex("Base Map (RGBA)", 2D) = "black" {}
        _MapTex1("Layer 1 (R)", 2D) = "white" {}
        _MapTex2("Layer 2 (G)", 2D) = "white" {}
        _MapTex3("Layer 3 (B)", 2D) = "white" {}
        _Color("Diffuse Color", color) = (1, 1, 1, 1)
        _ColorStrength("Color Strength", Float) = 1
        
        [Enum(ON,1,OFF,0)]	            _ZWrite("Z Write", Int) = 1
        [Enum(BACK,2,FRONT,1,OFF,0)]	_Cull("Cull", Int) = 2

    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "LightMode" = "ForwardBase"
            }

            Cull[_Cull]
            ZWrite[_ZWrite]

            Pass
            {
                CGPROGRAM
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                struct appdata
                {
                    float4 vrtx : POSITION;
                    float3 norm : NORMAL;
                    float2 uv0 : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vrtx : SV_POSITION;
                    float3 norm : NORMAL;
                    float2 uv0 : TEXCOORD0;
                    float2 uv1 : TEXCOORD1;
                    float2 uv2 : TEXCOORD2;
                    float2 uv3 : TEXCOORD3;
                    UNITY_FOG_COORDS(4)
                };

                sampler2D _BaseTex;
                float4 _BaseTex_ST;
                sampler2D _MapTex1;
                float4 _MapTex1_ST;
                sampler2D _MapTex2;
                float4 _MapTex2_ST;
                sampler2D _MapTex3;
                float4 _MapTex3_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vrtx = UnityObjectToClipPos(v.vrtx);
                    o.norm = UnityObjectToWorldNormal(v.norm);
                    o.uv0 = TRANSFORM_TEX(v.uv0, _BaseTex);
                    o.uv1 = TRANSFORM_TEX(v.uv0, _MapTex1);
                    o.uv2 = TRANSFORM_TEX(v.uv0, _MapTex2);
                    o.uv3 = TRANSFORM_TEX(v.uv0, _MapTex3);
                    UNITY_TRANSFER_FOG(o, o.vrtx);
                    return o;
                }


                fixed4 _Color;
                float _ColorStrength;
                uniform float bloomSpecular;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed3 mbase = tex2D(_BaseTex, i.uv0).rgb;
                    
                    fixed4 res = mbase.r * tex2D(_MapTex1, i.uv1);
                    res += mbase.g * tex2D(_MapTex2, i.uv2);
                    res += mbase.b * tex2D(_MapTex3, i.uv3);
                    res.rgb *= _Color.rgb * _ColorStrength;

                    half dl = max(0, dot(i.norm, _WorldSpaceLightPos0.xyz));
                    fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                    res.rgb *= lerp(ambient, _LightColor0.rgb, dl);

                    res.a = bloomSpecular;
                    UNITY_APPLY_FOG(i.fogCoord, res);
                    return res;
                }
                ENDCG
            }
        }
            Fallback "Mobile/Diffuse"
}
