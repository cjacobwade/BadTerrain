using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BadTerrain))]
public class BadTerrainEditor : Editor
{
	public BadTerrain BadTerrain => target as BadTerrain;

	private void OnEnable()
	{
		if (target == null)
			return;

		// Force regenerate if duplicating
		TerrainData terrainData = BadTerrain.Terrain.terrainData;
		if (terrainData != null)
		{
			Terrain[] terrains = FindObjectsOfType<Terrain>();
			foreach (var terrain in terrains)
			{
				if(terrain != BadTerrain.Terrain &&
					terrain.terrainData == terrainData)
				{
					BadTerrain.Terrain.terrainData = null;
					BadTerrain.Regenerate();
					break;
				}
			}
		}

		Undo.undoRedoPerformed -= OnUndo;
		Undo.undoRedoPerformed += OnUndo;
	}

	private void OnDisable()
	{
		Undo.undoRedoPerformed -= OnUndo;
	}

	private void OnUndo()
	{
		if (targets == null || targets.Length == 0 || target == null)
		{
			Undo.undoRedoPerformed -= OnUndo;
			return;
		}

		BadTerrain.Regenerate();

		var editModeProp = serializedObject.FindProperty("editMode");
		bool editMode = editModeProp.boolValue;

		BadTerrain.SetEditMode(editMode);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var editModeProp = serializedObject.FindProperty("editMode");
		bool editMode = editModeProp.boolValue;

		if(editMode)
		{
			if(GUILayout.Button("Stop Editing"))
			{
				Undo.RecordObject(BadTerrain, "Change Edit Mode");
				BadTerrain.SetEditMode(false);
				EditorUtility.SetDirty(BadTerrain);
			}
		}
		else
		{
			if(GUILayout.Button("Start Editing"))
			{
				Undo.RecordObject(BadTerrain, "Change Edit Mode");
				BadTerrain.SetEditMode(true);
				EditorUtility.SetDirty(BadTerrain);
			}
		}

		if(GUILayout.Button("Regenerate"))
		{
			Undo.RecordObject(BadTerrain, "Regenerate");
			BadTerrain.Regenerate();
			BadTerrain.SetEditMode(false);
			EditorUtility.SetDirty(BadTerrain);
		}

		if(GUILayout.Button("Reset Splatmap"))
		{
			Undo.RecordObject(BadTerrain, "Reset Splatmap");
			BadTerrain.ResetSplatmap();
			EditorUtility.SetDirty(BadTerrain);
		}
	}
}
