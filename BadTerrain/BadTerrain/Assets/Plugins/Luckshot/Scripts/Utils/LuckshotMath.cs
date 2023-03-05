#define LUCKSHOT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ResetType
{
	Position = 1 << 0,
	Rotation = 1 << 1,
	Scale = 1 << 2,
	PositionRotation = Position | Rotation,
	All = Position | Rotation | Scale
}

public static class LuckshotMath
{
	public static float TAU = Mathf.PI * 2f;

	public static float Epsilon = 0.001f;

	// BITS
	public static bool CheckBit(int bit, int shouldContainBit)
	{ return (bit & shouldContainBit) == shouldContainBit; }

	// FLOATS
	public static bool AreEqual(float a, float b)
	{ return Mathf.Abs(a - b) < Mathf.Epsilon; }

	public static float InverseLerpUnclamped(float a, float b, float value)
	{ return (value - a) / (b - a); }

	public static float RSign()
	{ return UnityEngine.Random.value > 0.5f ? 1 : -1; }

	public static bool RBool()
	{ return UnityEngine.Random.value > 0.5f; }

	public static float WrapAngle(this float angle)
	{
		while (angle > 180f)
			angle -= 360f;

		while (angle < -180f)
			angle += 360f;

		return angle;
	}

	public static float TimeToLerp(float speed)
	{
		// This math was determined by calculating number of iterations
		// need to lerp some value most of the way towards a target 
		// with varying speeds and inputting that into a graph plotter 
		// to find a nearest fit equation. 
		// This isn't exact, but hopefully close enough to be useful

		return 180f * Mathf.Pow(speed, -1f) * Time.fixedDeltaTime;
	}

	// LISTS
	public static T First<T>(this List<T> list)
	{ return list.Count == 0 ? default(T) : list[0]; }

	public static T First<T>(this T[] array)
	{ return array.Length == 0 ? default(T) : array[0]; }

	public static T Last<T>(this List<T> list)
	{ return list.Count == 0 ? default(T) : list[list.Count - 1]; }

	public static T Last<T>(this T[] array)
	{ return array.Length == 0 ? default(T) : array[array.Length - 1]; }

