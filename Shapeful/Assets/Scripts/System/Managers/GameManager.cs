using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using CSTGames.DataPersistence;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Data;

public class GameManager : Singleton<GameManager>, ISaveDataTransceiver
{
	[Header("Slow Motion Settings"), Space]
	[SerializeField, Tooltip("How slow would the effect be?"), Range(.01f, .5f)]
	private float slowFactor;
	
	[SerializeField, Tooltip("Duration before returning to normal speed, in seconds.")]
	private float slowDownDuration;

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI gameScoreText;
	[SerializeField] private TextMeshProUGUI healthText;
	
	[Space]
	[SerializeField] private GameSummaryBox gameSummary;
	[SerializeField] private GameObject playerUI;
	[SerializeField] private GameObject pauseMenuPanel;

	[Space]
	[SerializeField] private Animator healthAnimator;

	[Header("Events"), Space]
	public UnityEvent onGameOver = new UnityEvent();
	public UnityEvent onGameContinue = new UnityEvent();

	// Properties.
	public int CurrentScore => _score;
	public int ScoreMultiplier { get; set; } = 1;
	public float DamageReduction { get; set; } = 0f;
	public bool GameOver { get; private set; }
	public static bool IsPause { get; private set; }

	// Private fields.
	public const uint MAX_CONTINUE_ATTEMPT = 3;

	private Animator _gameScoreAnimator;
	private DateTime _dataLastLoaded;

	private int _highscore = 0;
	private int _score = 0;
	private float _fixedTimeStep;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadStaticFields()
	{
		IsPause = false;
	}

	protected override void Awake()
	{
		base.Awake();

		_gameScoreAnimator = gameScoreText.GetComponentInParent<Animator>();
		gameSummary.Initialize();
	}

	private void Start()
	{
		_fixedTimeStep = Time.fixedDeltaTime;

		gameScoreText.text = _score.ToString();
		
		// Add listener callbacks for singleton class.
		onGameOver.AddListener(() => ShapeSpawner.Instance.gameObject.SetActive(false));
		onGameContinue.AddListener(() => ShapeSpawner.Instance.gameObject.SetActive(true));

		//Debug.Log(TimeSpan.FromSeconds(30) / TimeSpan.FromSeconds(60));
	}

	private void Update()
	{
		SpeedUpTime();
		gameSummary.UpdateContinueAttemptCooldown();
	}

	#region Save and Load Data
	public bool Ready => true;

	public void SaveData(GameData data)
	{
		data.highscore = _highscore;
	}
	
	public void LoadData(GameData data)
	{
		_highscore = data.highscore;
		_dataLastLoaded = DateTime.FromBinary(data.lastUpdated);
	}
	#endregion

	public void UpdateScore(int score)
	{
		_score += score * ScoreMultiplier;
		gameScoreText.text = _score.ToString();
		_gameScoreAnimator.Play("Increment", 0, 0f);
	}

	public void UpdatePlayerHealth(int maxHealth, int currentHealth)
	{
		healthAnimator.speed = Mathf.InverseLerp(maxHealth, 1, currentHealth);

		if (currentHealth <= 0)
		{
			GameOver = true;

			gameSummary.ContinueAttempts.RemainingCD -= (DateTime.Now - _dataLastLoaded);

			onGameOver?.Invoke();
			Invoke("ShowGameOverScreen", 1.5f);
		}

		healthText.text = currentHealth.ToString();
	}

	#region UI Callback Methods
	public void ContinueGame()
	{
		if (gameSummary.AllowContinue)
		{
			_dataLastLoaded = DateTime.Now;

			GameOver = false;
			onGameContinue?.Invoke();

			gameSummary.Hide();
			playerUI.SetActive(true);

			gameSummary.ContinueAttempts.UnaryDecrement();
		}
	}

	public void RestartGame()
	{
		LevelManager.Instance.ReloadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToMenu()
	{
		LevelManager.Instance.LoadScene("Scenes/Menu");
		
		Time.timeScale = 1f;
		IsPause = false;
	}

	public void PauseGame()
	{
		pauseMenuPanel.SetActive(true);
		playerUI.SetActive(false);

		Time.timeScale = 0f;
		IsPause = true;
	}

	public void UnpauseGame()
	{
		pauseMenuPanel.SetActive(false);
		SlowDownTime();

		StartCoroutine(UnpauseCountDown());
	}
	#endregion

	public void SlowDownTime()
	{
		Time.timeScale = slowFactor;
		Time.fixedDeltaTime = _fixedTimeStep * Time.timeScale;
	}

	/// <summary>
	/// Gradully speeds up time back to normal.
	/// </summary>
	private void SpeedUpTime()
	{
		if (!IsPause && Time.timeScale < 1f)
		{
			Time.timeScale += (1f / slowDownDuration) * Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Min(Time.timeScale, 1f);
			Time.fixedDeltaTime = _fixedTimeStep * Time.timeScale;
		}
	}

	private void ShowGameOverScreen()
	{
		playerUI.SetActive(false);

		gameSummary.CalculateGemShards(_score, DateTime.Now - _dataLastLoaded);
		gameSummary.Show(_score, _highscore);
		_highscore = Mathf.Max(_score, _highscore);
		
		PowerUpManager.Instance.ForceRemoveAll();
		GameDataManager.Instance.SaveGame();

		InterstitialAdsManager.Instance.TryShowAd();
	}

	private IEnumerator UnpauseCountDown()
	{	
		float timeLeft = 3f;
		string previousTime = "";

		while(timeLeft >= 0f)
		{
			yield return null;

			timeLeft -= Time.unscaledDeltaTime;

			gameScoreText.text = Mathf.FloorToInt(timeLeft + 1f).ToString("0");

			if (!previousTime.Equals(gameScoreText.text))
			{
				_gameScoreAnimator.Play("Count Down", 0, 0f);
				previousTime = gameScoreText.text;
			}
		}

		gameScoreText.text = _score.ToString();
		_gameScoreAnimator.Play("Slide In");

		playerUI.SetActive(true);
		IsPause = false;
	}
}
