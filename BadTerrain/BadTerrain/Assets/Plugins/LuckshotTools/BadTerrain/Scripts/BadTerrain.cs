using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

[ExecuteInEditMode]
public class BadTerrain : MonoBehaviour
{
	public enum TerrainResolution
	{
		_33,
		_65,
		_129,
		_257,
		_513,
		_1025,
		_2049,
		_4097
	}

	public int GetResolution(TerrainResolution hr)
	{
		switch(hr)
		{
			case TerrainResolution._33:
				return 33;
			case TerrainResolution._65:
				return 65;
			case TerrainResolution._129:
				return 129;
			case TerrainResolution._257:
				return 257;
			case TerrainResolution._513:
				return 513;
			case TerrainResolution._1025:
				return 1025;
			case TerrainResolution._2049:
				return 2049;
			case TerrainResolution._4097:
				return 4097;
		}

		return -1;
	}

	private MaterialPropertyBlock propertyBlock = null;
	public MaterialPropertyBlock PropertyBlock
	{
		get
		{
			if (propertyBlock == null)
				propertyBlock = new MaterialPropertyBlock();

			return propertyBlock;
		}
	}

	private Terrain terrain = null;
	public Terrain Terrain
	{ 
		get
		{
			if (terrain == null)
				terrain = GetComponentInChildren<Terrain>();

			if (terrain == null)
			{
				terrain = new GameObject("Terrain", typeof(Terrain), typeof(TerrainCollider)).GetComponent<Terrain>();
				terrain.transform.SetParent(transform);
			}

			if (terrain.terrainData == null)
			{
				terrain.terrainData = new TerrainData();
				terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
			}

			if(terrain.transform.GetSiblingIndex() != 0)
				terrain.transform.SetAsFirstSibling();

			return terrain;
		}
	}

	private CommandBuffer cmd = null;
	private Camera fakeCam = null;

	[SerializeField]
	private RenderTexture heightmapRT = null;
	public RenderTexture HeightmapRT => heightmapRT;

	[SerializeField]
	private RenderTexture alphamapRT = null;
	public RenderTexture AlphamapRT => alphamapRT;

	[SerializeField]
	private RenderTexture holesRT = null;
	public RenderTexture HolesRT => holesRT;

	private List<BadTerrainOp> terrainOps = new List<BadTerrainOp>();
	private List<Renderer> renderers = new List<Renderer>();

	[Header("Terrain")]

	[SerializeField]
	private Material terrainMat = null;

	[SerializeField]
	private TerrainLayer[] terrainLayers = null;

	[SerializeField]
	private TerrainResolution heightmapResolution = TerrainResolution._1025;

	[SerializeField]
	private TerrainResolution alphamapResolution = TerrainResolution._513;

	[SerializeField]
	private float heightmapError = 5f;

	// Todo adjust this per platform

	[Header("Heightmap")]

	[SerializeField]
	private Material depthMat = null;

	[SerializeField]
	private Material blurMat = null;

	[SerializeField]
	private Material clipMat = null;

	[SerializeField]
	private Material ceilMat = null;

	[SerializeField, Range(0, 50)]
	private int blurIterations = 10;

	[SerializeField, HideInInspector]
	private bool editMode = false;

	public event System.Action<BadTerrain> StartRegenerate = delegate {};
	public event System.Action<BadTerrain> FinishRegenerate = delegate { };

	public event System.Action<BadTerrain, CommandBuffer> SetupCommandBuffer_AfterCamera = delegate { };
	public event System.Action<BadTerrain, CommandBuffer> SetupCommandBuffer_HeightmapBeforeBlur = delegate { };
	public event System.Action<BadTerrain, CommandBuffer> SetupCommandBuffer_HeightmapAfterBlur = delegate { };

	public event System.Action<BadTerrain, CommandBuffer> SetupCommandBuffer_Alphamap = delegate { };

	public event System.Action<BadTerrain, CommandBuffer> BeforeExecuteCommandBuffer = delegate { };
	public event System.Action<BadTerrain, CommandBuffer> AfterExecuteCommandBuffer = delegate { };