	public static T Random<T>(this List<T> list)
	{ return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)]; }

	public static T Random<T>(this T[] array)
	{ return array.Length == 0 ? default(T) : array[UnityEngine.Random.Range(0, array.Length)]; }

	public static void Add<T>(this List<T> inList, params T[] inAdds)
	{
		for (int i = 0; i < inAdds.Length; i++)
			inList.Add(inAdds[i]);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		var count = list.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i)
		{
			var r = UnityEngine.Random.Range(i, count);
			var tmp = list[i];
			list[i] = list[r];
			list[r] = tmp;
		}
	}

	public static void Chaikin(ref Vector3[] pts, ref Vector3[] pts2)
	{
		pts2[0] = pts[0];
		pts2[pts2.Length - 1] = pts[pts.Length - 1];

		int j = 1;
		for (int i = 0; i < pts.Length - 2; i++)
		{
			pts2[j] = pts[i] + (pts[i + 1] - pts[i]) * 0.75f;
			pts2[j + 1] = pts[i + 1] + (pts[i + 2] - pts[i + 1]) * 0.25f;
			j += 2;
		}
	}

	// TRANSFORMS

	public static PoseInfo GetPose(this Transform transform)
	{ return new PoseInfo(transform.position, transform.rotation); }

	public static void LookAt2D(this Transform transform, Transform target)
	{ transform.LookAt2D(target.position); }

	public static void LookAt2D(this Transform transform, Vector3 position)
	{
		Vector3 directionToTarget = position - transform.position;
		float lookAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
		transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
	}


	public static Transform FindRecursive(this Transform transform, string name)
	{
		if (transform == null)
			return null;

		if (transform.name == name)
			return transform;

		Transform result = transform.Find(name);
		if (result != null)
			return result;

		foreach (Transform child in transform)
		{
			result = child.FindRecursive(name);
			if (result != null)
				return result;
		}

		return result;
	}

	public static void ResetLocals(this Transform transform, ResetType inResetType = ResetType.All)
	{
		if (LuckshotMath.CheckBit((int)inResetType, (int)ResetType.Position))
			transform.localPosition = Vector3.zero;

		if (LuckshotMath.CheckBit((int)inResetType, (int)ResetType.Rotation))
			transform.localRotation = Quaternion.identity;

		if (LuckshotMath.CheckBit((int)inResetType, (int)ResetType.Scale))
			transform.localScale = Vector3.one;
	}

	// VECTORS
	public static bool AreEqual(Color32 a, Color32 b)
	{
		return Mathf.Abs(a.r - b.r) < Mathf.Epsilon &&
			Mathf.Abs(a.b - b.b) < Mathf.Epsilon &&
			Mathf.Abs(a.g - b.g) < Mathf.Epsilon &&
			Mathf.Abs(a.a - b.a) < Mathf.Epsilon;
	}

	public static Vector2 SetX(this Vector2 vec, float x)
	{
		vec.x = x;
		return vec;
	}

	public static Vector2 SetY(this Vector2 vec, float y)
	{
		vec.y = y;
		return vec;
	}

	public static Vector3 SetX(this Vector3 vec, float x)
	{
		vec.x = x;
		return vec;
	}

	public static Vector3 SetY(this Vector3 vec, float y)
	{
		vec.y = y;
		return vec;
	}

	public static Vector3 SetXY(this Vector3 vec, Vector2 xy)
	{
		vec.x = xy.x;
		vec.y = xy.y;
		return vec;
	}

	public static Vector3 SetXZ(this Vector3 vec, Vector2 xz)
	{
		vec.x = xz.x;
		vec.z = xz.y;
		return vec;
	}

	public static Vector3 SetYZ(this Vector3 vec, Vector2 yz)
	{
		vec.y = yz.x;
		vec.z = yz.y;
		return vec;
	}

	public static Vector3 SetXY(this Vector3 vec, float x, float y)
	{
		vec.x = x;
		vec.y = y;
		return vec;
	}

	public static Vector3 SetXZ(this Vector3 vec, float x, float z)
	{
		vec.x = x;
		vec.z = z;
		return vec;
	}

	public static Vector3 SetYZ(this Vector3 vec, float y, float z)
	{
		vec.z = z;
		vec.y = y;
		return vec;
	}

	public static Vector3 SetZ(this Vector3 vec, float z)
	{
		vec.z = z;
		return vec;
	}

	public static Vector4 SetX(this Vector4 vec, float x)
	{
		vec.x = x;
		return vec;
	}

	public static Vector4 SetY(this Vector4 vec, float y)
	{
		vec.y = y;
		return vec;
	}

	public static Vector4 SetZ(this Vector4 vec, float z)
	{
		vec.z = z;
		return vec;
	}

	public static Vector4 SetW(this Vector4 vec, float w)
	{
		vec.w = w;
		return vec;
	}

	public static Vector4 SetXY(this Vector4 vec, Vector2 xy)
	{
		vec.x = xy.x;
		vec.y = xy.y;
		return vec;
	}

	public static Vector4 SetXZ(this Vector4 vec, Vector2 xz)
	{
		vec.x = xz.x;
		vec.z = xz.y;
		return vec;
	}

	public static Vector4 SetYZ(this Vector4 vec, Vector2 yz)
	{
		vec.y = yz.x;
		vec.z = yz.y;
		return vec;
	}

	public static Vector4 SetXW(this Vector4 vec, Vector2 xw)
	{
		vec.x = xw.x;
		vec.w = xw.y;
		return vec;
	}

	public static Vector4 SetYW(this Vector4 vec, Vector2 yw)
	{
		vec.y = yw.x;
		vec.w = yw.y;
		return vec;
	}

	public static Vector4 SetZW(this Vector4 vec, Vector2 zw)
	{
		vec.z = zw.x;
		vec.w = zw.y;
		return vec;
	}

	public static Vector4 SetXY(this Vector4 vec, float x, float y)
	{
		vec.x = x;
		vec.y = y;
		return vec;
	}

	public static Vector4 SetXZ(this Vector4 vec, float x, float z)
	{
		vec.x = x;
		vec.z = z;
		return vec;
	}

	public static Vector4 SetYZ(this Vector4 vec, float y, float z)
	{
		vec.z = z;
		vec.y = y;
		return vec;
	}

	public static Vector4 SetXW(this Vector4 vec, float x, float w)
	{
		vec.x = x;
		vec.w = w;
		return vec;
	}

	public static Vector4 SetYW(this Vector4 vec, float y, float w)
	{
		vec.w = w;
		vec.y = y;
		return vec;
	}

	public static Vector4 SetZW(this Vector4 vec, float z, float w)
	{
		vec.w = w;
		vec.z = z;
		return vec;
	}

	public static Vector4 SetXYZ(this Vector4 vec, Vector3 xyz)
	{
		vec.x = xyz.x;
		vec.y = xyz.y;
		vec.z = xyz.z;
		return vec;
	}

	public static Vector4 SetXYW(this Vector4 vec, Vector3 xyw)
	{
		vec.x = xyw.x;
		vec.y = xyw.y;
		vec.w = xyw.z;
		return vec;
	}

	public static Vector4 SetXZW(this Vector4 vec, Vector3 xzw)
	{
		vec.x = xzw.x;
		vec.z = xzw.y;
		vec.w = xzw.z;
		return vec;
	}

	public static Vector4 SetYZW(this Vector4 vec, Vector3 yzw)
	{
		vec.y = yzw.x;
		vec.z = yzw.y;
		vec.w = yzw.z;
		return vec;
	}

	public static Vector4 SetXYZ(this Vector4 vec, float x, float y, float z)
	{
		vec.x = x;
		vec.y = y;
		vec.z = z;
		return vec;
	}

	public static Vector4 SetXYW(this Vector4 vec, float x, float y, float w)
	{
		vec.x = x;
		vec.y = y;
		vec.w = w;
		return vec;
	}

	public static Vector4 SetXZW(this Vector4 vec, float x, float z, float w)
	{
		vec.x = x;
		vec.z = z;
		vec.w = w;
		return vec;
	}

	public static Vector4 SetYZW(this Vector4 vec, float y, float z, float w)
	{
		vec.y = y;
		vec.z = z;
		vec.w = w;
		return vec;
	}

	public static float Max(this Vector2 vec)
	{ return Mathf.Max(vec.x, vec.y); }

	public static float Max(this Vector3 vec)
	{ return Mathf.Max(vec.x, vec.y, vec.z); }

	public static float Max(this Vector4 vec)
	{ return Mathf.Max(vec.x, vec.y, vec.z, vec.w); }

	public static float Min(this Vector2 vec)
	{ return Mathf.Min(vec.x, vec.y); }

	public static float Min(this Vector3 vec)
	{ return Mathf.Min(vec.x, vec.y, vec.z); }

	public static float Min(this Vector4 vec)
	{ return Mathf.Min(vec.x, vec.y, vec.z, vec.w); }

	public static Vector2 Range(Vector2 a, Vector2 b)
	{
		return new Vector2(
			UnityEngine.Random.Range(a.x, b.x),
			UnityEngine.Random.Range(a.y, b.y));
	}

	public static Vector3 Range(Vector3 a, Vector3 b)
	{
		return new Vector3(
			UnityEngine.Random.Range(a.x, b.x),
			UnityEngine.Random.Range(a.y, b.y),
			UnityEngine.Random.Range(a.z, b.z));
	}

	public static Vector4 Range(Vector4 a, Vector4 b)
	{
		return new Vector4(
			UnityEngine.Random.Range(a.x, b.x),
			UnityEngine.Random.Range(a.y, b.y),
			UnityEngine.Random.Range(a.z, b.z),
			UnityEngine.Random.Range(a.w, b.w));
	}

	public static bool IsInRange(int orig, int min, int max)
	{ return orig >= min && orig <= max; }

	public static bool IsInRange(float orig, float min, float max)
	{ return orig > min && orig < max; }

	public static bool IsInRange(Vector3 orig, Vector3 min, Vector3 max)
	{
		return IsInRange(orig.x, min.x, max.x) &&
				IsInRange(orig.y, min.y, max.y) &&
				IsInRange(orig.z, min.z, max.z);
	}

	public static bool IsInRange(Vector2 orig, Vector2 min, Vector2 max)
	{
		return IsInRange(orig.x, min.x, max.x) &&
				IsInRange(orig.y, min.y, max.y);
	}

	public static Vector2 Clamp(Vector2 orig, Vector2 min, Vector2 max)
	{
		return new Vector2(Mathf.Clamp(orig.x, min.x, max.x),
							Mathf.Clamp(orig.y, min.y, max.y));
	}

	public static Vector3 Clamp(Vector3 orig, Vector3 min, Vector3 max)
	{
		return new Vector3(Mathf.Clamp(orig.x, min.x, max.x),
							Mathf.Clamp(orig.y, min.y, max.y),
							Mathf.Clamp(orig.z, min.z, max.z));
	}

	public static Vector4 Clamp(Vector4 orig, Vector4 min, Vector4 max)
	{
		return new Vector4(Mathf.Clamp(orig.x, min.x, max.x),
						   Mathf.Clamp(orig.y, min.y, max.y),
						   Mathf.Clamp(orig.z, min.z, max.z),
						   Mathf.Clamp(orig.w, min.w, max.w));
	}

	public static Vector2 Abs(Vector2 v)
	{
		v.x = Mathf.Abs(v.x);
		v.y = Mathf.Abs(v.y);
		return v;
	}

	public static Vector3 Abs(Vector3 v)
	{
		v.x = Mathf.Abs(v.x);
		v.y = Mathf.Abs(v.y);
		v.z = Mathf.Abs(v.z);
		return v;
	}

	public static Vector4 Abs(Vector4 v)
	{
		v.x = Mathf.Abs(v.x);
		v.y = Mathf.Abs(v.y);
		v.z = Mathf.Abs(v.z);
		v.w = Mathf.Abs(v.w);
		return v;
	}

	public static Vector2 RandomBetween(Vector2 a, Vector2 b)
	{
		Vector2 retVal = Vector2.zero;
		retVal.x = UnityEngine.Random.Range(a.x, b.x);
		retVal.y = UnityEngine.Random.Range(a.y, b.y);
		return retVal;
	}

	public static Vector3 RandomBetween(Vector3 a, Vector3 b)
	{
		Vector3 retVal = Vector3.zero;
		retVal.x = UnityEngine.Random.Range(a.x, b.x);
		retVal.y = UnityEngine.Random.Range(a.y, b.y);
		retVal.z = UnityEngine.Random.Range(a.z, b.z);
		return retVal;
	}

	public static Vector4 RandomBetween(Vector4 a, Vector4 b)
	{
		Vector4 retVal = Vector4.zero;
		retVal.x = UnityEngine.Random.Range(a.x, b.x);
		retVal.y = UnityEngine.Random.Range(a.y, b.y);
		retVal.z = UnityEngine.Random.Range(a.z, b.z);
		retVal.w = UnityEngine.Random.Range(a.w, b.w);
		return retVal;
	}

	public static Vector2 Random(this Vector2 v)
	{
		v.x *= UnityEngine.Random.value;
		v.y *= UnityEngine.Random.value;
		return v;
	}

	public static Vector3 Random(this Vector3 v)
	{
		v.x *= UnityEngine.Random.value;
		v.y *= UnityEngine.Random.value;
		v.z *= UnityEngine.Random.value;
		return v;
	}

	public static Vector4 Random(this Vector4 v)
	{
		v.x *= UnityEngine.Random.value;
		v.y *= UnityEngine.Random.value;
		v.z *= UnityEngine.Random.value;
		v.w *= UnityEngine.Random.value;
		return v;
	}

	public static Vector2 Round(this Vector2 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		return v;
	}

	public static Vector3 Round(this Vector3 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}

	public static Vector4 Round(this Vector4 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		v.w = Mathf.Round(v.w);
		return v;
	}

	public static Vector2 Round(this Vector2 v, int decimals)
	{
		v.x = (float)Math.Round(v.x, decimals);
		v.y = (float)Math.Round(v.y, decimals);
		return v;
	}

	public static Vector3 Round(this Vector3 v, int decimals)
	{
		v.x = (float)Math.Round(v.x, decimals);
		v.y = (float)Math.Round(v.y, decimals);
		v.z = (float)Math.Round(v.z, decimals);
		return v;
	}

	public static Vector4 Round(this Vector4 v, int decimals)
	{
		v.x = (float)Math.Round(v.x, decimals);
		v.y = (float)Math.Round(v.y, decimals);
		v.z = (float)Math.Round(v.z, decimals);
		v.w = (float)Math.Round(v.w, decimals);
		return v;
	}

	public static Vector3 Orthogonal(this Vector3 v)
	{ return new Vector3(v.y, -v.x, v.z); }

	public static Vector2 Orthogonal(this Vector2 v)
	{ return new Vector2(v.y, -v.x); }

	public static Bounds ToWorldSpace(this Bounds localBounds, Transform relativeTo)
	{
		Vector3 center = relativeTo.TransformPoint(localBounds.center);

		// transform the local extents' axes
		Vector3 extents = localBounds.extents;
		Vector3 axisX = relativeTo.TransformVector(extents.x, 0, 0);
		Vector3 axisY = relativeTo.TransformVector(0, extents.y, 0);
		Vector3 axisZ = relativeTo.TransformVector(0, 0, extents.z);

		// sum their absolute value to get the world extents
		extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
		extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
		extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

		return new Bounds { center = center, extents = extents };
	}


	public static Vector3 GetWorldDirection(Axis axis)
	{
		switch (axis)
		{
			case Axis.X:
				return Vector3.right;
			case Axis.Y:
				return Vector3.up;
			case Axis.Z:
				return Vector3.forward;
		}

		return Vector3.zero;
	}

	public static Vector3 GetWorldDirection(this Transform t, Axis axis)
	{ return t.TransformDirection(GetWorldDirection(axis)); }
}

