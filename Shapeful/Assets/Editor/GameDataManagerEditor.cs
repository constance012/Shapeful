using CSTGames.DataPersistence;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDataManager))]
public class GameDataManagerEditor : Editor
{
	private GameDataManager _inspectedObject;
	private SerializedObject _managerSerializedObj;

	private SerializedProperty _enableManager;
	private SerializedProperty _useEncryption;

	private void OnEnable()
	{
		_inspectedObject = (GameDataManager)target;
		_managerSerializedObj = new SerializedObject(_inspectedObject);
		_enableManager = _managerSerializedObj.FindProperty("enableManager");
		_useEncryption = _managerSerializedObj.FindProperty("useEncryption");
	}

	public override void OnInspectorGUI()
	{
		_managerSerializedObj.Update();

		GUIStyle style = new GUIStyle();
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;

		// Enable or Disable the manager.
		GUILayout.Label(new GUIContent("Enable Game Data Manager", "Check this box to specify whether to enable this component."), style);
		GUILayout.Space(5f);

		bool isEnabled = GUILayout.Toggle(_enableManager.boolValue, " Enable");
		_enableManager.boolValue = isEnabled;
		GUI.enabled = isEnabled;

		DrawDefaultInspector();

		GUILayout.Space(10f);

		// Manual Encryption.
		GUILayout.Label(new GUIContent("Encryption", "Check this box to specify whether to encrypt the data or not. " +
										"Changes will happened immediately if the value of this box is changed."), style);
		GUILayout.Space(5f);

		bool newValue = GUILayout.Toggle(_useEncryption.boolValue, " Use Encryption");
		_useEncryption.boolValue = newValue;

		if (newValue != _inspectedObject.useEncryption)
		{
			if (newValue)
			{
				Debug.Log("Encrypting data...");
				_inspectedObject.EncryptManually();
			}
			else
			{
				Debug.Log("Decrypting data...");
				_inspectedObject.DecryptManually();
			}
		}

		_managerSerializedObj.ApplyModifiedProperties();
	}
}