	// Use for debugging
// 	[ImageEffectOpaque]
// 	private void OnRenderImage(RenderTexture source, RenderTexture destination)
// 	{
// 		Regenerate();
// 
// 		Graphics.Blit(source, destination);
// 	}

	private void CacheTerrainOps()
	{
		foreach (var terrainOp in terrainOps)
			terrainOp.UnregisterOperation(this);

		terrainOps.Clear();
		terrainOps.AddRange(transform.GetComponentsInChildren<BadTerrainOp>(false));

		foreach (var terrainOp in terrainOps)
			terrainOp.RegisterOperation(this);
	}

	private void CacheRenderers()
	{
		var childRenderers = GetComponentsInChildren<Renderer>(false);
		renderers = childRenderers.Where(r =>
		{
			for (int i = 0; i < terrainOps.Count; i++)
			{
				if (r.transform.IsChildOf(terrainOps[i].transform))
					return false;
			}

			return true;
		}).ToList();
	}

	private Bounds CalculateBounds()
	{
		var bounds = new Bounds();

		CacheTerrainOps();
		CacheRenderers();

		if (renderers.Count > 0)
		{
			bounds = renderers[0].bounds;

			foreach (var renderer in renderers)
				bounds.Encapsulate(renderer.bounds);
		}

		return bounds;
	}

	public void SetEditMode(bool setEditMode)
	{
		editMode = setEditMode;

		CacheTerrainOps();
		CacheRenderers();

		foreach (var renderer in renderers)
			renderer.enabled = setEditMode;

		Terrain.enabled = !setEditMode;
	}

	public void ResetSplatmap()
	{
		if (Terrain.terrainData.alphamapTextureCount > 0)
		{
			Texture2D alphamap = Terrain.terrainData.GetAlphamapTexture(0);
			if (alphamap != null)
			{
				int alphaTexSize = alphamap.width;

				var emptyAlphamap = new RenderTexture(new RenderTextureDescriptor(alphaTexSize, alphaTexSize, RenderTextureFormat.ARGB32, 24, alphamap.mipmapCount));
				emptyAlphamap.name = "EmptyAlphaMap" + alphaTexSize;
				emptyAlphamap.useMipMap = true;
				emptyAlphamap.autoGenerateMips = false;

				var cmd = new CommandBuffer();
				cmd.name = "Clear Alphamap";

				cmd.SetRenderTarget(emptyAlphamap);
				cmd.ClearRenderTarget(true, true, Color.clear);
				cmd.GenerateMips(emptyAlphamap);

				cmd.CopyTexture(alphamap, emptyAlphamap);

				cmd.ClearRenderTarget(false, true, Color.red.SetA(0f));
				cmd.GenerateMips(emptyAlphamap);

				cmd.CopyTexture(emptyAlphamap, alphamap);

				if (alphamapRT != null)
					cmd.CopyTexture(emptyAlphamap, alphamapRT);

				Graphics.ExecuteCommandBuffer(cmd);

				Terrain.terrainData.DirtyTextureRegion("alphamap", new RectInt(0, 0, alphamap.width, alphamap.height), true);

				DestroyImmediate(emptyAlphamap);
			}
		}
	}