// DATA STRUCTURES
[System.Serializable]
public struct FloatRange
{
	public float min;
	public float max;

	public FloatRange(float inMin, float inMax)
	{
		min = inMin;
		max = inMax;
	}

	public FloatRange(FloatRange inFloatRange)
	{
		min = inFloatRange.min;
		max = inFloatRange.max;
	}

	public float Clamp(float v)
	{ return Mathf.Clamp(v, min, max); }

	public int Clamp(int v)
	{ return Mathf.Clamp(v, (int)min, (int)max); }

	public float Lerp(float t)
	{ return Mathf.Lerp(min, max, t); }

	public static FloatRange Lerp(FloatRange a, FloatRange b, float alpha)
	{
		a.min = Mathf.Lerp(a.min, b.min, alpha);
		a.max = Mathf.Lerp(a.max, b.max, alpha);
		return a;
	}

	public float UnclampedLerp(float t)
	{ return min + (max - min) * t; }

	public float InverseLerp(float value)
	{ return Mathf.InverseLerp(min, max, value); }

	public float UnclampedInverseLerp(float value)
	{ return (value - min) / (max - min); }

	public float Range
	{ get { return max - min; } }

	public float Random
	{ get { return UnityEngine.Random.Range(min, max); } }

	public bool Contains(float v)
	{ return v > min && v < max; }

