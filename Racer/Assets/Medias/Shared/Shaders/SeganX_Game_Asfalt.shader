Shader "SeganX/Game/Asfalt"
{
    Properties
    {
        _MainTex("Diffuse Texture", 2D) = "white" {}
        [NoScaleOffset]_SpecTex("Specular Texture", 2D) = "white" {}
        _Color("Diffuse Color", Color) = (1,1,1,1)
        _ColorStrength("Color Strength", Float) = 1
        _SpecularAtten("Specular", Range(0, 1)) = 0.5
        _SpecularPower("Specular Power", Range(5, 200)) = 50
        _SpeedTileFactor("Speed Tile Factor", Range(0, 1)) = 0.7

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
                #pragma multi_compile __ SX_SIMPLE


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
                    float3 wrl : TEXCOORD1;
                    float zdpt : TEXCOORD2;
                    UNITY_FOG_COORDS(3)
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vrtx = UnityObjectToClipPos(v.vrtx);
                    o.norm = UnityObjectToWorldNormal(v.norm);
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    o.wrl = mul(unity_ObjectToWorld, v.vrtx).xyz;
                    const float depz = -1.0f / (8.0f - 4.0f);
                    const float depw = 8.0f / (8.0f - 4.0f);
                    o.zdpt = (UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vrtx.z) * depz + depw);
                    UNITY_TRANSFER_FOG(o, o.vrtx);
                    return o;
                }


                sampler2D _SpecTex;
                fixed4 _Color;
                float _SpecularPower;
                float _SpecularAtten;
                float _ColorStrength;
                float _Speed;
                float _SpeedTileFactor;
                uniform float bloomSpecular;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 res = tex2D(_MainTex, i.uv0);

#if !SX_SIMPLE
                    fixed3 blur = tex2D(_MainTex, float2(i.uv0.x * 0.05f + _SpeedTileFactor, i.uv0.y));
                    res.rgb = lerp(res.rgb, blur, clamp(i.zdpt * _Speed, 0, 1));
#endif
                    res *= _Color * _ColorStrength;

                    half dl = max(0, dot(i.norm, _WorldSpaceLightPos0.xyz));
                    fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                    res.rgb *= lerp(ambient, _LightColor0.rgb, dl);

#if SX_SIMPLE
                    res.a = bloomSpecular;
#else
                    float specmap = tex2D(_SpecTex, i.uv0).a;
                    half3 viewDir = normalize(UnityWorldSpaceViewDir(i.wrl));
                    half3 lightpos = normalize(float3(_WorldSpaceLightPos0.x, 0.25f, _WorldSpaceLightPos0.z));
                    float spec = specmap * pow(max(0, dot(i.norm, normalize(lightpos + viewDir))), _SpecularPower) * _SpecularAtten;
                    res.rgb += (pow(res.rgb, 0.7f) * spec + pow(res.rgb * spec * 5, 4)) * _LightColor0.rgb * _LightColor0.a;
                    res.a = min(bloomSpecular, spec) * _LightColor0.a;
#endif

                    res.rgb *= _LightColor0.a;

                    UNITY_APPLY_FOG(i.fogCoord, res);
                    return res;
                }
                ENDCG
            }
        }
            Fallback "Mobile/Diffuse"
}