	public void Regenerate()
	{
		var bounds = CalculateBounds();
		Terrain.transform.position = bounds.min;
		Terrain.transform.rotation = Quaternion.identity;

		Terrain.materialTemplate = terrainMat;
		
		Terrain.terrainData.terrainLayers = terrainLayers;

		int alphaRes = GetResolution(alphamapResolution) - 1;
		if(Terrain.terrainData.alphamapResolution != alphaRes)
			Terrain.terrainData.alphamapResolution = alphaRes;

		int heightRes = GetResolution(heightmapResolution);
		if(Terrain.terrainData.heightmapResolution != heightRes)
			Terrain.terrainData.heightmapResolution = heightRes;

		Terrain.terrainData.size = bounds.size;

		Terrain.heightmapPixelError = heightmapError;
		Terrain.drawTreesAndFoliage = false;
		Terrain.shadowCastingMode = ShadowCastingMode.On;
		
		ResetSplatmap();

		Texture2D alphamap = Terrain.terrainData.GetAlphamapTexture(0);
		int alphaTexSize = alphamap.width;

		if (alphamapRT == null ||
			alphamapRT.width != alphaTexSize ||
			alphamapRT.height != alphaTexSize)
		{
			if (alphamapRT != null)
				DestroyImmediate(alphamapRT);

			alphamapRT = new RenderTexture(new RenderTextureDescriptor(alphaTexSize, alphaTexSize, RenderTextureFormat.ARGB32, 24, alphamap.mipmapCount));
			alphamapRT.name = "AlphaMap" + alphaTexSize;
			alphamapRT.useMipMap = true;
		}

		int holeTexSize = Terrain.terrainData.holesResolution;

		if (holesRT == null || 
			holesRT.width != holeTexSize ||
			holesRT.height != holeTexSize)
		{
			if (holesRT != null)
				DestroyImmediate(holesRT);

			holesRT = new RenderTexture(new RenderTextureDescriptor(holeTexSize, holeTexSize, RenderTextureFormat.R8, 24));
			holesRT.name = "HoleMap" + holeTexSize;
		}

		int heightTexSize = GetResolution(heightmapResolution);
		if (heightmapRT == null ||
			heightmapRT.width != heightTexSize ||
			heightmapRT.height != heightTexSize)
		{
			if (heightmapRT != null)
				DestroyImmediate(heightmapRT);

			heightmapRT = new RenderTexture(heightTexSize, heightTexSize, 24, RenderTextureFormat.R16);
			heightmapRT.name = "HeightMap" + heightTexSize;
			heightmapRT.useMipMap = false;
		}

		StartRegenerate(this);

		RenderTerrain();

		Graphics.CopyTexture(heightmapRT, Terrain.terrainData.heightmapTexture);
		Terrain.terrainData.DirtyHeightmapRegion(new RectInt(0, 0, heightmapRT.width, heightmapRT.height), TerrainHeightmapSyncControl.HeightAndLod);

		RenderTexture.active = alphamapRT;
		Terrain.terrainData.CopyActiveRenderTextureToTexture("alphamap", 0, new RectInt(0, 0, alphamap.width, alphamap.height), Vector2Int.zero, false);
		RenderTexture.active = null;

		Texture2D holeTex = new Texture2D(holeTexSize, holeTexSize, TextureFormat.R8, false, true);

		var prevRT = RenderTexture.active;
		RenderTexture.active = holesRT;
		holeTex.ReadPixels(new Rect(0, 0, holesRT.width, holesRT.height), 0, 0);
		holeTex.Apply();
		RenderTexture.active = prevRT;

		int numTrue = 0;

		var holes = new bool[holeTexSize, holeTexSize];
		for (int y = 0; y < holes.GetLength(0); y++)
		{
			for (int x = 0; x < holes.GetLength(1); x++)
			{
				holes[y, x] = holeTex.GetPixel(x, y).r > 0.5f;
				if (holes[y, x])
					numTrue++;
			}
		}

		Terrain.terrainData.SetHoles(0, 0, holes);

		FinishRegenerate(this);
	}

	private void SetupCamera()
	{
		var bounds = CalculateBounds();

		if (fakeCam == null)
		{
			fakeCam = new GameObject("BadTerrain_FakeCam_" + gameObject.scene.name, typeof(Camera)).GetComponent<Camera>();
			//fakeCam.gameObject.hideFlags = HideFlags.HideAndDontSave;
			fakeCam.orthographic = true;
			fakeCam.enabled = false;

			fakeCam.depthTextureMode = DepthTextureMode.Depth;
		}

		fakeCam.nearClipPlane = 0f;
		fakeCam.farClipPlane = bounds.size.y;

		Vector3 pos = bounds.center.SetY(bounds.max.y);
		Quaternion rot = Quaternion.Euler(90f, 0f, 0f);

		fakeCam.transform.SetPositionAndRotation(pos, rot);
		fakeCam.aspect = bounds.size.x / bounds.size.z;
		fakeCam.orthographicSize = bounds.size.z / 2f;

		cmd.SetViewProjectionMatrices(fakeCam.worldToCameraMatrix, fakeCam.projectionMatrix);

		SetupCommandBuffer_AfterCamera(this, cmd);
	}