	public bool Contains(int v)
	{ return v > min && v < max; }

	public float Midpoint
	{ get { return (min + max) * 0.5f; } }

	public static FloatRange operator +(FloatRange a, FloatRange b)
	{
		a.min += b.min;
		a.max += b.max;
		return a;
	}

	public static FloatRange operator *(FloatRange floatRange, float x)
	{
		floatRange.min *= x;
		floatRange.max *= x;
		return floatRange;
	}
}

[System.Serializable]
public struct IntRange
{
	public int min;
	public int max;

	public IntRange(int inMin, int inMax)
	{
		min = inMin;
		max = inMax;
	}

	public float Clamp(float v)
	{ return Mathf.Clamp(v, min, max); }

	public int Clamp(int v)
	{ return Mathf.Clamp(v, min, max); }

	public float Lerp(float t)
	{ return Mathf.Lerp(min, max, t); }

	public float UnclampedLerp(float t)
	{ return min + (max - min) * t; }

	public float InverseLerp(float value)
	{ return Mathf.InverseLerp(min, max, value); }

	public float UnclampedInverseLerp(float value)
	{ return (value - min) / (max - min); }

	public int Range
	{ get { return max - min; } }

	public int Random
	{ get { return UnityEngine.Random.Range(min, max); } }

	public bool Contains(float v)
	{ return v > min && v < max; }

