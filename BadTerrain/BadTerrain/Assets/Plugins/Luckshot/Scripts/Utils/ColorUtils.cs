using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ColorCombineType
{
	Multiply,
	Average,
	Add,
	Replace
}

[System.Serializable]
public struct ColorCombineOp
{
	public Color color;
	public ColorCombineType combineType;

	public ColorCombineOp(Color color, ColorCombineType combineType)
	{
		this.color = color;
		this.combineType = combineType;
	}

	public static implicit operator Color(ColorCombineOp colorCombineOp)
	{ return colorCombineOp.color; }

	public static implicit operator ColorCombineOp(Color color)
	{ return new ColorCombineOp(color, ColorCombineType.Multiply); }

	public Color Combine(Color baseColor)
	{ return baseColor.Combine(color, combineType); }
}

[System.Serializable]
public class LensManagerColorCombineOp : LensManager<ColorCombineOp>
{
	public LensManagerColorCombineOp(Func<List<ColorCombineOp>, ColorCombineOp> inEvaluateFunc) :
		base(inEvaluateFunc)
	{
	}

	public LensManagerColorCombineOp(LensManagerColorCombineOp other, Func<List<ColorCombineOp>, ColorCombineOp> inEvaluateFunc = null) : 
		base(other, inEvaluateFunc)
	{
	}

	public static implicit operator Color(LensManagerColorCombineOp colorCombineOpLensManager)
	{ return colorCombineOpLensManager.cachedResult.color; }
}

public static class ColorUtils
{
	public static Color SetR(this Color c, float r)
	{
		c.r = r;
		return c;
	}

	public static Color SetG(this Color c, float g)
	{
		c.g = g;
		return c;
	}

	public static Color SetB(this Color c, float b)
	{
		c.b = b;
		return c;
	}

	public static Color SetA(this Color c, float a)
	{
		c.a = a;
		return c;
	}

	public static Color32 SetR(this Color32 c, byte r)
	{
		c.r = r;
		return c;
	}

	public static Color32 SetG(this Color32 c, byte g)
	{
		c.g = g;
		return c;
	}

	public static Color32 SetB(this Color32 c, byte b)
	{
		c.b = b;
		return c;
	}

	public static Color32 SetA(this Color32 c, byte a)
	{
		c.a = a;
		return c;
	}

	public static Color Combine(this Color baseColor, Color combineColor, ColorCombineType combineType = ColorCombineType.Multiply)
	{
		switch (combineType)
		{
			case ColorCombineType.Add:
				return baseColor + combineColor;
			case ColorCombineType.Average:
				return (baseColor + combineColor) / 2f;
			case ColorCombineType.Multiply:
				return baseColor * combineColor;
			case ColorCombineType.Replace:
				return combineColor;
		}

		return baseColor;
	}

	public static ColorCombineOp Combine(List<ColorCombineOp> inRequests, Color? inDefault = null)
	{
		if (!inDefault.HasValue)
			inDefault = new Color(1f, 1f, 1f, 0f);

		if (inRequests.Count > 0)
		{
			Color color = inDefault.Value;
			for(int i =0; i < inRequests.Count; i++)
				color = color.Combine(inRequests[i].color, inRequests[i].combineType);

			return new ColorCombineOp(color, ColorCombineType.Replace);
		}
		else
		{
			return new ColorCombineOp(inDefault.Value, ColorCombineType.Replace);
		}
	}

	public static HSVColor ToHSV(this Color color)
	{ return RGBToHSV(color); }

	public static Color ToRGB(this HSVColor color)
	{ return HSVToRGB(color); }

	public static HSVColor RGBToHSV(Color color)
	{
		float h, s, v;
		Color.RGBToHSV(color, out h, out s, out v);
		return new HSVColor(h, s, v, color.a);
	}

	public static Color HSVToRGB(HSVColor hsvColor)
	{ return Color.HSVToRGB(hsvColor.h, hsvColor.s, hsvColor.v); }

	public static string ColorToRGBHex(Color color)
	{
		string rHex = ((int)(color.r * 255)).ToString("x2");
		string gHex = ((int)(color.g * 255)).ToString("x2");
		string bHex = ((int)(color.b * 255)).ToString("x2");
		return rHex + gHex + bHex;
	}

	public static Color InvertColor(Color c)
	{ return new Color(1f - c.r, 1f - c.g, 1f - c.b, c.a); }

	public static Color Random()
	{ return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)); }

	public static Color32 Random32()
	{ return Random(); }

	public static int CompareColors(Color a, Color b)
	{ return (a.r + a.g + a.b + a.a).CompareTo(b.r + b.g + b.b + b.a); }

	public static int CompareColors32(Color32 a, Color32 b)
	{ return (a.r + a.g + a.b + a.a).CompareTo(b.r + b.g + b.b + b.a); }

	public static float ColorDistance(Color a, Color b)
	{ return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a); }

	public static Texture2D CreateGradientTexture(Gradient gradient, int width)
	{
		var ramp = new Texture2D(width, 4, TextureFormat.RGBA32, true, true);
		var colors = GetPixelsFromGradient(gradient, width);
		ramp.SetPixels(colors);
		ramp.Apply(true);
		return ramp;
	}

	public static Color[] GetPixelsFromGradient(Gradient gradient, int width)
	{
		var pixels = new Color[width * 4];
		for (var x = 0; x < width; x++)
		{
			var delta = Mathf.Clamp01(x / (float)width);
			var col = gradient.Evaluate(delta);
			pixels[x + 0 * width] = col;
			pixels[x + 1 * width] = col;
			pixels[x + 2 * width] = col;
			pixels[x + 3 * width] = col;
		}
		return pixels;
	}
}

[System.Serializable]
public struct HSVColor
{
	public float h;
	public float s;
	public float v;
	public float a;

	public HSVColor(float h, float s, float v, float a = 1f)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		this.a = a;
	}

	public HSVColor(Color col)
	{
		HSVColor temp = ColorUtils.RGBToHSV(col);
		h = temp.h;
		s = temp.s;
		v = temp.v;
		a = temp.a;
	}

	public Color HSVToRGB()
	{ return ColorUtils.HSVToRGB(this); }

	public override string ToString()
	{
		return "H:" + h + " S:" + s + " V:" + v;
	}
}