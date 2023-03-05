// Upgrade NOTE: upgraded instancing buffer 'MyProperties' to new syntax.

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Bad Terrain/Heightmap" 
{
	Properties 
	{
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
			#include "Assets\Shaders\MathUtils.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _CameraDepthTexture;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float4 color = i.screenPos.z / i.screenPos.w / 2.0;
				color = max(color, 0.01);
				color.a = 1;

				return color;
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}