	public bool Contains(int v)
	{ return v > min && v < max; }

	public float Midpoint
	{ get { return (min + max) * 0.5f; } }

	public static IntRange operator +(IntRange a, IntRange b)
	{
		a.min += b.min;
		a.max += b.max;
		return a;
	}

	public static IntRange operator *(IntRange intRange, int x)
	{
		intRange.min *= x;
		intRange.max *= x;
		return intRange;
	}
}

[System.Serializable]
public struct Vec2Range
{
	public Vector2 min;
	public Vector2 max;

	public Vec2Range(Vector2 _min, Vector2 _max)
	{
		min = _min;
		max = _max;
	}

	public Vector2 Clamp(Vector2 v)
	{
		return new Vector2(Mathf.Clamp(v.x, min.x, max.x),
						  Mathf.Clamp(v.y, min.y, max.y));
	}

	public Vector2 Lerp(float t)
	{ return Vector2.Lerp(min, max, t); }

	public Vector2 UnclampedLerp(float t)
	{ return min + (max - min) * t; }

	public Vector2 Range
	{ get { return max - min; } }

	public Vector2 Random
	{
		get
		{
			return new Vector2(UnityEngine.Random.Range(min.x, max.x),
				UnityEngine.Random.Range(min.y, max.y));
		}
	}

