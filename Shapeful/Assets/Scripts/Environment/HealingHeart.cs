using UnityEngine;

public class HealingHeart : Collectible
{
	[Header("Properties"), Space]
	[SerializeField] private int healAmount;

	protected override void OnCollected()
	{
		AudioManager.Instance.PlayWithRandomPitch("Heart Pickup", .7f, 1.2f);
		player.Heal(healAmount);
	}
}
