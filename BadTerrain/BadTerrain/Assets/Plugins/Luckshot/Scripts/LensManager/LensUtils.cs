using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LensUtils
{
	public static T Priority<T>(List<T> inRequests, T inDefault = default(T))
	{
		if (inRequests.Count == 0)
			return inDefault;
		else
			return inRequests[0];
	}

	public static float Multiply(List<float> inRequests, float inDefault = 1f)
	{
		if (inRequests.Count > 0)
		{
			float product = inDefault;
			for (int i = 0; i < inRequests.Count; i++)
			{
				product *= inRequests[i];
			}
			return product;
		}
		else
		{
			return inDefault;
		}
	}

	public static int Multiply(List<int> inRequests, int inDefault = 1)
	{
		if (inRequests.Count > 0)
		{
			int product = inDefault;
			for (int i = 0; i < inRequests.Count; i++)
			{
				product *= inRequests[i];
			}
			return product;
		}
		else
		{
			return inDefault;
		}
	}

	public static Color Multiply(List<Color> inRequests, Color? inDefault = null)
	{
		if (!inDefault.HasValue)
			inDefault = new Color(1f, 1f, 1f, 0f);

		if (inRequests.Count > 0)
		{
			Color product = Color.white;
			for (int i = 0; i < inRequests.Count; i++)
			{
				product *= inRequests[i];
			}
			return product;
		}
		else
		{
			return inDefault.Value;
		}
	}

	public static float Average(List<float> inRequests, float inDefault = 1f)
	{
		if (inRequests.Count > 0)
		{
			float sum = 0f;
			for (int i = 0; i < inRequests.Count; i++)
			{
				sum += inRequests[i];
			}
			return sum / inRequests.Count;
		}
		else
		{
			return inDefault;
		}
	}

	public static int Average(List<int> inRequests, int inDefault = 1)
	{
		if (inRequests.Count > 0)
		{
			int sum = 0;
			for (int i = 0; i < inRequests.Count; i++)
			{
				sum += inRequests[i];
			}
			return sum / inRequests.Count;
		}
		else
		{
			return inDefault;
		}
	}

	public static Color Average(List<Color> inRequests, Color? inDefault = null)
	{
		if (!inDefault.HasValue)
			inDefault = new Color(1f, 1f, 1f, 0f);

		if (inRequests.Count > 0)
		{
			Color product = Color.clear;
			for (int i = 0; i < inRequests.Count; i++)
			{
				product += inRequests[i];
			}
			return product / inRequests.Count;
		}
		else
		{
			return inDefault.Value;
		}
	}

	public static float Add(List<float> inRequests, float inDefault = 0f)
	{
		if (inRequests.Count > 0)
		{
			float sum = inDefault;
			for (int i = 0; i < inRequests.Count; i++)
			{
				sum += inRequests[i];
			}
			return sum;
		}
		else
		{
			return inDefault;
		}
	}

	public static int Add(List<int> inRequests, int inDefault = 0)
	{
		if (inRequests.Count > 0)
		{
			int sum = inDefault;
			for (int i = 0; i < inRequests.Count; i++)
			{
				sum += inRequests[i];
			}
			return sum;
		}
		else
		{
			return inDefault;
		}
	}

	public static Color Add(List<Color> inRequests, Color? inDefault = null)
	{
		if (!inDefault.HasValue)
			inDefault = new Color(1f, 1f, 1f, 0f);

		if (inRequests.Count > 0)
		{
			Color product = Color.white;
			for (int i = 0; i < inRequests.Count; i++)
			{
				product += inRequests[i];
			}
			return product;
		}
		else
		{
			return inDefault.Value;
		}
	}

	public static float Max(List<float> inRequests, float inDefault = 1f)
	{
		if (inRequests.Count > 0)
		{
			float max = Mathf.NegativeInfinity;
			for (int i = 0; i < inRequests.Count; i++)
			{
				max = Mathf.Max(max, inRequests[i]);
			}
			return max;
		}
		else
		{
			return inDefault;
		}
	}

	public static int Max(List<int> inRequests, int inDefault = 1)
	{
		if (inRequests.Count > 0)
		{
			int max = int.MinValue;
			for (int i = 0; i < inRequests.Count; i++)
			{
				max = Mathf.Max(max, inRequests[i]);
			}
			return max;
		}
		else
		{
			return inDefault;
		}
	}

	public static float Min(List<float> inRequests, float inDefault = 1f)
	{
		if (inRequests.Count > 0)
		{
			float min = Mathf.Infinity;
			for (int i = 0; i < inRequests.Count; i++)
			{
				min = Mathf.Min(min, inRequests[i]);
			}
			return min;
		}
		else
		{
			return inDefault;
		}
	}

	public static int Min(List<int> inRequests, int inDefault = 1)
	{
		if (inRequests.Count > 0)
		{
			int min = int.MaxValue;
			for (int i = 0; i < inRequests.Count; i++)
			{
				min = Mathf.Min(min, inRequests[i]);
			}
			return min;
		}
		else
		{
			return inDefault;
		}
	}

	public static bool AllTrue(List<bool> inRequests, bool inDefault = true)
	{
		if (inRequests.Count > 0)
		{
			for (int i = 0; i < inRequests.Count; i++)
			{
				if (!inRequests[i]) return false;
			}
			return true;
		}
		else
		{
			return inDefault;
		}
	}

	public static bool AllFalse(List<bool> inRequests, bool inDefault = false)
	{
		if (inRequests.Count > 0)
		{
			for (int i = 0; i < inRequests.Count; i++)
			{
				if (inRequests[i]) return false;
			}
			return true;
		}
		else
		{
			return inDefault;
		}
	}

	public static bool AnyTrue(List<bool> inRequests, bool inDefault = false)
	{
		if (inRequests.Count > 0)
		{
			for (int i = 0; i < inRequests.Count; i++)
			{
				if (inRequests[i]) return true;
			}
			return false;
		}
		else
		{
			return inDefault;
		}
	}

	public static bool AnyFalse(List<bool> inRequests, bool inDefault = true)
	{
		if (inRequests.Count > 0)
		{
			for (int i = 0; i < inRequests.Count; i++)
			{
				if (!inRequests[i]) return true;
			}
			return false;
		}
		else
		{
			return inDefault;
		}
	}

	public static CursorLockMode MouseCursor(List<CursorLockMode> inRequests, CursorLockMode inDefault = CursorLockMode.Locked)
	{
		if (inRequests.Count > 0)
		{
			for (int i = 0; i < inRequests.Count; i++)
			{
				if (inRequests[i] == CursorLockMode.None)
					return CursorLockMode.None;
			}

			for (int i = 0; i < inRequests.Count; i++)
			{
				if (inRequests[i] == CursorLockMode.Confined)
					return CursorLockMode.Confined;
			}

			return CursorLockMode.Locked;
		}
		else
			return inDefault;
	}
}