	public bool Contains(Vector2 v)
	{
		return v.x > min.x && v.x < max.x &&
			v.y > min.y && v.y < max.y;
	}

	public Vector2 Midpoint
	{ get { return (min + max) * 0.5f; } }

	public static Vec2Range operator +(Vec2Range a, Vec2Range b)
	{
		a.min += b.min;
		a.max += b.max;
		return a;
	}

	public static Vec2Range operator *(Vec2Range floatRange, float x)
	{
		floatRange.min *= x;
		floatRange.max *= x;
		return floatRange;
	}

	public static Vec2Range Lerp(Vec2Range a, Vec2Range b, float alpha)
	{
		a.min = Vector2.Lerp(a.min, b.min, alpha);
		a.max = Vector2.Lerp(a.max, b.max, alpha);
		return a;
	}
}

[System.Serializable]
public struct Vec3Range
{
	public Vector3 min;
	public Vector3 max;

	public Vec3Range(Vector3 _min, Vector3 _max)
	{
		min = _min;
		max = _max;
	}

	public Vector3 Clamp(Vector3 v)
	{
		return new Vector4(Mathf.Clamp(v.x, min.x, max.x),
						 Mathf.Clamp(v.y, min.y, max.y),
						 Mathf.Clamp(v.z, min.z, max.z));
	}

