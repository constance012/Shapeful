using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using CSTGames.DataPersistence;

public class GameManager : Singleton<GameManager>
{
	[Header("Reference"), Space]
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI healthText;

	[Space]
	[SerializeField] private GameObject normalGamePanel;
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private TextMeshProUGUI highscoreText;

	[HideInInspector] public bool gameOver;

	[Header("Save File Configurations")]
	[SerializeField] private string subFolder;
	[SerializeField] private string fileName;
	[SerializeField] private bool useEncryption;

	// Private fields.
	private SaveFileHandler<GameData> _saveHandler;
	private GameData _gameData;
	private int _score = 0;

	protected override void Awake()
	{
		base.Awake();
		_saveHandler = new SaveFileHandler<GameData>(Application.persistentDataPath, subFolder, fileName, useEncryption);
	}

	private void Start()
	{
		_gameData = _saveHandler.LoadDataFromFile();

		scoreText.text = _score.ToString();
	}

	public void UpdateScore()
	{
		_score++;
		scoreText.text = _score.ToString();

		if (_score % 10 == 0)
		{
			CameraManager.Instance.RandomRotationSpeed();
		}
	}

	public void UpdatePlayerHealth(int health)
	{
		if (health == 0)
		{
			GameOver();
		}

		healthText.text = health.ToString();
	}

	public void GameOver()
	{
		if (_score > _gameData.highscore)
		{
			highscoreText.text = $"NEW BEST: {_score}";
			_gameData.highscore = _score;
		}
		else
			highscoreText.text = $"BEST: {_gameData.highscore}";

		_saveHandler.SaveDataToFile(_gameData);

		normalGamePanel.SetActive(false);
		gameOverPanel.SetActive(true);

		ShapeSpawner.Instance.gameObject.SetActive(false);
		gameOver = true;
	}

	public void RestartGame()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToMenu()
	{
		SceneManager.LoadSceneAsync("Scenes/Menu");
	}
}
