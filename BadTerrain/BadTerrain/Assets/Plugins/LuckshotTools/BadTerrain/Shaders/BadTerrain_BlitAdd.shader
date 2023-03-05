Shader "Bad Terrain/Blit Add" 
{
	Properties 
	{
		_MainTex("Main Tex", 2D) = "white"{}
		//_AddTex("Add Texture", 2D) = "gray"{}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		LOD 100

		Pass 
		{  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _AddTex;
			float _Add;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed4 add = tex2D(_AddTex, i.uv) * _Add;
				col.r += add;

				return col;
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}
