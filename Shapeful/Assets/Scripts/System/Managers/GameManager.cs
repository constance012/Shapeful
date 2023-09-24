using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using CSTGames.DataPersistence;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>, ISaveDataTransceiver
{
	[Header("Events"), Space]
	public UnityEvent onGameOver = new UnityEvent();

	[Header("Reference"), Space]
	[SerializeField] private TextMeshProUGUI gameScoreText;
	[SerializeField] private TextMeshProUGUI healthText;

	[Space]
	[SerializeField] private GameObject normalGamePanel;
	[SerializeField] private GameObject gameOverPanel;
	
	[Space]
	[SerializeField] private TextMeshProUGUI highscoreText;
	[SerializeField] private TextMeshProUGUI summaryScoreText;

	[HideInInspector] public bool gameOver;

	// Private fields.
	private int _highscore = 0;
	private int _score = 0;

	private void Start()
	{
		gameScoreText.text = _score.ToString();
		onGameOver.AddListener(() => ShapeSpawner.Instance.gameObject.SetActive(false));
	}

	public void LoadData(GameData data)
	{
		_highscore = data.highscore;
	}

	public void SaveData(GameData data)
	{
		data.highscore = _highscore;
	}

	public void UpdateScore()
	{
		_score++;
		gameScoreText.text = _score.ToString();

		if (_score % 10 == 0)
		{
			CameraManager.Instance.RandomRotationSpeed();
		}
	}

	public void UpdatePlayerHealth(int health)
	{
		if (health == 0)
		{
			gameOver = true;
			onGameOver?.Invoke();
			Invoke("ShowGameOverScreen", 1.5f);
		}

		healthText.text = health.ToString();
	}

	public void ShowGameOverScreen()
	{
		if (_score > _highscore)
		{
			highscoreText.text = $"<color=#B02E2E> NEW BEST: {_score} </color>";
			summaryScoreText.text = $"----------------\nGOOD JOB, MATE!";

			_highscore = _score;
			GameDataManager.Instance.SaveGame();
		}
		else
		{
			highscoreText.text = $"BEST: {_highscore}";
			summaryScoreText.text = $"----------------\nCURRENT: {_score}";
		}

		normalGamePanel.SetActive(false);
		gameOverPanel.SetActive(true);
	}

	public void RestartGame()
	{
		LevelManager.Instance.ReloadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToMenu()
	{
		LevelManager.Instance.LoadScene("Scenes/Menu");
	}
}
