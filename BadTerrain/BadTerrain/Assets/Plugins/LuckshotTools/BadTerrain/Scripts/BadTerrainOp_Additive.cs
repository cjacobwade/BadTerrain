using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BadTerrainOp_Additive : BadTerrainOp
{
	[SerializeField]
	private RenderTexture addMapRT = null;

	[SerializeField]
	private Material addMat = null;

	[SerializeField]
	private Material blitAddMat = null;

	[SerializeField, Range(-0.1f, 0.1f)]
	private float addStrength = 0.02f;

	public override void RegisterOperation(BadTerrain badTerrain)
	{
		base.RegisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_HeightmapBeforeBlur += SetupCommandBuffer_BeforeBlur;
	}

	public override void UnregisterOperation(BadTerrain badTerrain)
	{
		base.UnregisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_HeightmapBeforeBlur -= SetupCommandBuffer_BeforeBlur;
	}

	public override void StartRegenerate(BadTerrain badTerrain)
	{
		base.StartRegenerate(badTerrain);

		int textureSize = badTerrain.HeightmapRT.width;

		if (addMapRT == null ||
			addMapRT.width != textureSize ||
			addMapRT.height != textureSize)
		{
			if (addMapRT != null)
				DestroyImmediate(addMapRT);

			addMapRT = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.R16);
			addMapRT.name = "BadTerrain_AddMap";
		}
	}

	private void SetupCommandBuffer_BeforeBlur(BadTerrain badTerrain, CommandBuffer cmd)
	{
		cmd.BeginSample("Additive");

		cmd.BeginSample("Render Adds");

		cmd.SetRenderTarget(addMapRT);
		cmd.ClearRenderTarget(true, true, Color.clear);

		for (int i = 0; i < renderers.Count; i++)
		{
			cmd.SetGlobalColor("_Color", Color.white);
			cmd.DrawRenderer(renderers[i], addMat);
		}

		cmd.EndSample("Render Adds");

		cmd.BeginSample("Heightmap Add");

		int textureSize = badTerrain.HeightmapRT.width;

		int backBufferID = Shader.PropertyToID("_AddOp_BackBuffer");
		cmd.GetTemporaryRT(backBufferID, textureSize, textureSize, 24, FilterMode.Bilinear, RenderTextureFormat.R16);
		cmd.SetRenderTarget(backBufferID);
		cmd.ClearRenderTarget(true, true, Color.black);

		cmd.SetGlobalTexture("_AddTex", addMapRT);
		cmd.SetGlobalFloat("_Add", addStrength);

		cmd.Blit(badTerrain.HeightmapRT, backBufferID, blitAddMat);
		cmd.Blit(backBufferID, badTerrain.HeightmapRT);

		cmd.ReleaseTemporaryRT(backBufferID);

		cmd.EndSample("Heightmap Add");

		cmd.EndSample("Additive");
	}
}
