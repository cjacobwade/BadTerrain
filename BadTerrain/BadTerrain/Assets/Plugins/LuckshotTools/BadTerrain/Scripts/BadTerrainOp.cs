using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BadTerrainOp : MonoBehaviour
{
	[SerializeField, HideInInspector]
	protected bool editMode = false;

	protected List<Renderer> renderers = new List<Renderer>();

	public virtual void RegisterOperation(BadTerrain badTerrain)
	{
		badTerrain.StartRegenerate += StartRegenerate;
		badTerrain.BeforeExecuteCommandBuffer += BeforeExecuteCommandBuffer;
		badTerrain.AfterExecuteCommandBuffer += AfterExecuteCommandBuffer;
	}

	public virtual void UnregisterOperation(BadTerrain badTerrain)
	{
		badTerrain.StartRegenerate -= StartRegenerate;
		badTerrain.BeforeExecuteCommandBuffer -= BeforeExecuteCommandBuffer;
		badTerrain.AfterExecuteCommandBuffer -= AfterExecuteCommandBuffer;
	}
	
	protected void CacheRenderers()
	{
		renderers.Clear();
		renderers.AddRange(transform.GetComponentsInChildren<Renderer>(false));

		for(int i = 0; i < renderers.Count; i++)
		{
			var rend = renderers[i];
			Vector3 scale = rend.transform.localScale;
			if(Mathf.Sign(scale.x) * Mathf.Sign(scale.y) * Mathf.Sign(scale.z) < 0f)
			{
				Debug.LogErrorFormat(rend, "{0} has negative transform scale. Command buffer will fail to draw, so removing from renderer list", rend);
				renderers.RemoveAt(i--);
			}
		}
	}

	public void SetEditMode(bool setEditMode)
	{
		editMode = setEditMode;

		CacheRenderers();

		foreach (var renderer in renderers)
			renderer.enabled = setEditMode;
	}

	public void Regenerate()
	{
		BadTerrain badTerrain = GetComponentInParent<BadTerrain>();
		if (badTerrain != null)
			badTerrain.Regenerate();
	}

	public virtual void StartRegenerate(BadTerrain badTerrain)
	{
		CacheRenderers();
	}

	public virtual void BeforeExecuteCommandBuffer(BadTerrain badTerrain, CommandBuffer cmd)
	{
		foreach (var renderer in renderers)
			renderer.enabled = true;
	}

	public virtual void AfterExecuteCommandBuffer(BadTerrain badTerrain, CommandBuffer cmd)
	{
		foreach (var renderer in renderers)
			renderer.enabled = false;
	}
}
