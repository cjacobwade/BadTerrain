using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class BadTerrainOp_Splat : BadTerrainOp
{
	[SerializeField]
	private RenderTexture splatMapRT = null;

	[SerializeField]
	private Material splatMat = null;

	[SerializeField]
	private Material blurMat = null;

	[SerializeField]
	private Material maxMat = null;

	[SerializeField]
	private TerrainLayer terrainLayer = null;

	[SerializeField, Range(0f, 1f)]
	private float opacity = 1f;

	private readonly Color[] writeColors = new Color[]
	{
		new Color(1f, 0f, 0f, 0f),
		new Color(0f, 1f, 0f, 0f),
		new Color(0f, 0f, 1f, 0f),
		new Color(0f, 0f, 0f, 1f)
	};

	public override void RegisterOperation(BadTerrain badTerrain)
	{
		base.RegisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_Alphamap += SetupCommandBuffer_Alphamap;
	}

	public override void UnregisterOperation(BadTerrain badTerrain)
	{
		base.UnregisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_Alphamap -= SetupCommandBuffer_Alphamap;
	}

	public override void StartRegenerate(BadTerrain badTerrain)
	{
		base.StartRegenerate(badTerrain);

		int textureSize = badTerrain.AlphamapRT.width;

		if (splatMapRT == null ||
			splatMapRT.width != textureSize ||
			splatMapRT.height != textureSize)
		{
			if (splatMapRT != null)
				DestroyImmediate(splatMapRT);

			splatMapRT = new RenderTexture(new RenderTextureDescriptor(textureSize, textureSize, RenderTextureFormat.ARGB32, 24, badTerrain.AlphamapRT.mipmapCount));
			splatMapRT.name = "SplatTex" + gameObject.GetInstanceID();
			splatMapRT.autoGenerateMips = true;
		}
	}

	private void SetupCommandBuffer_Alphamap(BadTerrain badTerrain, CommandBuffer cmd)
	{
		cmd.BeginSample("Splat");

		int textureSize = badTerrain.AlphamapRT.width;

		cmd.BeginSample("Render Splatters");

		cmd.SetRenderTarget(splatMapRT);
		cmd.ClearRenderTarget(true, true, Color.clear);

		int layerIndex = Array.IndexOf(badTerrain.Terrain.terrainData.terrainLayers, terrainLayer);
		Color color = writeColors[layerIndex] * opacity;

		for (int i = 0; i < renderers.Count; i++)
		{
			cmd.SetGlobalColor("_Color", color);
			cmd.DrawRenderer(renderers[i], splatMat);
		}

		cmd.EndSample("Render Splatters");

		cmd.BeginSample("Overlay Splat");

		int backBufferID = Shader.PropertyToID("_SplatOp_TempBuffer");
		cmd.GetTemporaryRT(backBufferID, new RenderTextureDescriptor(textureSize, textureSize, RenderTextureFormat.ARGB32, 24));
		cmd.SetRenderTarget(backBufferID);
		cmd.ClearRenderTarget(true, true, Color.clear);

		cmd.Blit(splatMapRT, backBufferID, blurMat);
		cmd.Blit(backBufferID, splatMapRT, blurMat);

		// NOTE blit target can't be passed in as other texture!
		cmd.SetGlobalTexture("_OtherTex", badTerrain.AlphamapRT);
		cmd.Blit(splatMapRT, backBufferID, maxMat, 0);
		cmd.Blit(backBufferID, badTerrain.AlphamapRT);

		cmd.ReleaseTemporaryRT(backBufferID);

		cmd.EndSample("Overlay Splat");

		cmd.EndSample("Splat");
	}
}
