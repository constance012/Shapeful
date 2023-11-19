using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using CSTGames.DataPersistence;
using UnityEngine.Events;
using System.Collections;
using System;

public class GameManager : Singleton<GameManager>, ISaveDataTransceiver
{
	[Header("Slow Motion Settings"), Space]
	[SerializeField, Tooltip("How slow would the effect be?"), Range(.01f, .5f)]
	private float slowFactor;
	
	[SerializeField, Tooltip("Duration before returning to normal speed, in seconds.")]
	private float slowDownDuration;

	[Header("Rewarded Content Settings"), Space]
	[SerializeField] private RewardedAdsButton continueButton;

	[SerializeField, Tooltip("The required cooldown for a continue game attempt. In SECONDS.")]
	private double continueAttemptCooldown;
	[SerializeField] private TextMeshProUGUI continueAttemptText;

	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI gameScoreText;
	[SerializeField] private TextMeshProUGUI healthText;

	[Space]
	[SerializeField] private TextMeshProUGUI highscoreText;
	[SerializeField] private TextMeshProUGUI summaryScoreText;
	
	[Space]
	[SerializeField] private GameObject playerUI;
	[SerializeField] private GameObject gameOverPanel;
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
	private Animator _gameScoreAnimator;
	private DateTime _dataLastLoaded;
	private CooldownBasedData _continueAttempts;

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
		_continueAttempts = new CooldownBasedData(GameData.MAX_CONTINUE_ATTEMPT, continueAttemptCooldown);

		Debug.Log("Game manager awoken.");
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

		if (GameOver && _continueAttempts.IsOnCooldown)
		{
			// TODO - Increase the attempt when the cooldown is finished.
			_continueAttempts.RemainingCD -= TimeSpan.FromSeconds((float)Time.deltaTime);

			continueButton.SetStatus(_continueAttempts.value >= 1, _continueAttempts.NextValueRemainingCD);
			continueAttemptText.text = $"REMAINING ATTEMPTS: <color=#AE2929> {_continueAttempts.value} </color>";
		}
	}

	#region Save and Load Data.
	public void LoadData(GameData data)
	{
		_highscore = data.highscore;

		_dataLastLoaded = DateTime.FromBinary(data.lastUpdated);

		_continueAttempts.LoadValueSinceLastPlayed(data);
	}

	public void SaveData(GameData data)
	{
		data.highscore = _highscore;
		data.continueAttempts = _continueAttempts.value;
		data.ContinueAttemptRemainingCD = _continueAttempts.FromRemainingCD();
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
			_continueAttempts.RemainingCD -= (DateTime.Now - _dataLastLoaded);

			GameOver = true;

			onGameOver?.Invoke();
			Invoke("ShowGameOverScreen", 1.5f);
		}

		healthText.text = currentHealth.ToString();
	}

	#region UI Callback Methods
	public void ContinueGame()
	{
		if (_continueAttempts.value > 0)
		{
			_dataLastLoaded = DateTime.Now;

			GameOver = false;
			onGameContinue?.Invoke();

			gameOverPanel.SetActive(false);
			playerUI.SetActive(true);

			_continueAttempts.UnaryDecrement();
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
		if (_score > _highscore)
		{
			highscoreText.text = $"<color=#B02E2E> NEW BEST: {_score} </color>";
			summaryScoreText.text = $"----------------\nGOOD JOB, MATE!";

			_highscore = _score;
		}
		else
		{
			highscoreText.text = $"BEST: {_highscore}";
			summaryScoreText.text = $"----------------\nCURRENT: {_score}";
		}

		continueAttemptText.text = $"REMAINING ATTEMPTS: <color=#AE2929> {_continueAttempts.value} </color>";
		
		GameDataManager.Instance.SaveGame();
		PowerUpManager.Instance.ForceRemoveAll();

		playerUI.SetActive(false);
		gameOverPanel.SetActive(true);

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

			if (!previousTime.Equals(gameScoreText.text) && timeLeft > 0f)
			{
				_gameScoreAnimator.Play("Count Down", 0, 0f);
			}

			previousTime = gameScoreText.text;
		}

		gameScoreText.text = _score.ToString();
		_gameScoreAnimator.Play("Slide In");

		playerUI.SetActive(true);
		IsPause = false;
	}
}
