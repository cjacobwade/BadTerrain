using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(LensManagerBase), true)]
public class LensManagerBaseDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position.height = EditorGUIUtility.singleLineHeight;

		var requestsProp = property.FindPropertyRelative("activeRequests");
		var valuesProp = property.FindPropertyRelative("evaluateValues");
		var resultProp = property.FindPropertyRelative("cachedResult");

		string formatString = string.Format("{0} ({1}) - Num Requests: {2}", label.text, GetPropertyLabel(resultProp), requestsProp.arraySize);

		bool playingAndAnyRequests = Application.IsPlaying(property.serializedObject.targetObject) && requestsProp.arraySize > 0;

		GUIStyle style = new GUIStyle(playingAndAnyRequests ? EditorStyles.boldLabel : EditorStyles.label);
		style.richText = true;

		EditorGUI.PropertyField(position, property, new GUIContent(formatString), false);

		if (property.isExpanded)
		{
			position.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.indentLevel++;

			for (int i = 0; i < requestsProp.arraySize; i++)
			{
				var requestProp = requestsProp.GetArrayElementAtIndex(i);
				if (requestProp != null)
				{
					string name = requestProp.FindPropertyRelative("Name").stringValue;
					int instanceID = requestProp.FindPropertyRelative("instanceID").intValue;

					SerializedProperty valueProp = valuesProp.GetArrayElementAtIndex(i);
					string valueText = GetPropertyLabel(valueProp);
					string formatText = !string.IsNullOrEmpty(name) ? "{0}: {1}" : "{1}";
					string labelText = string.Format(formatText, name, valueText);

					GUI.enabled = false;

					EditorGUI.PropertyField(position, valueProp, new GUIContent(labelText), true);
					position.y += EditorGUI.GetPropertyHeight(valueProp, true);

					GUI.enabled = true;

					//EditorGUI.LabelField(position, string.Format(formatText, name, valueText));
					int controlID = GUIUtility.GetControlID(FocusType.Passive) - 1;

					EventType eventType = Event.current.GetTypeForControl(controlID);
					if (eventType == EventType.MouseDown && instanceID != -1)
						EditorGUIUtility.PingObject(instanceID);
				}
			}

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}

	private string GetPropertyLabel(SerializedProperty prop)
	{
		switch (prop.propertyType)
		{
			case SerializedPropertyType.Float:
				return prop.floatValue.ToString();
			case SerializedPropertyType.Vector2:
				return prop.vector2Value.ToString();
			case SerializedPropertyType.Vector3:
				return prop.vector3Value.ToString();
			case SerializedPropertyType.Vector4:
				return prop.vector4Value.ToString();
			case SerializedPropertyType.Integer:
				return prop.intValue.ToString();
			case SerializedPropertyType.Color:
				return prop.colorValue.ToString();
			case SerializedPropertyType.Quaternion:
				return prop.quaternionValue.ToString();
			case SerializedPropertyType.String:
				return prop.stringValue;
			case SerializedPropertyType.Boolean:
				return prop.boolValue.ToString();
			case SerializedPropertyType.ObjectReference:
				return prop.objectReferenceValue?.name;
			case SerializedPropertyType.Enum:
				return prop.enumDisplayNames[prop.enumValueIndex];
			case SerializedPropertyType.Bounds:
				return prop.boundsValue.ToString();
			case SerializedPropertyType.BoundsInt:
				return prop.boundsIntValue.ToString();
			default:
				return string.Empty;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = EditorGUIUtility.singleLineHeight;

		if(property.isExpanded)
        {
			var requestsProp = property.FindPropertyRelative("activeRequests");
			var valuesProp = property.FindPropertyRelative("evaluateValues");

			if (valuesProp != null) // unserializable fields be like
			{
				for (int i = 0; i < requestsProp.arraySize; i++)
				{
					var requestProp = requestsProp.GetArrayElementAtIndex(i);
					if (requestProp != null)
					{
						SerializedProperty valueProp = valuesProp.GetArrayElementAtIndex(i);
						height += EditorGUI.GetPropertyHeight(valueProp, true);
					}
				}
			}
		}

		return height;
	}
}