	private void SetupHeightmapCommands()
	{
		cmd.BeginSample("Heightmap");

		cmd.BeginSample("Render Base");

		cmd.SetRenderTarget(heightmapRT);
		cmd.ClearRenderTarget(true, true, Color.black);

		for (int i = 0; i < renderers.Count; i++)
		{
			Renderer renderer = renderers[i];

			int numSubmeshes = 0;

			MeshRenderer mr = renderer as MeshRenderer;
			if (mr != null)
			{
				MeshFilter mf = mr.GetComponent<MeshFilter>();
				if(mf != null && mf.sharedMesh != null)
					numSubmeshes = mf.sharedMesh.subMeshCount;
			}
			else
            {
				SkinnedMeshRenderer smr = renderer as SkinnedMeshRenderer;
				if (smr != null)
					numSubmeshes = smr.sharedMesh.subMeshCount;
            }

			for(int j = 0; j < numSubmeshes; j++)
				cmd.DrawRenderer(renderers[i], depthMat, j);
		}

		cmd.EndSample("Render Base");

		SetupCommandBuffer_HeightmapBeforeBlur(this, cmd);

		var textureSize = GetResolution(heightmapResolution);

		int backBufferID = Shader.PropertyToID("_Heightmap_BackBuffer");
		cmd.GetTemporaryRT(backBufferID, textureSize, textureSize, 24, FilterMode.Point, RenderTextureFormat.R16);
		cmd.SetRenderTarget(backBufferID);
		cmd.ClearRenderTarget(true, true, Color.black);

		if (blurMat != null)
		{
			cmd.BeginSample("Blur Heightmap");

			for (int i = 0; i < blurIterations; i++)
			{
				cmd.Blit(heightmapRT, backBufferID, blurMat);
				cmd.Blit(backBufferID, heightmapRT, blurMat);
			}

			cmd.Blit(heightmapRT, backBufferID, clipMat, 0);
			cmd.CopyTexture(backBufferID, heightmapRT);

			cmd.EndSample("Blur Heightmap");
		}

		cmd.BeginSample("Clip Holes");

		cmd.Blit(heightmapRT, backBufferID, ceilMat);
		cmd.Blit(backBufferID, holesRT);

		cmd.EndSample("Clip Holes");

		cmd.ReleaseTemporaryRT(backBufferID);

		cmd.EndSample("Heightmap");

		SetupCommandBuffer_HeightmapAfterBlur(this, cmd);
	}

	private void SetupAlphamapCommands()
	{
		SetupCommandBuffer_Alphamap(this, cmd);
	}

	private void RenderTerrain()
	{
		if (cmd == null)
		{
			cmd = new CommandBuffer();
			cmd.name = "BadTerrain-" + GetInstanceID();
		}

		cmd.Clear();

		Texture2D alphamap = Terrain.terrainData.alphamapTextures[0];
		cmd.CopyTexture(alphamap, alphamapRT);

		SetupCamera();
		SetupHeightmapCommands();
		SetupAlphamapCommands();

		var prevRT = RenderTexture.active;
		RenderTexture.active = null;

		bool prevFog = RenderSettings.fog;
		RenderSettings.fog = false;

		Dictionary<Renderer, bool> rendererToPrevEnabled = new Dictionary<Renderer, bool>();
		foreach(var renderer in renderers)
		{
			rendererToPrevEnabled.Add(renderer, renderer.enabled);
			renderer.enabled = true;
		}

		BeforeExecuteCommandBuffer(this, cmd);

		Graphics.ExecuteCommandBuffer(cmd);

		AfterExecuteCommandBuffer(this, cmd);

		RenderTexture.active = prevRT;
		RenderSettings.fog = prevFog;

		foreach (var kvp in rendererToPrevEnabled)
			kvp.Key.enabled = kvp.Value;
		
		DestroyImmediate(fakeCam.gameObject);

		//Debug.Break();
	}

	private void OnDrawGizmosSelected()
	{
		var bounds = CalculateBounds();

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
}
