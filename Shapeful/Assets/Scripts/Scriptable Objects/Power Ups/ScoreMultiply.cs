using UnityEngine;

[CreateAssetMenu(fileName = "New Power-Up", menuName = "Power-Ups/Score Multiply")]
public class ScoreMultiply : PowerUp
{
	[Header("Score Multiplier"), Space]
	[Min(1)] public int multiplier;

	public override void ApplyEffect()
	{
		GameManager.Instance.ScoreMultiplier *= multiplier;
	}

	public override void RemoveEffect()
	{
		GameManager.Instance.ScoreMultiplier /= multiplier;
	}
}
