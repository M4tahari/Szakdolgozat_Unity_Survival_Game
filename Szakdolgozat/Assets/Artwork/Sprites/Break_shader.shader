Shader "Unlit/Break_shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BreakTex1 ("Break1", 2D) = "white" {}
        _BreakTex2("Break2", 2D) = "white" {}
        _BreakTex3("Break3", 2D) = "white" {}
        _Damage ("Damage", Range(0,1)) = 0.5
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
            };

            sampler2D _MainTex;
            sampler2D _BreakTex1;
            sampler2D _BreakTex2;
            sampler2D _BreakTex3;
            half _Damage;
            
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 breakTex;

                if (_Damage > 0.75) {
                    breakTex = tex2D(_BreakTex3, i.uv);
                }

                else if (_Damage > 0.5) {
                    breakTex = tex2D(_BreakTex2, i.uv);
                }

                else if (_Damage > 0.25) {
                    breakTex = tex2D(_BreakTex1, i.uv);
                }

                if (_Damage > 0.25) {
                    col = col * (1 - breakTex.a) + breakTex * (breakTex.a);
                }

                else {
                    col = col;
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
