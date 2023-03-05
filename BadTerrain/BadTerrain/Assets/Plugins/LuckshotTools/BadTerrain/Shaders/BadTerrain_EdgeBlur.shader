Shader "Bad Terrain/Edge Blur"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white"{}
        _Offset("Offset (x, y)", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half2 _Offset;

            float4 sampleBlur(float2 uv, fixed2 offset)
            {
                uv += float2(offset.x * ddx(uv.x), offset.y * ddy(uv.y));
                return tex2D(_MainTex, uv);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 color =
                    tex2D(_MainTex, i.uv) +
                    sampleBlur(i.uv, fixed2(_Offset.x, _Offset.x)) +
                    sampleBlur(i.uv, fixed2(-_Offset.x, _Offset.x)) +
                    sampleBlur(i.uv, fixed2(_Offset.x, -_Offset.x)) +
                    sampleBlur(i.uv, fixed2(-_Offset.x, -_Offset.x)); 

                return color * 0.2;
            }
            ENDCG
        }
    }
}
