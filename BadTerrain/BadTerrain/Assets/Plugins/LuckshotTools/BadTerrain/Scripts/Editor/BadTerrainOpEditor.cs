using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BadTerrainOp), true)]
public class BadTerrainOpEditor : Editor
{
	BadTerrainOp TerrainOp => target as BadTerrainOp;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var editModeProp = serializedObject.FindProperty("editMode");
		bool editMode = editModeProp.boolValue;

		if (editMode)
		{
			if (GUILayout.Button("Stop Editing"))
			{
				TerrainOp.SetEditMode(false);
				TerrainOp.Regenerate();
			}
		}
		else
		{
			if (GUILayout.Button("Start Editing"))
			{
				TerrainOp.SetEditMode(true);
			}
		}

		if(GUILayout.Button("Regenerate"))
		{
			TerrainOp.Regenerate();
		}
	}
}
