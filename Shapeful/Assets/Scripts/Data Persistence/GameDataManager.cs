using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

namespace CSTGames.DataPersistence
{
	public class GameDataManager : PersistentSingleton<GameDataManager>
	{
		[Header("Debugging"), Space]
		[ReadOnly] public bool enableManager;

		[Header("Save File Configurations")]
		[SerializeField] private string subFolder;
		[SerializeField] private string fileName;
		[ReadOnly] public bool useEncryption;

		// Private fields.
		private List<ISaveDataTransceiver> _transceivers;
		private SaveFileHandler<GameData> _saveHandler;
		private GameData _currentData;

		#region Unity Methods.
		protected override void Awake()
		{
			base.Awake();

			if (!enableManager)
				Debug.LogWarning("DEBUGGING: Game Data Manager is currently disabled, game data will not be persisted between sessions.");

			_saveHandler = new SaveFileHandler<GameData>(Application.persistentDataPath, subFolder, fileName, useEncryption);
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void Start()
		{
			this._transceivers = GetAllTransceivers();
			
		#if UNITY_EDITOR
			if (SceneManager.GetSceneByName("Scenes/Game").isLoaded)
			{
				Debug.Log("Loads game data in editor.");
				LoadGame();
			}
		#endif
		}
		#endregion

		#region Game Data Management.
		public void NewGame()
		{
			_currentData = new GameData();
		}

		public void LoadGame(bool distributeData = true)
		{
			if (!enableManager)
				return;

			_currentData = _saveHandler.LoadDataFromFile();

			if (_currentData == null)
			{
				Debug.LogWarning("WARNING: No game data was found. Starting a new game.");
				NewGame();
			}

			if (distributeData)
				DistributeDataToTransceivers();
		}

		public void SaveGame()
		{
			if (!enableManager)
				return;

			if (_currentData == null)
			{
				Debug.LogWarning("WARNING: Unable to save because of missing data or data corruption. Aborting...");
				return;
			}

			// Notify all transceivers to write their data into the current data object.
			foreach (ISaveDataTransceiver transceiver in _transceivers)
			{
				transceiver.SaveData(_currentData);
			}

			// Timestamp the data.
			_currentData.lastUpdated = DateTime.Now.ToBinary();

			// Save that data into a file on the local machine.
			_saveHandler.SaveDataToFile(_currentData);
		}

		public void DistributeDataToTransceivers()
		{
			if (_currentData != null)
			{
				foreach (ISaveDataTransceiver transceiver in _transceivers)
					transceiver.LoadData(_currentData);
			}
		}

		private List<ISaveDataTransceiver> GetAllTransceivers()
		{
			IEnumerable<ISaveDataTransceiver> _dataTransceivers = FindObjectsOfType<MonoBehaviour>(true).
																	OfType<ISaveDataTransceiver>();

			return new List<ISaveDataTransceiver>(_dataTransceivers);
		}
		#endregion

		#region Data Encryption/Decryption.
		public void EncryptManually()
		{
			if (useEncryption)
				return;

			SaveFileHandler<GameData> handler = new SaveFileHandler<GameData>(Application.persistentDataPath, subFolder, fileName, false);

			string fullPath = Path.Combine(Application.persistentDataPath, subFolder, fileName);

			if (!File.Exists(fullPath))
			{
				Debug.LogError($"No player data was found at: {fullPath}.\n Thus can not perform the ENCRYPTION process.");
				return;
			}

			// Load the readable data.
			GameData data = handler.LoadDataFromFile();

			// Turn on encryption and save the data again.
			handler.UseEncryption = true;
			handler.SaveDataToFile(data);

			Debug.LogWarning("ENCRYPTION process complete!");

			useEncryption = true;
		}

		public void DecryptManually()
		{
			if (!useEncryption)
				return;

			SaveFileHandler<GameData> handler = new SaveFileHandler<GameData>(Application.persistentDataPath, subFolder, fileName, true);

			string fullPath = Path.Combine(Application.persistentDataPath, subFolder, fileName);

			if (!File.Exists(fullPath))
			{
				Debug.LogError($"No player data was found at: {fullPath}.\n Thus can not perform the DECRYPTION process.");
				return;
			}

			// Load the encrypted data.
			GameData data = handler.LoadDataFromFile();

			// Turn off encryption and save the data again.
			handler.UseEncryption = false;
			handler.SaveDataToFile(data);

			Debug.LogWarning("DECRYPTION process complete!");

			useEncryption = false;
		}
		#endregion

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (!enableManager)
				return;

			Debug.Log($"Loaded scene: {scene.name}", this);
			this._transceivers = GetAllTransceivers();

			DistributeDataToTransceivers();
		}
	}
}