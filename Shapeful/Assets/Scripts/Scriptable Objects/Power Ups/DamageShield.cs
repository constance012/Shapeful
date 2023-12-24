using UnityEngine;

[CreateAssetMenu(fileName = "New Power-Up", menuName = "Power-Ups/Damage Shield")]
public class DamageShield : PowerUp, IVisualPowerUp
{
	[Header("Damage Negation"), Space]
	[Range(0f, 1f)] public float damageReductionScale;

	[Header("Active Visuals"), Space]
	public Sprite[] visuals;
	public Color visualColor;

	public Color VisualColor => visualColor;

	public override void ApplyEffect()
	{
		GameManager.Instance.DamageReduction = damageReductionScale;
	}

	public override void RemoveEffect()
	{
		GameManager.Instance.DamageReduction = 0f;
	}

	public Sprite GetSpriteAtCurrentState()
	{
		float percent = _currentUseTimes / (float)maxUseTimes;

		if (percent >= .75f)
			return visuals[0];
		else if (percent >= .5f)
			return visuals[1];
		else if (percent >= .25f)
			return visuals[2];
		else
			return visuals[3];
	}
}
