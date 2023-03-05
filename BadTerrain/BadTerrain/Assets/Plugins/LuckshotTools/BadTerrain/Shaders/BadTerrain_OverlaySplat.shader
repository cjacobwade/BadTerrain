Shader "Bad Terrain/Overlay Splat"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "black"{}
        //_OtherTex("Other Tex", 2D) = "gray"{}
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue" = "Geometry"}
        LOD 100

        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _OtherTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 a = tex2D(_MainTex, i.uv);
                fixed4 b = tex2D(_OtherTex, i.uv);

                float maxA = max(max(max(a.r, a.g), a.b), a.a);

                fixed4 color = lerp(b, a, maxA);
                return color;
            }

            ENDCG
        }
    }
}
