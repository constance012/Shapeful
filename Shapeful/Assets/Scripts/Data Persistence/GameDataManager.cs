using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CSTGames.DataPersistence
{
	public class GameDataManager : PersistentSingleton<GameDataManager>
	{
		[Header("Debugging"), Space]
		[ReadOnly] public bool enableManager;

		[Header("Save File Configurations")]
		[SerializeField] private string subFolder;
		[SerializeField] private string fileName;
		[SerializeField] private bool useEncryption;

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

		#if UNITY_EDITOR
		private void Start()
		{
			if (SceneManager.GetSceneByName("Scenes/Game").isLoaded)
			{
				Debug.Log("Loads game data in editor.");
				this._transceivers = GetAllTransceivers();
				LoadGame();
			}
		}
		#endif
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

			foreach (ISaveDataTransceiver transceiver in _transceivers)
			{
				transceiver.SaveData(_currentData);
			}

			_saveHandler.SaveDataToFile(_currentData);
		}

		public void DistributeDataToTransceivers()
		{
			foreach (ISaveDataTransceiver transceiver in _transceivers)
				transceiver.LoadData(_currentData);
		}
		#endregion

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (!enableManager)
				return;

			Debug.Log($"Loaded scene: {scene.name}", this);
			this._transceivers = GetAllTransceivers();

			if (_currentData != null)
				DistributeDataToTransceivers();
		}

		private List<ISaveDataTransceiver> GetAllTransceivers()
		{
			IEnumerable<ISaveDataTransceiver> _dataTransceivers = FindObjectsOfType<MonoBehaviour>(true).
																	OfType<ISaveDataTransceiver>();

			return new List<ISaveDataTransceiver>(_dataTransceivers);
		}
	}
}