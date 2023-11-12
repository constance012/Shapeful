using UnityEngine;

[CreateAssetMenu(fileName = "New Power-Up", menuName = "Power-Ups/Damage Shield")]
public class DamageShield : PowerUp
{
	[Header("Damage Negation"), Space]
	[Range(0f, 1f)] public float damageReductionScale;
	public bool breakable;

	public override void ApplyEffect()
	{
		GameManager.Instance.DamageReduction = damageReductionScale;
	}

	public override void RemoveEffect()
	{
		GameManager.Instance.DamageReduction = 0f;
	}
}