[System.Serializable]
public class LensManagerBool : LensManager<bool>
{
	public LensManagerBool(Func<List<bool>, bool> inEvaluateFunc) : base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerFloat : LensManager<float>
{
	public LensManagerFloat(Func<List<float>, float> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerInt : LensManager<int>
{
	public LensManagerInt(Func<List<int>, int> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerVector2 : LensManager<Vector2>
{
	public LensManagerVector2(Func<List<Vector2>, Vector2> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerVector3 : LensManager<Vector3>
{
	public LensManagerVector3(Func<List<Vector3>, Vector3> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerVector4 : LensManager<Vector4>
{
	public LensManagerVector4(Func<List<Vector4>, Vector4> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerQuaternion : LensManager<Quaternion>
{
	public LensManagerQuaternion(Func<List<Quaternion>, Quaternion> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerColor : LensManager<Color>
{
	public LensManagerColor(Func<List<Color>, Color> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerString : LensManager<string>
{
	public LensManagerString(Func<List<string>, string> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerTransform : LensManager<Transform>
{
	public LensManagerTransform(Func<List<Transform>, Transform> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerAnimationClip : LensManager<AnimationClip>
{
	public LensManagerAnimationClip(Func<List<AnimationClip>, AnimationClip> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}

[System.Serializable]
public class LensManagerRenderer : LensManager<Renderer>
{
	public LensManagerRenderer(Func<List<Renderer>, Renderer> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}
}