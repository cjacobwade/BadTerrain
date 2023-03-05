using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[System.Serializable]
public class Lens
{
	public int Priority = 0;
	public object Context;

#if UNITY_EDITOR
	public string Name;

	[SerializeField]
	private int instanceID = -1;
#endif

	protected Lens(object context, string name = default, int priority = 0)
	{
		Debug.Assert(context != null, "Invalid context passed to a lens handle.");

		Context = context;
		Priority = priority;

#if UNITY_EDITOR
		// Used for debugging
		Name = string.IsNullOrEmpty(name) ? context.ToString() : name;

		if (context is IConvertible)
		{
			Object obj = (Object)Convert.ChangeType(context, typeof(Object));
			if (obj != null)
				instanceID = obj.GetInstanceID();
		}
#endif
	}
}

public class Lens<T> : Lens
{
	public T Value;

	public Lens(object context, T value, string name = default(string), int priority = 0) :
		base(context, name, priority)
	{
		Value = value;
	}
}

// Need this for lens manager custom editor
public abstract class LensManagerBase
{ }

public class LensManager<T> : LensManagerBase
{
	public LensManager(Func<List<T>, T> inEvaluateFunc)
	{
		Debug.Assert(inEvaluateFunc != null, "Lens Manager's evaluate func is null. This is not allowed");
		evaluateFunc = inEvaluateFunc;
		EvaluateRequests();
	}

	public LensManager(LensManager<T> other, Func<List<T>, T> inEvaluateFunc = null)
	{
		evaluateFunc = inEvaluateFunc != null ? inEvaluateFunc : other.evaluateFunc;

		activeRequests = other.activeRequests;
		cachedResult = other.cachedResult;

		OnEvaluate = other.OnEvaluate;
		OnValueChanged = other.OnValueChanged;

		EvaluateRequests();
	}

	[SerializeField]
	protected T cachedResult = default(T);
	public T CachedResult => cachedResult;

	protected int requestCount = 0;
	public int GetRequestCount => requestCount;

	protected Func<List<T>, T> evaluateFunc = null;

	[SerializeField]
	private List<Lens> activeRequests = new List<Lens>();
	public List<Lens> ActiveRequests => activeRequests;

	[SerializeField]
	private List<T> evaluateValues = new List<T>();

	public event Action<T> OnEvaluate = delegate {};
	public event Action<T> OnValueChanged = delegate{};

	public static implicit operator T(LensManager<T> inLensManager)
	{ return inLensManager.cachedResult; }

	public void EvaluateRequests()
	{
		T prevCachedResult = cachedResult;

		evaluateValues.Clear();

		for (int i = 0; i < activeRequests.Count; i++)
		{
			if (activeRequests[i].Context == null)
			{
				activeRequests.RemoveAt(i--);
			}
			else
			{
				evaluateValues.Add((activeRequests[i] as Lens<T>).Value);
			}
		}

		requestCount = activeRequests.Count;
		cachedResult = evaluateFunc(evaluateValues);

		bool cachedResultNull = cachedResult == null;
		bool prevCachedResultNull = prevCachedResult == null;

		if (cachedResultNull != prevCachedResultNull || 
			(!cachedResultNull && !cachedResult.Equals(prevCachedResult)))
		{
			OnValueChanged(cachedResult);
		}

		OnEvaluate(cachedResult);
	}

	public bool AddRequest(Lens handle, bool evaluate = true)
	{
		if (!activeRequests.Contains(handle))
		{
			if(handle as Lens<T> == null)
			{
				Debug.LogErrorFormat("Trying to add {0} request to {1} lens manager.", handle.GetType(), typeof(T).ToString());
			}

			bool added = false;
			for (int i = 0; i < activeRequests.Count; i++)
			{
				if (handle.Priority >= activeRequests[i].Priority)
				{
					activeRequests.Insert(i, handle);
					added = true;
					break;
				}
			}

			if (!added)
				activeRequests.Add(handle);

			if (evaluate)
				EvaluateRequests();

			return true;
		}

		return false;
	}

	public bool RemoveRequest(Lens handle, bool evaluate = true)
	{
		if (activeRequests.Remove(handle))
		{
			if(evaluate)
				EvaluateRequests();

			return true;
		}

		return false;
	}

	public bool RemoveRequestsWithContext(object inContext, bool evaluate = true)
	{
		bool anyRemoved = false;
		for (int i = 0; i < activeRequests.Count; i++)
		{
			if (activeRequests[i].Context == inContext)
			{
				activeRequests.RemoveAt(i--);
				anyRemoved = true;
			}
		}

		if (anyRemoved && evaluate)
			EvaluateRequests();

		return anyRemoved;
	}

	public void ClearRequests()
	{
		activeRequests.Clear();
		EvaluateRequests();
	}
}