using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
	[Header("General Properties"), Space]
	public string powerUpName;
	[Min(1f)] public float duration;

	[Space] public Sprite icon;
	[TextArea(3, 10)] public string description;

	// Properties.
	public PowerUpIndicator IndicatorUI
	{
		get { return _indicatorUI; }
		set
		{
			_indicatorUI = value;
			_indicatorUI.Initialize(this);
		}
	}

	public bool DurationExpired { get; private set; }

	// Private fields.
	private PowerUpIndicator _indicatorUI;

	public void UpdateDuration()
	{
		duration -= Time.deltaTime;

		_indicatorUI.UpdateUI(duration);

		if (duration <= 0f)
		{
			Destroy(_indicatorUI.gameObject);
			DurationExpired = true;
		}
	}

	// TODO: Add to the power up from the manager's list.
	// TODO: Apply the effect to the player.
	public abstract void ApplyEffect();

	// TODO: Remove the power up from the manager's list.
	// TODO: Reverse the effect to normal.
	public abstract void RemoveEffect();

	public bool CompareName(string target)
	{
		return powerUpName.Equals(target);
	}
}
