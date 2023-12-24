using UnityEngine;

public class BonusStar : Collectable
{
	[Header("Bonus Score Random Range")]
	[SerializeField, Min(1), Tooltip("The upper cap of a range starting at 1.")]
	private int rangeMax;

	[SerializeField, Min(1), Tooltip("A multiplier of the bonus score.")]
	private int bonusMultiplier;

	protected override void OnCollected()
	{
		// TODO: Play a different pickup sound.
		AudioManager.Instance.PlayWithRandomPitch("Star Pickup", .7f, 1.2f);

		int bonusScore = Random.Range(1, rangeMax + 1) * bonusMultiplier;
		GameManager.Instance.UpdateScore(bonusScore);

		player.GenerateDamageText($"+{bonusScore} Scores", DamageText.BonusScoreColor, DamageTextStyle.Small);
	}
}
