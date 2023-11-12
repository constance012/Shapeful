using UnityEngine;

public class PowerUpHolder : Collectable
{
	[Header("Attached Power Up"), Space]
	[SerializeField] private PowerUp powerUp;

	[Header("References"), Space]
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
		player.PowerUpReceived(_currentPowerUp.powerUpName);

		// TODO: Add data to power up manager.
		// Apply the effect instantly.
		if (PowerUpManager.Instance.TryApply(_currentPowerUp))
		{
			_currentPowerUp.ApplyEffect();
		}
	}
}