	public Vector3 Lerp(float t)
	{ return Vector3.Lerp(min, max, t); }

	public Vector3 UnclampedLerp(float t)
	{ return min + (max - min) * t; }

	public Vector3 Range
	{ get { return max - min; } }

	public Vector3 Random
	{
		get
		{
			return new Vector3(UnityEngine.Random.Range(min.x, max.x),
				UnityEngine.Random.Range(min.y, max.y),
				UnityEngine.Random.Range(min.z, max.z));
		}
	}

	public bool Contains(Vector3 v)
	{
		return v.x > min.x && v.x < max.x &&
			v.y > min.y && v.y < max.y &&
			v.z > min.z && v.z < max.z;
	}

	public Vector3 Midpoint
	{ get { return (min + max) * 0.5f; } }

	public static Vec3Range operator +(Vec3Range a, Vec3Range b)
	{
		a.min += b.min;
		a.max += b.max;
		return a;
	}

	public static Vec3Range operator *(Vec3Range floatRange, float x)
	{
		floatRange.min *= x;
		floatRange.max *= x;
		return floatRange;
	}

	public static Vec3Range Lerp(Vec3Range a, Vec3Range b, float alpha)
	{
		a.min = Vector3.Lerp(a.min, b.min, alpha);
		a.max = Vector3.Lerp(a.max, b.max, alpha);
		return a;
	}
}

[System.Serializable]
public struct Vec4Range
{
	public Vector4 min;
	public Vector4 max;

	public Vec4Range(Vector4 _min, Vector4 _max)
	{
		min = _min;
		max = _max;
	}

	public Vector4 Clamp(Vector4 v)
	{
		return new Vector4(Mathf.Clamp(v.x, min.x, max.x),
					   Mathf.Clamp(v.y, min.y, max.y),
					   Mathf.Clamp(v.z, min.z, max.z),
					   Mathf.Clamp(v.w, min.w, max.w)); ;
	}

	public Vector4 Lerp(float t)
	{ return Vector4.Lerp(min, max, t); }

	public Vector4 UnclampedLerp(float t)
	{ return min + (max - min) * t; }

	public Vector4 Range
	{ get { return max - min; } }

	public Vector4 Random
	{
		get
		{
			return new Vector4(UnityEngine.Random.Range(min.x, max.y),
				UnityEngine.Random.Range(min.y, max.y),
				UnityEngine.Random.Range(min.z, max.z),
				UnityEngine.Random.Range(min.w, max.w));
		}
	}

	public bool Contains(Vector4 v)
	{
		return v.x > min.x && v.x < max.x &&
			v.y > min.y && v.y < max.y &&
			v.z > min.z && v.z < max.z &&
			v.w > min.w && v.w < max.w;
	}

	public Vector4 Midpoint
	{ get { return (min + max) * 0.5f; } }

	public static Vec4Range operator +(Vec4Range a, Vec4Range b)
	{
		a.min += b.min;
		a.max += b.max;
		return a;
	}

	public static Vec4Range operator *(Vec4Range floatRange, float x)
	{
		floatRange.min *= x;
		floatRange.max *= x;
		return floatRange;
	}

	public static Vec4Range Lerp(Vec4Range a, Vec4Range b, float alpha)
	{
		a.min = Vector4.Lerp(a.min, b.min, alpha);
		a.max = Vector4.Lerp(a.max, b.max, alpha);
		return a;
	}
}

[System.Serializable]
public struct PoseInfo
{
	public Vector3 position;
	public Quaternion rotation;

	public PoseInfo(Vector3 inPosition, Quaternion inRotation)
	{
		position = inPosition;
		rotation = inRotation;
	}

	public void Lerp(PoseInfo a, PoseInfo b, float alpha)
	{
		position = Vector3.Lerp(a.position, b.position, alpha);
		rotation = Quaternion.Slerp(a.rotation, b.rotation, alpha);
	}
}

public enum Axis
{
	X,
	Y,
	Z
}