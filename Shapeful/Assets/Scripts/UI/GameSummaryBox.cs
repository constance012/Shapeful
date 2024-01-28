using CSTGames.DataPersistence;
using System;
using TMPro;
using UnityEngine;

public class GameSummaryBox : MonoBehaviour, ISaveDataTransceiver
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI highscoreText;
	[SerializeField] private TextMeshProUGUI summaryScoreText;

	[Header("Earned Gem Shards Settings"), Space]
	[SerializeField, Tooltip("How many gem shards can be earned per score.")] private float scoreConvertRatio;
	[SerializeField, Tooltip("How many gem shards can be earned per elapsed second.")] private float timeConvertRatio;
	[SerializeField] private TextMeshProUGUI gemEarnedText;

	[Header("Rewarded Content Settings"), Space]
	[SerializeField] private RewardedAdsButton continueButton;

	[Tooltip("The required cooldown for a continue game attempt. In SECONDS.")]
	public double continueAttemptCooldown;
	[SerializeField] private TextMeshProUGUI continueAttemptText;

	public CooldownBasedData ContinueAttempts { get; private set; }
	public bool AllowContinue => ContinueAttempts.value > 0;

	// Private fields.
	private int _earnedGemShards;

	#region Save and Load data
	public bool Ready => ContinueAttempts != null;

	public void SaveData(GameData data)
	{
		data.gemShards += _earnedGemShards;
		data.continueAttempts = ContinueAttempts.value;
		data.ContinueAttemptRemainingCD = ContinueAttempts.FromRemainingCD();
	}

	public void LoadData(GameData data)
	{
		ContinueAttempts.LoadValueSinceLastPlayed(data);
	}
	#endregion

	public void Initialize()
	{
		ContinueAttempts = new CooldownBasedData(GameManager.MAX_CONTINUE_ATTEMPT, continueAttemptCooldown);
	}

	public void Show(int currentScore, int highscore)
	{
		if (currentScore > highscore)
		{
			highscoreText.text = $"<color=#B02E2E> NEW BEST: {currentScore} </color>";
			summaryScoreText.text = $"----------------\nGOOD JOB, MATE!";
		}
		else
		{
			highscoreText.text = $"BEST: {highscore}";
			summaryScoreText.text = $"----------------\nCURRENT: {currentScore}";
		}

		continueAttemptText.text = $"REMAINING ATTEMPTS: <color=#AE2929> {ContinueAttempts.value} </color>";

		transform.parent.gameObject.SetActive(true);
	}

	public void Hide()
	{
		transform.parent.gameObject.SetActive(false);
	}

	public void UpdateContinueAttemptCooldown()
	{
		if (GameManager.Instance.GameOver && ContinueAttempts.IsOnCooldown)
		{
			// TODO - Increase the attempt when the cooldown is finished.
			ContinueAttempts.RemainingCD -= TimeSpan.FromSeconds(Time.deltaTime);

			continueButton.SetStatus(ContinueAttempts.value >= 1, ContinueAttempts.NextValueRemainingCD);
			continueAttemptText.text = $"REMAINING ATTEMPTS: <color=#AE2929> {ContinueAttempts.value} </color>";
		}
	}

	public void CalculateGemShards(int currentScore, TimeSpan elapsedTime)
	{
		int scoreShards = Mathf.CeilToInt(currentScore * scoreConvertRatio);
		int timeShards = Mathf.FloorToInt((float)elapsedTime.TotalSeconds * timeConvertRatio);

		_earnedGemShards = scoreShards + timeShards;

		gemEarnedText.text = $"+{_earnedGemShards}";
	}
}
