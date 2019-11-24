Shader "SeganX/Game/Diffuse"
{
    Properties
    {
        _MainTex("Diffuse Texture", 2D) = "white" {}
        _Color("Diffuse Color", Color) = (1,1,1,1)
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
            LOD 200

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
                    UNITY_FOG_COORDS(4)
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vrtx = UnityObjectToClipPos(v.vrtx);
                    o.norm = UnityObjectToWorldNormal(v.norm);
                    o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
                    UNITY_TRANSFER_FOG(o, o.vrtx);
                    return o;
                }


                fixed4 _Color;
                float _ColorStrength;
                uniform float bloomSpecular;

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 res = tex2D(_MainTex, i.uv0) * _Color * _ColorStrength;

                    half dl = max(0, dot(i.norm, _WorldSpaceLightPos0.xyz));
                    fixed3 ambient = (i.norm.y > 0) ? lerp(unity_AmbientEquator.rgb, unity_AmbientSky.rgb, i.norm.y) : lerp(unity_AmbientEquator.rgb, unity_AmbientGround.rgb, -i.norm.y);
                    res.rgb *= lerp(ambient, _LightColor0.rgb, dl);

                    UNITY_APPLY_FOG(i.fogCoord, res);
                    res.a = bloomSpecular * _Color.a;

                    res.rgb *= _LightColor0.a;
                    return res;
                }
                ENDCG
            }
        }
            Fallback "Mobile/Diffuse"
}
