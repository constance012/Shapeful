using UnityEngine;

public class PowerUpHolder : Collectable
{
	[Header("Attached Power Up"), Space]
	[SerializeField] private PowerUp powerUp;

	[Header("Holder References"), Space]
	[SerializeField] private ParticleSystem applyEffect;
	[SerializeField] private SpriteRenderer spriteRenderer;

	// Private fields.
	private PowerUp _currentPowerUp;

	private void Start()
	{
		_currentPowerUp = Instantiate(powerUp);
		_currentPowerUp.name = powerUp.name;

		spriteRenderer.sprite = _currentPowerUp.icon;
	}

	protected override void OnCollected()
	{
		AudioManager.Instance.PlayWithRandomPitch("Power Up", .8f, 1.2f);

		// TODO: Add data to power up manager.
		// Apply the effect instantly.
		if (PowerUpManager.Instance.TryApply(_currentPowerUp))
		{
			if (applyEffect != null)
				Instantiate(applyEffect, player.transform.position, Quaternion.identity);
			
			player.OnPowerUpReceived(_currentPowerUp.powerUpName);
		}
	}
}
