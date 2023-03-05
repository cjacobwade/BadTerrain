#ifndef MATHUTILS_INCLUDED
#define MATHUTILS_INCLUDED

sampler2D _NoiseTex;

half _ClipAxis;
half _ClipDistance;
half4 _ClipPos;

float3 rgb2hsv(float3 c)
{
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c)
{
	c.g = max(c.g, 0.0); //make sure that saturation value is positive
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

inline float angleBetween(fixed4 vec, fixed4 vec2) {
     return acos(dot(vec, vec2)/(length(vec)*length(vec2))) * 360 / (3.14159 * 2);
 }

inline float inverselerp(float min, float max, float x)
{
	return (x - min) / (max - min);
}

float hash(float n)
{
	return frac(sin(n)*43758.5453);
}

float cheapVertNoise(float3 v)
{
	v *= 0.0301;
	return (float)tex2Dlod(_NoiseTex, float4(v.x + v.y, v.z - v.y, 0, 0)).r;
}

float cheapNoise(float3 v)
{
	v *= 0.0301;
	return (float)tex2D(_NoiseTex, float2(v.x + v.y, v.z - v.y)).r;
}

float noise(float3 v)
{
	// The noise function returns a value in the range 0.0f -> 1.0f

	float3 p = floor(v);
	float3 f = frac(v);

	f = f*f*(3.0 - 2.0*f);
	float n = p.x + p.y*57.0 + 113.0*p.z;

	return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
		lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
		lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
		lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
}

float3 rotatearoundaxis(float3 pos, float3 axis, float angle)
{
    angle = radians(angle);
    float s = sin(angle);
    float c = cos(angle);
    float one_minus_c = 1.0 - c;

    axis = normalize(axis);
    float3x3 rot_mat = 
    {   one_minus_c * axis.x * axis.x + c, one_minus_c * axis.x * axis.y - axis.z * s, one_minus_c * axis.z * axis.x + axis.y * s,
        one_minus_c * axis.x * axis.y + axis.z * s, one_minus_c * axis.y * axis.y + c, one_minus_c * axis.y * axis.z - axis.x * s,
        one_minus_c * axis.z * axis.x - axis.y * s, one_minus_c * axis.y * axis.z + axis.x * s, one_minus_c * axis.z * axis.z + c
    };

	return mul(rot_mat, pos);
}

void ClipWorldAxis(fixed3 worldPos)
{
	float3 directions[3] =
	{
		fixed3(1, 0, 0),
		fixed3(0, 1, 0),
		fixed3(0, 0, 1)
	};

	fixed3 offset = worldPos - _ClipPos.xyz;
	float3 axis = directions[_ClipAxis];
	float axisLength = dot(offset, normalize(axis));

	clip((_ClipDistance - axisLength) * sign(axisLength));
}

void ClipLocalAxis(fixed3 worldPos)
{
	float3 directions[3] =
	{
		mul(unity_ObjectToWorld, fixed4(1, 0, 0, 0)).xyz,
		mul(unity_ObjectToWorld, fixed4(0, 1, 0, 0)).xyz,
		mul(unity_ObjectToWorld, fixed4(0, 0, 1, 0)).xyz
	};

	fixed3 offset = worldPos - _ClipPos.xyz;
	float3 axis = directions[_ClipAxis];
	float axisLength = dot(offset, normalize(axis));

	clip((_ClipDistance - axisLength) * sign(axisLength));
}

#endif // MATHUTILS