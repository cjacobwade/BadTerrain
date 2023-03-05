using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BadTerrainOp_Holes : BadTerrainOp
{
	[SerializeField]
	private RenderTexture holeMapRT = null;

	[SerializeField]
	private Material clipMat = null;

	[SerializeField]
	private Material blitClipMat = null;

	public override void RegisterOperation(BadTerrain badTerrain)
	{
		base.RegisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_HeightmapAfterBlur += SetupCommandBuffer_AfterBlur;
	}

	public override void UnregisterOperation(BadTerrain badTerrain)
	{
		base.UnregisterOperation(badTerrain);
		badTerrain.SetupCommandBuffer_HeightmapAfterBlur -= SetupCommandBuffer_AfterBlur;
	}

	public override void StartRegenerate(BadTerrain badTerrain)
	{
		base.StartRegenerate(badTerrain);

		int textureSize = badTerrain.HolesRT.width;

		if (holeMapRT == null ||
			holeMapRT.width != textureSize ||
			holeMapRT.height != textureSize)
		{
			if (holeMapRT != null)
				DestroyImmediate(holeMapRT);

			holeMapRT = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.R8);
			holeMapRT.name = "BadTerrain_HoleMap";
		}
	}

	private void SetupCommandBuffer_AfterBlur(BadTerrain badTerrain, CommandBuffer cmd)
	{
		cmd.BeginSample("Holes");

		cmd.BeginSample("Render Clips");

		cmd.SetRenderTarget(holeMapRT);
		cmd.ClearRenderTarget(true, true, Color.clear);

		for (int i = 0; i < renderers.Count; i++)
		{
			cmd.SetGlobalColor("_Color", Color.white);
			cmd.DrawRenderer(renderers[i], clipMat);
		}

		cmd.EndSample("Render Clips");

		cmd.BeginSample("Cut Holes");

		int textureSize = badTerrain.HolesRT.width;

		int backBufferID = Shader.PropertyToID("_HoleOp_BackBuffer");
		cmd.GetTemporaryRT(backBufferID, textureSize, textureSize, 24, FilterMode.Bilinear, RenderTextureFormat.R8);
		cmd.SetRenderTarget(backBufferID);
		cmd.ClearRenderTarget(true, true, Color.black);

		cmd.SetGlobalTexture("_ClipTex", holeMapRT);

		cmd.Blit(badTerrain.HolesRT, backBufferID, blitClipMat);
		cmd.Blit(backBufferID, badTerrain.HolesRT);

		cmd.ReleaseTemporaryRT(backBufferID);

		cmd.EndSample("Cut Holes");

		cmd.EndSample("Holes");
	}
}
