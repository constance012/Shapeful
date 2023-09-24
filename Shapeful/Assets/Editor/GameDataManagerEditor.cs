using CSTGames.DataPersistence;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDataManager))]
public class GameDataManagerEditor : Editor
{
	GameDataManager inspectedObject;
	SerializedObject managerSerializedObj;
	SerializedProperty enableManager;

	private void OnEnable()
	{
		inspectedObject = (GameDataManager)target;
		managerSerializedObj = new SerializedObject(inspectedObject);
		enableManager = managerSerializedObj.FindProperty("enableManager");
	}

	public override void OnInspectorGUI()
	{
		managerSerializedObj.Update();

		GUIStyle style = new GUIStyle();
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;

		GUILayout.Label(new GUIContent("Enable Game Data Manager", "Check this box to specify whether to enable this component."), style);
		GUILayout.Space(5f);

		bool isEnabled = GUILayout.Toggle(enableManager.boolValue, " Enable");
		enableManager.boolValue = isEnabled;
		GUI.enabled = isEnabled;

		DrawDefaultInspector();

		managerSerializedObj.ApplyModifiedProperties();
	}
}
